using Microsoft.AspNetCore.SignalR;
using Onyx_POS.Services;
using Onyx_POS.SignalR;
using System.Data.SqlClient;

namespace Onyx_POS.HostedService
{
    public class DatabaseService(IHubContext<ConnectionStatusHub> hubContext, CommonService commonService) : BackgroundService
    {
        private readonly IHubContext<ConnectionStatusHub> _hubContext = hubContext;
        private readonly CommonService _commonService = commonService;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                bool isDbConnected = CheckDatabaseConnection();
                await _hubContext.Clients.All.SendAsync("ReceiveConnectionStatus", isDbConnected);
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
        private bool CheckDatabaseConnection()
        {
            try
            {
                var _remoteConnectionString = _commonService.GetRemoteConnectionString();
                using var connection = new SqlConnection(_remoteConnectionString);
                connection.Open();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
