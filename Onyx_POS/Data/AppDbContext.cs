using System.Data;
using System.Data.SqlClient;

namespace Onyx_POS.Data
{
    public class AppDbContext
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private readonly string _connectionStringRemote;
        public AppDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
            string UserId = "absluser";
            string Password = "0c4gn2zn";
            _connectionString = string.Format(_configuration.GetConnectionString("POSDb"), UserId, Password);
            _connectionStringRemote = string.Format(_configuration.GetConnectionString("POSServerDb"), UserId, Password);
        }
        public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
        public IDbConnection CreateRemoteConnection() => new SqlConnection(_connectionStringRemote);
    }
}
