using Onyx_POS.Data;
using Onyx_POS.Models;
using Dapper;
using System.Data;
using System.Data.SqlClient;

namespace Onyx_POS.Services
{
    public class SalesService(AppDbContext context, CommonService commonService)
    {
        private readonly AppDbContext _context = context;
        private readonly string _remoteConnectionString = commonService.GetRemoteConnectionString();
        public IEnumerable<POSTempItemModel> GetPosTempItems()
        {
            var query = "select TrnNo, TrnSlNo, TrnDept, TrnPlu, TrnType, TrnMode, TrnUser, TrnDt, TrnTime, TrnErrPlu, TrnLoc, TrnDeptPlu, TrnQty, TrnUnit, TrnPackQty, TrnPrice, TrnPrLvl, TrnLDisc, TrnLDiscPercent, TrnTDisc,TrnTDiscType, TrnPosId, TrnShift, TrnNetVal, TrnName, TrnDesc, TrnAmt, TrnSalesman, TrnParty, TrnFlag, TrnBarcode FROM PosTemp";
            using var connection = _context.CreateConnection();
            var data = connection.Query<POSTempItemModel>(query);
            return data;
        }
        public IEnumerable<POSTempItemModel> ClearPosTempItems(int transNo)
        {
            var query = $"delete from  PosTemp  Where TrnNo = {transNo}";
            using var connection = _context.CreateConnection();
            var data = connection.Query<POSTempItemModel>(query);
            return data;
        }
        public IEnumerable<ItemPriceCheckModel> GetPriceCheckItems(string code)
        {
            var procedureName = "spPrice_Check";
            var parameters = new DynamicParameters();
            parameters.Add("v_ItemCode", code);
            using var connection = _context.CreateConnection();
            var data = connection.Query<ItemPriceCheckModel>
                (procedureName, parameters, commandType: CommandType.StoredProcedure);
            return data;
        }
        public PluItemModel PluFind(string barcode)
        {
            var procedureName = "PluFind";
            var parameters = new DynamicParameters();
            parameters.Add("v_Cd", barcode);
            using var connection = _context.CreateConnection();
            var data = connection.QueryFirstOrDefault<PluItemModel>
                (procedureName, parameters, commandType: CommandType.StoredProcedure);
            return data;
        }
        public void InsertItem(SaleItemModel model)
        {
            string query = @"insert into PosTemp     (TrnNo,TrnSlNo,TrnDt,TrnDept,TrnPlu,TrnQty,TrnPrice,TrnTDisc,TrnUnit,TrnPackQty,TrnName,TrnNetVal,TrnTDiscType,TrnMode,TrnBarcode,TrnUser,TrnShift,TrnAmt,TrnLoc,TrnPosId,TrnSalesman,TrnPrLvl,TrnType,TrnErrPlu,TrnParty,TrnTime)
           values(@TrnNo,@TrnSlNo,@TrnDt,@TrnDept,@TrnPlu,@TrnQty,@TrnPrice,0,@TrnUnit,@TrnPackQty,@TrnName,@TrnNetVal,@TrnTDiscType,@TrnMode,@TrnBarcode,@TrnUser,@TrnShift,@TrnAmt,@TrnLoc,@TrnPosId,@TrnSalesman,'1',@TrnType,'Y','CASH',@TrnTime)";
            var parameters = new DynamicParameters();
            parameters.Add("@TrnNo", model.TrnNo);
            parameters.Add("@TrnSlNo", model.SrNo);
            parameters.Add("@TrnDt", DateTime.Now.ToString("yyyyMMdd"));
            parameters.Add("@TrnDept", model.Dept);
            parameters.Add("@TrnPlu", model.Plu);
            parameters.Add("@TrnQty", model.Qty);
            parameters.Add("@TrnPrice", model.Rate);
            parameters.Add("@TrnUnit", model.Unit);
            parameters.Add("@TrnPackQty", model.PackQty);
            parameters.Add("@TrnName", model.Name);
            parameters.Add("@TrnNetVal", (model.Rate * model.Qty) - model.TaxAmt);
            parameters.Add("@TrnTDiscType", model.TrnDiscType);
            parameters.Add("@TrnMode", model.TrnMode);
            parameters.Add("@TrnType", model.TrnType);
            parameters.Add("@TrnBarcode", model.Barcode);
            parameters.Add("@TrnShift", model.ShiftNo);
            parameters.Add("@TrnAmt", Math.Round(model.TaxAmt, 2).ToString("0.00"));
            parameters.Add("@TrnUser", model.User);
            parameters.Add("@TrnSalesman", string.Empty);
            parameters.Add("@TrnLoc", model.LocId);
            parameters.Add("@TrnPosId", model.POSId);
            parameters.Add("@TrnTime", DateTime.Now.ToString("HH:mm"));
            using var connection = _context.CreateConnection();
            connection.Query(query, parameters);
        }
        public void UpsertPosTransHead(PosHead model)
        {
            string checkQuery = "SELECT COUNT(1) FROM PosTransHead WHERE TrnNo = @TrnNo";
            string insertQuery = @"INSERT INTO PosTransHead(TrnNo, BillRefNo, PosId, TrnUser, TrnShift, TrnStatus, TrnDate, TrnAmt, TrnTotalQty, TrnTotalItems, TrnPayNo, TrnTDisc, TrnComment) 
                           VALUES (@TrnNo, @BillRefNo, @PosId, @TrnUser, @TrnShift, @TrnStatus, @TrnDate, @TrnAmt, @TrnTotalQty, @TrnTotalItems, @TrnPayNo, @TrnTDisc, @TrnComment)";
            string updateQuery = @"UPDATE PosTransHead 
                           SET BillRefNo = @BillRefNo, PosId = @PosId, TrnUser = @TrnUser, TrnShift = @TrnShift, TrnStatus = @TrnStatus, TrnDate = @TrnDate, TrnAmt = @TrnAmt, TrnTotalQty = @TrnTotalQty, 
                               TrnTotalItems = @TrnTotalItems, TrnPayNo = @TrnPayNo, TrnTDisc = @TrnTDisc, TrnComment = @TrnComment 
                           WHERE TrnNo = @TrnNo";

            var parameters = new DynamicParameters();
            parameters.Add("@TrnNo", model.TrnNo);
            parameters.Add("@BillRefNo", model.BillRefNo);
            parameters.Add("@PosId", model.PosId);
            parameters.Add("@TrnUser", model.User);
            parameters.Add("@TrnShift", model.Shift);
            parameters.Add("@TrnStatus", model.Status);
            parameters.Add("@TrnDate", DateTime.Now.ToString("yyyyMMdd"));
            parameters.Add("@TrnAmt", model.Amt);
            parameters.Add("@TrnPayNo", 0);
            parameters.Add("@TrnTotalQty", model.TotalQty);
            parameters.Add("@TrnTotalItems", model.TotalItems);
            parameters.Add("@TrnTDisc", 0);
            parameters.Add("@TrnComment", string.Empty);

            using var connection = _context.CreateConnection();
            var exists = connection.ExecuteScalar<bool>(checkQuery, new { model.TrnNo });
            if (exists)
                connection.Execute(updateQuery, parameters);
            else
                connection.Execute(insertQuery, parameters);
        }
        public void InsertHoldTransDetails(IEnumerable<POSTempItemModel> posTempItems)
        {
            DeleteHoldTransDetails(posTempItems.FirstOrDefault().TrnNo);
            var query = @"INSERT INTO [dbo].[HoldtranDetail] ([HBillRefNo], [TrnNo], [TrnSlNo], [TrnDt], [TrnDept], [TrnPlu], [TrnQty], [TrnPrice], [TrnUnit], [TrnPackQty], [TrnPrLvl], [TrnLDisc], [TrnTDisc], [TrnLDiscPercent], [TrnTDiscType], [TrnMode], [TrnType], [TrnDeptPlu], [TrnNetVal], [TrnUser], [TrnTime], [TrnErrPlu], [TrnLoc], [TrnPosId], [TrnShift], [TrnAmt], [TrnParty], [TrnSalesman], [TrnDesc], [TrnFlag], [TrnName], [TrnBarcode]) 
                VALUES (@HBillRefNo, @TrnNo, @TrnSlNo, @TrnDt, @TrnDept, @TrnPlu, @TrnQty, @TrnPrice, @TrnUnit, @TrnPackQty, @TrnPrLvl, @TrnLDisc, @TrnTDisc, @TrnLDiscPercent, @TrnTDiscType, @TrnMode, @TrnType, @TrnDeptPlu, @TrnNetVal, @TrnUser, @TrnTime, @TrnErrPlu, @TrnLoc, @TrnPosId, @TrnShift, @TrnAmt, @TrnParty, @TrnSalesman, @TrnDesc, @TrnFlag, @TrnName, @TrnBarcode)";

            using var connection = _context.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();
            try
            {
                connection.Execute(query, posTempItems, transaction: transaction);
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception("Error inserting data", ex);
            }
        }
        public void InsertHoldTransDetailsRemote(IEnumerable<POSTempItemModel> posTempItems)
        {
            DeleteHoldTransDetailsRemote(posTempItems.FirstOrDefault().TrnNo);
            var query = @"INSERT INTO [dbo].[HoldtranDetail] ([HBillRefNo], [TrnNo], [TrnSlNo], [TrnDt], [TrnDept], [TrnPlu], [TrnQty], [TrnPrice], [TrnUnit], [TrnPackQty], [TrnPrLvl], [TrnLDisc], [TrnTDisc], [TrnLDiscPercent], [TrnTDiscType], [TrnMode], [TrnType], [TrnDeptPlu], [TrnNetVal], [TrnUser], [TrnTime], [TrnErrPlu], [TrnLoc], [TrnPosId], [TrnShift], [TrnAmt], [TrnParty], [TrnSalesman], [TrnDesc], [TrnFlag], [TrnName], [TrnBarcode]) 
                VALUES (@HBillRefNo, @TrnNo, @TrnSlNo, @TrnDt, @TrnDept, @TrnPlu, @TrnQty, @TrnPrice, @TrnUnit, @TrnPackQty, @TrnPrLvl, @TrnLDisc, @TrnTDisc, @TrnLDiscPercent, @TrnTDiscType, @TrnMode, @TrnType, @TrnDeptPlu, @TrnNetVal, @TrnUser, @TrnTime, @TrnErrPlu, @TrnLoc, @TrnPosId, @TrnShift, @TrnAmt, @TrnParty, @TrnSalesman, @TrnDesc, @TrnFlag, @TrnName, @TrnBarcode)";

            var connection = new SqlConnection(_remoteConnectionString);
            connection.Open();
            using var transaction = connection.BeginTransaction();
            try
            {
                connection.Execute(query, posTempItems, transaction: transaction);
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception("Error inserting data", ex);
            }
        }
        public void UpsertHoldTransHead(HoldTransHead model)
        {
            string checkQuery = "SELECT COUNT(1) FROM [dbo].[HoldtranHead] WHERE TrnNo = @TrnNo";
            string insertQuery = "INSERT INTO [dbo].[HoldtranHead] ([HBillRefNo], [TrnNo], [PosId], [TrnStatus], [TrnDate], [TrnAmt], [TrnTotalQty], [TrnTotalItems], [TrnPayNo], [TrnComment], [TrnUser], [TrnShift], [TrnTDisc], [TrnLoc], [RPosId], [RTrnno], [RTrnUser], [RTrnDate]) VALUES (@HBillRefNo, @TrnNo, @PosId, @TrnStatus, @TrnDate, @TrnAmt, @TrnTotalQty, @TrnTotalItems, @TrnPayNo, @TrnComment, @TrnUser, @TrnShift, @TrnTDisc, @TrnLoc, @RPosId, @RTrnno, @RTrnUser, @RTrnDate)";
            string updateQuery = "UPDATE [dbo].[HoldtranHead] SET TrnNo = @TrnNo, PosId = @PosId, TrnStatus = @TrnStatus, TrnDate = @TrnDate, TrnAmt = @TrnAmt, TrnTotalQty = @TrnTotalQty, TrnTotalItems = @TrnTotalItems, TrnPayNo = @TrnPayNo, TrnComment = @TrnComment, TrnUser = @TrnUser, TrnShift = @TrnShift, TrnTDisc = @TrnTDisc, TrnLoc = @TrnLoc, RPosId = @RPosId, RTrnno = @RTrnno, RTrnUser = @RTrnUser, RTrnDate = @RTrnDate WHERE TrnNo = @TrnNo";

            var parameters = new DynamicParameters();
            parameters.Add("@HBillRefNo", model.HBillRefNo);
            parameters.Add("@TrnNo", model.TrnNo);
            parameters.Add("@PosId", model.PosId);
            parameters.Add("@TrnStatus", model.Status);
            parameters.Add("@TrnDate", model.TrnDate);
            parameters.Add("@TrnAmt", model.Amt);
            parameters.Add("@TrnTotalQty", model.TotalQty);
            parameters.Add("@TrnTotalItems", model.TotalItems);
            parameters.Add("@TrnPayNo", model.TrnPayNo);
            parameters.Add("@TrnComment", null);
            parameters.Add("@TrnUser", model.User);
            parameters.Add("@TrnShift", model.Shift);
            parameters.Add("@TrnTDisc", model.Discount);
            parameters.Add("@TrnLoc", model.LocId);
            parameters.Add("@RPosId", model.RPosId);
            parameters.Add("@RTrnNo", model.RTrnNo);
            parameters.Add("@RTrnUser", model.RTrnUser);
            parameters.Add("@RTrnDate", model.RTrnDate);
            using var connection = _context.CreateConnection();
            var exists = connection.ExecuteScalar<bool>(checkQuery, new { model.TrnNo });
            if (exists)
                connection.Execute(updateQuery, parameters);
            else
                connection.Execute(insertQuery, parameters);
        }
        public void UpsertHoldTransHeadRemote(HoldTransHead model)
        {
            string checkQuery = "SELECT COUNT(1) FROM [dbo].[HoldtranHead] WHERE TrnNo = @TrnNo";
            string insertQuery = "INSERT INTO [dbo].[HoldtranHead] ([HBillRefNo], [TrnNo], [PosId], [TrnStatus], [TrnDate], [TrnAmt], [TrnTotalQty], [TrnTotalItems], [TrnPayNo], [TrnComment], [TrnUser], [TrnShift], [TrnTDisc], [TrnLoc], [RPosId], [RTrnno], [RTrnUser], [RTrnDate]) VALUES (@HBillRefNo, @TrnNo, @PosId, @TrnStatus, @TrnDate, @TrnAmt, @TrnTotalQty, @TrnTotalItems, @TrnPayNo, @TrnComment, @TrnUser, @TrnShift, @TrnTDisc, @TrnLoc, @RPosId, @RTrnno, @RTrnUser, @RTrnDate)";
            string updateQuery = "UPDATE [dbo].[HoldtranHead] SET TrnNo = @TrnNo, PosId = @PosId, TrnStatus = @TrnStatus, TrnDate = @TrnDate, TrnAmt = @TrnAmt, TrnTotalQty = @TrnTotalQty, TrnTotalItems = @TrnTotalItems, TrnPayNo = @TrnPayNo, TrnComment = @TrnComment, TrnUser = @TrnUser, TrnShift = @TrnShift, TrnTDisc = @TrnTDisc, TrnLoc = @TrnLoc, RPosId = @RPosId, RTrnno = @RTrnno, RTrnUser = @RTrnUser, RTrnDate = @RTrnDate WHERE TrnNo = @TrnNo";

            var parameters = new DynamicParameters();
            parameters.Add("@HBillRefNo", model.HBillRefNo);
            parameters.Add("@TrnNo", model.TrnNo);
            parameters.Add("@PosId", model.PosId);
            parameters.Add("@TrnStatus", model.Status);
            parameters.Add("@TrnDate", model.TrnDate);
            parameters.Add("@TrnAmt", model.Amt);
            parameters.Add("@TrnTotalQty", model.TotalQty);
            parameters.Add("@TrnTotalItems", model.TotalItems);
            parameters.Add("@TrnPayNo", model.TrnPayNo);
            parameters.Add("@TrnComment", null);
            parameters.Add("@TrnUser", model.User);
            parameters.Add("@TrnShift", model.Shift);
            parameters.Add("@TrnTDisc", model.Discount);
            parameters.Add("@TrnLoc", model.LocId);
            parameters.Add("@RPosId", model.RPosId);
            parameters.Add("@RTrnNo", model.RTrnNo);
            parameters.Add("@RTrnUser", model.RTrnUser);
            parameters.Add("@RTrnDate", model.RTrnDate);
            var connection = new SqlConnection(_remoteConnectionString);
            var exists = connection.ExecuteScalar<bool>(checkQuery, new { model.TrnNo });
            if (exists)
                connection.Execute(updateQuery, parameters);
            else
                connection.Execute(insertQuery, parameters);
        }
        public IEnumerable<HoldTransHeadViewModel> GetHoldTransHeads()
        {
            var query = "select * FROM HoldTranHead where TrnStatus='Hold'";
            using var connection = _context.CreateConnection();
            var data = connection.Query<HoldTransHeadViewModel>(query);
            return data;
        }
        public IEnumerable<HoldTransHeadViewModel> GetHoldTransHeadsRemote()
        {
            var query = "select * FROM HoldTranHead where TrnStatus='Hold'";
            var connection = new SqlConnection(_remoteConnectionString);
            var data = connection.Query<HoldTransHeadViewModel>(query);
            return data;
        }
        public void DeleteHoldTransDetails(int transNo)
        {
            var query = $"delete FROM HoldTranDetail where TrnNo = {transNo}";
            using var connection = _context.CreateConnection();
            connection.Query(query);
        }
        public IEnumerable<POSTempItemModel> GetHoldTransDetails(int transNo)
        {            
            var query = $"select * FROM HoldTranDetail where TrnNo = {transNo}";
            using var connection = _context.CreateConnection();
            var data = connection.Query<POSTempItemModel>(query);
            return data;
        }
        public void DeleteHoldTransDetailsRemote(int transNo)
        {            
            var query = $"delete FROM HoldTranDetail where TrnNo = {transNo}";
            var connection = new SqlConnection(_remoteConnectionString);
            connection.Query(query);
        }
        public IEnumerable<POSTempItemModel> GetHoldTransDetailsRemote(int transNo)
        {
            var query = $"select * FROM HoldTranDetail where TrnNo = {transNo}";
            var connection = new SqlConnection(_remoteConnectionString);
            var data = connection.Query<POSTempItemModel>(query);
            return data;
        }
        public void InsertPosTempItems(IEnumerable<POSTempItemModel> posTempItems)
        {
            string query = @"insert into PosTemp     (TrnNo,TrnSlNo,TrnDt,TrnDept,TrnPlu,TrnQty,TrnPrice,TrnTDisc,TrnUnit,TrnPackQty,TrnName,TrnNetVal,TrnTDiscType,TrnMode,TrnBarcode,TrnUser,TrnShift,TrnAmt,TrnLoc,TrnPosId,TrnSalesman,TrnPrLvl,TrnType,TrnErrPlu,TrnParty,TrnTime)
           values(@TrnNo,@TrnSlNo,@TrnDt,@TrnDept,@TrnPlu,@TrnQty,@TrnPrice,0,@TrnUnit,@TrnPackQty,@TrnName,@TrnNetVal,@TrnTDiscType,@TrnMode,@TrnBarcode,@TrnUser,@TrnShift,@TrnAmt,@TrnLoc,@TrnPosId,@TrnSalesman,'1',@TrnType,'Y','CASH',@TrnTime)";
            using var connection = _context.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();
            try
            {
                connection.Execute(query, posTempItems, transaction: transaction);
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception("Error inserting data", ex);
            }
        }
    }
}
