using Project.Domain.Interfaces.Services;

namespace Project.Worker.Services
{
    public class ClaimAutoApproveWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ClaimAutoApproveWorker> _logger;

        public ClaimAutoApproveWorker(IServiceProvider serviceProvider, ILogger<ClaimAutoApproveWorker> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Claim Auto Approve Worker started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var claimService = scope.ServiceProvider.GetRequiredService<IClaimService>();

                    await claimService.AutoApproveProcessingClaimsAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while auto-approving claims");
                }

                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
    }
}
