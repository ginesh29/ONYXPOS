using Dapper;
using Onyx_POS.Data;
using Onyx_POS.Models;
using System.Data.SqlClient;

namespace Onyx_POS.Services
{
    public class CommonService(AppDbContext context)
    {
        private readonly AppDbContext _context = context;
        public string GetRemoteServerName()
        {
            var query = "select Server from PosCtrlConsolidation";
            using var connection = _context.CreateConnection();
            var data = connection.QueryFirstOrDefault<string>(query);
            return data;
        }
        public string GetRemoteConnectionString()
        {
            var ServerName = GetRemoteServerName();
            string UserId = "absluser";
            string Password = "0c4gn2zn";
            string dbName = "POSServer";
            return $"Server={ServerName};Initial catalog={dbName};uid={UserId}; pwd={Password};TrustServerCertificate=True;Connection Timeout=120;";
        }
        public bool CheckDatabaseConnection()
        {
            try
            {
                var _remoteConnectionString = GetRemoteConnectionString();
                using var connection = new SqlConnection(_remoteConnectionString);
                connection.Open();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public PosCtrlModel GetCuurentPosCtrl()
        {
            var query = "select * from PosCtrl";
            using var connection = _context.CreateConnection();
            var data = connection.QueryFirstOrDefault<PosCtrlModel>(query);
            return data;
        }
        public ShiftModel GetActiveShiftDetail()
        {
            var query = "select * from Shift";
            using var connection = _context.CreateConnection();
            var data = connection.QueryFirstOrDefault<ShiftModel>(query);
            return data;
        }
        public ParameterModel GetParameterByType(string type)
        {
            var query = $"select * from PosParameters where Typ = '{type}'";
            using var connection = _context.CreateConnection();
            var data = connection.QueryFirstOrDefault<ParameterModel>(query);
            return data;
        }
        private bool HasTransaction(string status)
        {
            var query = "select count(*) from PosTransHead ";
            if (!string.IsNullOrEmpty(status))
                query += $"where TrnStatus = '{status}'";
            using var connection = _context.CreateConnection();
            var result = connection.QueryFirstOrDefault<int>(query);
            return result > 0;
        }
        private bool HasHoldTransaction(string status)
        {
            var query = "select count(*) from HoldtranHead ";
            if (!string.IsNullOrEmpty(status))
                query += $"where TrnStatus = '{status}'";
            using var connection = _context.CreateConnection();
            var result = connection.QueryFirstOrDefault<int>(query);
            return result > 0;
        }
        private bool HasHoldTransactionRemote(string status)
        {
            var query = "select count(*) from HoldtranHead ";
            if (!string.IsNullOrEmpty(status))
                query += $"where TrnStatus = '{status}'";
            string _remoteConnectionString = GetRemoteConnectionString();
            var connection = new SqlConnection(_remoteConnectionString);
            var result = connection.QueryFirstOrDefault<int>(query);
            return result > 0;
        }
        private int GetCurrentTransactionNo()
        {
            var query = "SELECT MAX(TrnNo) AS TrnNo FROM PosTransHead";
            using var connection = _context.CreateConnection();
            var maxTrnNo = connection.QueryFirstOrDefault<int?>(query);
            return maxTrnNo ?? 1;
        }
        private int GetNextTransactionNo()
        {
            var query = "SELECT MAX(TrnNo) AS TrnNo FROM PosTransHead";
            using var connection = _context.CreateConnection();
            var maxTrnNo = connection.QueryFirstOrDefault<int?>(query);
            return maxTrnNo.HasValue ? maxTrnNo.Value + 1 : 1;
        }
        public int GetTransactionNo()
        {
            bool hasTransaction = HasTransaction(TransStatus.New.GetDisplayName());
            var transNo = hasTransaction ? GetCurrentTransactionNo() : GetNextTransactionNo();
            return transNo;
        }
        private int GetCurrentHoldTransactionNo()
        {
            var query = "SELECT MAX(TrnNo) AS TrnNo FROM HoldTranHead";
            using var connection = _context.CreateConnection();
            var maxTrnNo = connection.QueryFirstOrDefault<int?>(query);
            return maxTrnNo ?? 1;
        }
        private int GetCurrentHoldTransactionNoRemote()
        {
            var query = "SELECT MAX(TrnNo) AS TrnNo FROM HoldTranHead";
            string _remoteConnectionString = GetRemoteConnectionString();
            var connection = new SqlConnection(_remoteConnectionString);
            var maxTrnNo = connection.QueryFirstOrDefault<int?>(query);
            return maxTrnNo ?? 1;
        }
        public int GetHoldTransactionNo()
        {
            bool hasHoldTransaction = HasHoldTransaction(TransStatus.Recalled.GetDisplayName());
            var transNo = hasHoldTransaction ? GetCurrentHoldTransactionNo() : GetNextHoldTransactionNo();
            return transNo;
        }
        private int GetNextHoldTransactionNo()
        {
            var query = "SELECT MAX(TrnNo) AS TrnNo FROM HoldTranHead";
            using var connection = _context.CreateConnection();
            var maxTrnNo = connection.QueryFirstOrDefault<int?>(query);
            return maxTrnNo.HasValue ? maxTrnNo.Value + 1 : 1;
        }
        private int GetNextHoldTransactionNoRemote()
        {
            var query = "SELECT MAX(TrnNo) AS TrnNo FROM HoldTranHead";
            string _remoteConnectionString = GetRemoteConnectionString();
            var connection = new SqlConnection(_remoteConnectionString);
            var maxTrnNo = connection.QueryFirstOrDefault<int?>(query);
            return maxTrnNo.HasValue ? maxTrnNo.Value + 1 : 1;
        }
        public int GetHoldTransactionNoRemote()
        {
            bool hasHoldTransactionRemote = HasHoldTransactionRemote(TransStatus.Recalled.GetDisplayName());
            var transNo = hasHoldTransactionRemote ? GetCurrentHoldTransactionNoRemote() : GetNextHoldTransactionNoRemote();
            return transNo;
        }
        public string GetBillRefNo(int transNo)
        {
            var _posDetail = GetCuurentPosCtrl();
            var dateTime = DateTime.Now.ToString("ddMMyyHHmm");
            string posId = _posDetail.P_PosId.ToString("D2");
            string billNo = Convert.ToString(transNo).PadLeft(11, '0');
            return $"B{_posDetail.P_LocId.Trim()}{posId}{dateTime}{billNo}";
        }
        public string GetHoldCancelledRefNo(int transNo, string type)
        {
            string billNo = Convert.ToString(transNo).PadLeft(10, '0');
            string prefix = type == "HOLD" ? "HB" : "CB";
            return $"{prefix}{billNo}";
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