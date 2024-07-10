using Onyx_POS.Data;
using Dapper;
using Onyx_POS.Models;

namespace Onyx_POS.Services
{
    public class LogService(AuthService authService, CommonService commonService, AppDbContext context)
    {
        private readonly LoggedInUserModel _loggedInUser = authService.GetLoggedInUser();
        private readonly ShiftModel _shiftDetail = commonService.GetActiveShiftDetail();
        private readonly AppDbContext _context = context;
        public void PosLog(string logCode, string message)
        {
            string query = @"Insert Into PosLog (LogCode, LogShift, LogDate, LogUser, LogParticulars) Values(@LogCode, @LogShift, @LogDate, @LogUser, @LogParticulars)";
            var parameters = new DynamicParameters();
            parameters.Add("@LogCode", logCode);
            parameters.Add("@LogShift", _shiftDetail.ShiftNo);
            parameters.Add("@LogDate", DateTime.Now);
            parameters.Add("@LogUser", _loggedInUser.U_Code);
            parameters.Add("@LogParticulars", message);
            using var connection = _context.CreateConnection();
            connection.Query(query, parameters);
        }
        public void ShiftDayEndLog(string logCode, string message)
        {
            string query = @"Insert Into PosLogShiftDayEnd (LogCode, LogShift, LogDate, LogUser, LogParticulars) Values(@LogCode, @LogShift, @LogDate, @LogUser, @LogParticulars)";
            var parameters = new DynamicParameters();
            parameters.Add("@LogCode", logCode);
            parameters.Add("@LogShift", _shiftDetail.ShiftNo);
            parameters.Add("@LogDate", DateTime.Now);
            parameters.Add("@LogUser", _loggedInUser.U_Code);
            parameters.Add("@LogParticulars", message);
            using var connection = _context.CreateConnection();
            connection.Query(query, parameters);
        }
    }
}
