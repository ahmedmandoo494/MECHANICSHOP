using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.Labors.Mappers;
using MechanicShop.Application.Features.RepairTasks.Mppers;
using MechanicShop.Application.Features.Scheduling.Dtos;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Vehicles;
using MechanicShop.Domain.WorkOrders.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MechanicShop.Application.Features.Scheduling.Queries;

public sealed class GetDailyScheduleQueryHandler(IAppDbContext context, TimeProvider timeProvider) : IRequestHandler<GetDailyScheduleQuery, Result<ScheduleDto>>
{
    private readonly IAppDbContext _context = context;
    private readonly TimeProvider _timeProvider = timeProvider;

    public async Task<Result<ScheduleDto>> Handle(GetDailyScheduleQuery query, CancellationToken ct)
    {
        var localStart = new DateTimeOffset(
            query.ScheduleDate.ToDateTime(TimeOnly.MinValue),
            query.TimeZone.GetUtcOffset(
                query.ScheduleDate.ToDateTime(TimeOnly.MinValue)));

        var localEnd = localStart.AddDays(1);


        var workorders = await _context.WorkOrders
            .Where(w =>
                w.StartAtUtc < localEnd &&
                w.EndAtUtc > localStart &&
                (query.LaborId == null || w.LaborId == query.LaborId))
            .Include(w => w.Vehicle)
            .Include(w => w.RepairTasks)
            .Include(w => w.Labor)
            .ToListAsync(ct);
        

        var now = TimeZoneInfo.ConvertTime(_timeProvider.GetUtcNow(),query.TimeZone);


        var result = new ScheduleDto
        {
            OnDate = query.ScheduleDate,
            EndOfDay = localEnd < now,
            Spots =[]
        };

        foreach (var spot in Enum.GetValues<Spots>())
        {
            var woBySpot = workorders
                .Where(w => w.Spot == spot)
                .OrderBy(w => w.StartAtUtc)
                .ToList();

            var current = localStart;
            var slots = new List<AvailabilitySlotDto>();

            while (current < localEnd)
            {
                var next = current.AddMinutes(15);

                var wo = woBySpot.FirstOrDefault(
                    w => w.StartAtUtc < next &&
                        w.EndAtUtc > current);
                if (wo != null)
                {
                    slots.Add(new AvailabilitySlotDto
                    {
                        WorkOrderId = wo.Id,
                        Spot = spot,
                        StartAt = wo.StartAtUtc,
                        EndAt = wo.EndAtUtc,
                        Vehicle = FromateVehicle(wo.Vehicle!),
                        Labor = wo.Labor!.ToDto(),
                        IsOccupied = true,
                        RepairTasks = [.. wo.RepairTasks
                            .ToList()
                            .ConvertAll(rt => rt.ToDto())],
                        WorkOrderLocked = !wo.IsEditable,
                        State = wo.Status,
                        IsAvaliable = false
                    });

                    current = wo.EndAtUtc;
                }
                else
                {
                    slots.Add(new AvailabilitySlotDto
                    {
                        Spot = spot,
                        StartAt = current,
                        EndAt = next,
                        WorkOrderLocked = false,
                        IsAvaliable = current >= now
                    });

                    current = next;
                }
            }

            result.Spots.Add(new SpotDto
            {
                Spot = spot,
                Slots = slots
            });
        }

        return result;
    }
    private static string? FromateVehicle(Vehicle vehicle) => vehicle is not null ? $"{vehicle.Make} | {vehicle.LicensePlate}": null; 
}