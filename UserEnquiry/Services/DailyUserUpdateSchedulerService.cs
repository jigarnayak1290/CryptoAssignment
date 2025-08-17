using UserEnquiry.DBContext;
using UserEnquiry.Models;
using UserEnquiry.Repository;

namespace UserEnquiry.Services
{
    /// <summary>
    /// Reinitialize daily, as we are daily updating user information
    /// </summary>
    public class DailyUserUpdateSchedulerService(IServiceProvider provider) : BackgroundService
    {
        private readonly IServiceProvider _provider = provider;

        /// <summary>
        /// It updates the Merkle tree daily at midnight UTC.(It assumes user data in Database will be updated daily)
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.UtcNow;
                var nextRun = now.Date.AddDays(1); // Next midnight UTC
                var delay = nextRun - now;

                await Task.Delay(delay, stoppingToken); // Wait until next day

                // Reinitialize the daily user data
                UserDataInitializeService.InitializeDailyData(_provider);
            }
        }
    }
}
