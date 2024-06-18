using Dapper;
using Onyx_POS.Data;
using Onyx_POS.Models;
using System.Data.SqlClient;

namespace Onyx_POS.Services
{
    public class CommonService(AppDbContext context)
    {
        private readonly AppDbContext _context = context;
        public PosCtrlModel GetCuurentPosCtrl()
        {
            var query = "select * from PosCtrl";
            using var connection = _context.CreateConnection();
            var data = connection.QueryFirstOrDefault<PosCtrlModel>(query);
            return data;
        }
        public int GetTransactionNo()
        {
            var query = "select max(TrnNo)[TrnNo] from PosTransHead";
            using var connection = _context.CreateConnection();
            var data = connection.QueryFirstOrDefault<int>(query);
            int result = data > 0 ? data + 1 : 1;
            return result;
        }
        public void GenerateModifiedSp(bool singleFile = true)
        {
            var modifiedSpQuery = @$"SELECT Name, Modify_Date, trim(type)Type FROM sys.objects
	                                WHERE (Type = 'P' or Type = 'TR') and Modify_Date > '2024-06-15'
	                                ORDER BY Modify_Date DESC";
            var connectionString = "Server=GINESH-PC\\SQLEXPRESS;Initial catalog=POS;uid=absluser; pwd=0c4gn2zn;TrustServerCertificate=True;Connection Timeout=120;";
            var connection = new SqlConnection(connectionString);
            var sps = connection.Query<SpModel>(modifiedSpQuery);
            var result = string.Empty;
            string filePath;
            foreach (var item in sps)
            {
                string query = $"SELECT OBJECT_DEFINITION(OBJECT_ID('{item.Name.Trim()}')) AS Definition";
                string storedProcedureText = connection.QueryFirstOrDefault<string>(query);
                if (!string.IsNullOrEmpty(storedProcedureText))
                {
                    storedProcedureText = storedProcedureText.Replace("CREATE", "CREATE OR ALTER", StringComparison.InvariantCultureIgnoreCase);

                    string spText = $"{storedProcedureText.Replace("CREATE OR ALTER table #", "CREATE table #", StringComparison.InvariantCultureIgnoreCase).Replace("CREATE OR ALTER TABLE dbo.#", "CREATE table dbo.#", StringComparison.InvariantCultureIgnoreCase)} \n Go \n";
                    if (singleFile)
                        result += spText;
                    else
                        result = spText;
                }
                if (!singleFile)
                {
                    string type = item.Type == "P" ? "Sp" : "Trigger";
                    filePath = $@"D:\Projects\POS\Onyx_POS\DB\scripts\{type}-Backup-{item.Modify_Date:ddMMyyyy}.sql";
                    File.WriteAllText(filePath, result);
                }
            }
            if (singleFile)
            {
                filePath = $@"D:\Projects\POS\Onyx_POS\DB\scripts\Backup-Single-{DateTime.Now:ddMMyyyy}.sql";
                File.WriteAllText(filePath, result);
            }
        }
    }
}