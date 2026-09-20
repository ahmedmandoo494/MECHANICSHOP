using System.Net.WebSockets;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.Dashboard.Dtos;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.WorkOrders.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MechanicShop.Application.Features.Dashboard.Queries.GetWorkOrderStats;

public sealed class GetWorkOrderStatsQueryHandler(IAppDbContext context) : IRequestHandler<GetWorkOrderStatsQuery, Result<TodayWorkOrderStatsDto>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<TodayWorkOrderStatsDto>> Handle(GetWorkOrderStatsQuery request, CancellationToken ct)
    {
        var start = request.Date.ToDateTime(TimeOnly.MinValue,DateTimeKind.Utc);
        var end = request.Date.AddDays(1).ToDateTime(TimeOnly.MinValue,DateTimeKind.Utc);
        var query = _context.WorkOrders
                    .Where(w => w.StartAtUtc >= start && w.StartAtUtc < end);
        var total = await query.CountAsync(ct);
        if(total == 0)
        {
            return new TodayWorkOrderStatsDto{
                Date = request.Date,
                Total =0,
                Scheduled =0,
                InProgress =0,
                Completed =0,
                Cancelled =0,
                TotalRevenue =0,
                TotalPartsCost =0,
                TotalLaborCost =0,
                UniqueVehicles =0,
                UniqueCustomers =0
            };
        }
        var stats = await query
            .Select(w => new
            {
                w.Status,
                w.VehicleId,
                w.Vehicle!.CustomerId,
                Revenue = w.Invoice != null ? w.Invoice.Total : 0,
                PartsCost = w.RepairTasks.SelectMany(rt => rt.Parts).Sum(p => p.Cost),
                LaborCost = w.RepairTasks.Sum (rt => rt.LaborCost),
            })
            .ToListAsync(ct);

        var totalRevenue = stats.Sum(w => w.Revenue);
        var totalPartsCost = stats.Sum(w => w.PartsCost);
        var totalLaborCost = stats.Sum(w => w.LaborCost);
        var uniqueVehicles = stats.Select(w => w.VehicleId).Distinct().Count();
        var uniqueCustomers = stats.Select(w => w.CustomerId).Distinct().Count();

        var netProfit = totalRevenue - totalPartsCost -totalLaborCost;
        var completed = stats.Count(w => w.Status == WorkOrderStatus.Completed);
        var cancelled = stats.Count(w => w.Status == WorkOrderStatus.Cancelled);

        var todayWorkOrderStatsDto = new TodayWorkOrderStatsDto
        {
            Date = request.Date,
            Total = total,
            Scheduled =stats.Count(q => q.Status == WorkOrderStatus.Scheduled),
            InProgress =stats.Count(q => q.Status == WorkOrderStatus.InProgress),
            Completed =completed,
            Cancelled =cancelled,
            TotalRevenue = totalRevenue,
            TotalPartsCost =totalPartsCost,
            TotalLaborCost = totalLaborCost,
            UniqueVehicles = uniqueVehicles,
            UniqueCustomers = uniqueCustomers,
            NetProfit =netProfit,
            ProfitMargin = totalRevenue > 0? (netProfit/totalRevenue)*100:0,
            CompletionRate =  ((decimal)completed / total) * 100,
            AverageRevenuePerOrder = totalRevenue /total,
            OrdersPerVehicle = total/uniqueVehicles,
            PartsCostRatio =totalRevenue > 0? (totalPartsCost / totalRevenue)*100:0,
            LaborCostRatio  = totalRevenue > 0?(totalLaborCost / totalRevenue)*100:0,
            CancellationRate = ((decimal)cancelled/total)*100
        };
        return todayWorkOrderStatsDto;
    }
}