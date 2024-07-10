using Onyx_POS.Data;
using Onyx_POS.Models;
using Dapper;
using System.Data;
using NPOI.POIFS.Properties;

namespace Onyx_POS.Services
{
    public class SalesService(AppDbContext context)
    {
        private readonly AppDbContext _context = context;
        public IEnumerable<POSTempItemModel> GetPosTempItems()
        {
            var query = "select TrnNo, TrnSlNo, TrnDept, TrnPlu, TrnType, TrnMode, TrnUser, TrnDt, TrnTime, TrnErrPlu, TrnLoc, TrnDeptPlu, TrnQty, TrnUnit, TrnPackQty, TrnPrice, TrnPrLvl, TrnLDisc, TrnLDiscPercent, TrnTDisc,TrnTDiscType, TrnPosId, TrnShift, TrnNetVal, TrnName,TrnNameAr, TrnDesc, TrnAmt, TrnSalesman, TrnParty, TrnFlag, TrnBarcode FROM PosTemp";
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
            string query = @"insert into PosTemp     (TrnNo,TrnSlNo,TrnDt,TrnDept,TrnPlu,TrnQty,TrnPrice,TrnTDisc,TrnUnit,TrnPackQty,TrnName,TrnNameAr,TrnNetVal,TrnTDiscType,TrnMode,TrnBarcode,TrnUser,TrnShift,TrnAmt,TrnLoc,TrnPosId,TrnSalesman,TrnPrLvl,TrnType,TrnErrPlu,TrnParty,TrnTime)
           values(@TrnNo,@TrnSlNo,@TrnDt,@TrnDept,@TrnPlu,@TrnQty,@TrnPrice,0,@TrnUnit,@TrnPackQty,@TrnName,@TrnNameAr,@TrnNetVal,@TrnTDiscType,@TrnMode,@TrnBarcode,@TrnUser,@TrnShift,@TrnAmt,@TrnLoc,@TrnPosId,@TrnSalesman,'1',@TrnType,'Y','CASH',@TrnTime)";
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
            parameters.Add("@TrnNameAr", model.NameAr);
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
        public void InsertPosTrans(IEnumerable<POSTempItemModel> PosTempItems)
        {
            var query = @"INSERT INTO [dbo].[PosTrans] (TrnNo, TrnSlNo, TrnDept, TrnPlu, TrnType, TrnMode, TrnUser, TrnDt, TrnTime, TrnErrPlu, TrnLoc, TrnDeptPlu, TrnQty, TrnUnit, TrnPackQty, TrnPrice, TrnPrLvl, TrnLDisc, TrnLDiscPercent, TrnTDisc,TrnTDiscType, TrnPosId, TrnShift, TrnNetVal, TrnName,TrnNameAr, TrnDesc, TrnAmt, TrnSalesman, TrnParty, TrnFlag, TrnBarcode)
                            VALUES (@TrnNo, @TrnSlNo, @TrnDept, @TrnPlu, @TrnType, @TrnMode, @TrnUser, @TrnDt, @TrnTime, @TrnErrPlu, @TrnLoc, @TrnDeptPlu, @TrnQty, @TrnUnit, @TrnPackQty, @TrnPrice, @TrnPrLvl, @TrnLDisc, @TrnLDiscPercent, @TrnTDisc,@TrnTDiscType, @TrnPosId, @TrnShift, @TrnNetVal, @TrnName,@TrnNameAr, @TrnDesc, @TrnAmt, @TrnSalesman, @TrnParty, @TrnFlag, @TrnBarcode)";
            using var connection = _context.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();
            try
            {
                connection.Execute(query, PosTempItems, transaction: transaction);
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception("Error inserting data", ex);
            }
        }
        public void InsertPosTransRemote(IEnumerable<POSTempItemModel> PosTempItems)
        {
            var query = @"INSERT INTO [dbo].[PosTrans] (TrnNo, TrnSlNo, TrnDept, TrnPlu, TrnType, TrnMode, TrnUser, TrnDt, TrnTime, TrnErrPlu, TrnLoc, TrnDeptPlu, TrnQty, TrnUnit, TrnPackQty, TrnPrice, TrnPrLvl, TrnLDisc, TrnLDiscPercent, TrnTDisc,TrnTDiscType, TrnPosId, TrnShift, TrnNetVal, TrnName,TrnNameAr, TrnDesc, TrnAmt, TrnSalesman, TrnParty, TrnFlag, TrnBarcode)
                            VALUES (@TrnNo, @TrnSlNo, @TrnDept, @TrnPlu, @TrnType, @TrnMode, @TrnUser, @TrnDt, @TrnTime, @TrnErrPlu, @TrnLoc, @TrnDeptPlu, @TrnQty, @TrnUnit, @TrnPackQty, @TrnPrice, @TrnPrLvl, @TrnLDisc, @TrnLDiscPercent, @TrnTDisc,@TrnTDiscType, @TrnPosId, @TrnShift, @TrnNetVal, @TrnName,@TrnNameAr, @TrnDesc, @TrnAmt, @TrnSalesman, @TrnParty, @TrnFlag, @TrnBarcode)";

            using var connection = _context.CreateRemoteConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();
            try
            {
                connection.Execute(query, PosTempItems, transaction: transaction);
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception("Error inserting data", ex);
            }
        }
        public void UpdatePosHead(PosHead model)
        {
            DeletePosHead(model.TrnNo, model.PosId);
            string query = @"insert into PosTransHead(TrnNo,PosId,TrnUser,TrnShift,TrnStatus,TrnDate,TrnAmt,TrnTotalQty,TrnTotalItems,TrnPayNo,TrnTDisc,TrnComment) values (@TrnNo,@PosId,@TrnUser,@TrnShift,@TrnStatus,@TrnDate,@TrnAmt,@TrnTotalQty,@TrnTotalItems,@TrnPayNo,@TrnTDisc,@TrnComment)";
            var parameters = new DynamicParameters();
            parameters.Add("@TrnNo", model.TrnNo);
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
            connection.Query(query, parameters);
        }
        public void UpdatePosHeadRemote(PosHead model)
        {
            string query = @"insert into PosTransHead(TrnNo,PosId,TrnUser,TrnShift,TrnStatus,TrnDate,TrnAmt,TrnTotalQty,TrnTotalItems,TrnPayNo) values (@TrnNo,@PosId,@TrnUser,@TrnShift,@TrnStatus,@TrnDate,@TrnAmt,@TrnTotalQty,@TrnTotalItems,@TrnPayNo)";
            var parameters = new DynamicParameters();
            parameters.Add("@TrnNo", model.TrnNo);
            parameters.Add("@PosId", model.PosId);
            parameters.Add("@TrnUser", model.User);
            parameters.Add("@TrnShift", model.Shift);
            parameters.Add("@TrnStatus", model.Status);
            parameters.Add("@TrnDate", DateTime.Now.ToString("yyyyMMdd"));
            parameters.Add("@TrnAmt", model.Amt);
            parameters.Add("@TrnPayNo", 0);
            parameters.Add("@TrnTotalQty", model.TotalQty);
            parameters.Add("@TrnTotalItems", model.TotalItems);
            using var connection = _context.CreateRemoteConnection();
            connection.Query(query, parameters);
        }
        public void DeletePosHead(int transNo, int posId)
        {
            string query = $"delete from  PosTransHead  Where TrnNo = {transNo} and  PosId= {posId}";
            using var connection = _context.CreateConnection();
            connection.Query(query);
        }
        public void DeletePosHeadRemote(int transNo, int posId)
        {
            string query = $"delete from  PosTransHead  Where TrnNo = {transNo} and  PosId= {posId}";
            using var connection = _context.CreateRemoteConnection();
            connection.Query(query);
        }
    }
}
