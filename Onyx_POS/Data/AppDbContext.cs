using System.Data;
using System.Data.SqlClient;

namespace Onyx_POS.Data
{
    public class AppDbContext
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        public AppDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
            string UserId = "absluser";
            string Password = "0c4gn2zn";
            string dbName = "POS";
            string ServerName = _configuration.GetConnectionString("ServerName");
            _connectionString = $"Server={ServerName};Initial catalog={dbName};uid={UserId}; pwd={Password};TrustServerCertificate=True;Connection Timeout=120;";
        }
        public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
    }
}
