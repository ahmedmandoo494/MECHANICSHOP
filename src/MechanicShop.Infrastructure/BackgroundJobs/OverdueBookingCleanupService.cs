using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.WorkOrders.Enums;
using MechanicShop.Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MechanicShop.Infrastructure.BackgroundJobs;

public class OverdueBookingCleanupService(IServiceScopeFactory serviceScopeFactory,
ILogger<OverdueBookingCleanupService> logger,
IOptions<AppSettings> options,
TimeProvider timeProvider) : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;
    private readonly ILogger<OverdueBookingCleanupService> _logger = logger;
    private readonly AppSettings _appSettings = options.Value;
    private readonly TimeProvider _timeProvider = timeProvider;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(_appSettings.OverdueBookingCleanupFrequencyMinutes));
        while(await timer.WaitForNextTickAsync(stoppingToken))
        {
            _logger.LogInformation("Checking overdue work orders at {now}",_timeProvider.GetUtcNow());
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<IAppDbContext>();
                var cutoff = _timeProvider.GetUtcNow().AddMinutes(-_appSettings.BookingCancellationThresholdMinutes);
                var overdue =await context.WorkOrders.Where(w => 
                    w.Status == WorkOrderStatus.Scheduled && w.StartAtUtc <= cutoff
                ).ToListAsync(stoppingToken);
                if(overdue.Count > 0)
                {
                    foreach (var w in overdue)
                    {
                       var result = w.Cancel();;
                        if (result.IsError)
                        {
                            _logger.LogWarning("Failed to cancel workorder{id}:{errors}",w.Id,result.Errors);
                        }
                    }
                    await context.SaveChangesAsync(stoppingToken);
                    _logger.LogInformation("Cancelled {count} overdue work orders:{ids}",overdue.Count,overdue.Select(o => o.Id));


                }
                else
                {
                    _logger.LogInformation("No overdue work orders found.");
                }

            }catch(Exception ex)
            {
                _logger.LogError(ex,"Error cleaning up overdue work orders.");
            }
        }
    }
}