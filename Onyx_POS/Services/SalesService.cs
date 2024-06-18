using Dapper;
using Onyx_POS.Data;
using Onyx_POS.Models;

namespace Onyx_POS.Services
{
    public class SalesService(AppDbContext context)
    {
        private readonly AppDbContext _context = context;
        public IEnumerable<ItemModel> GetItemFromPLU(string code)
        {
            var query = $"Select PluCode, PluName, PluNameAr, PluDept, PluUom, PluPrice, PluBarCode, PluVendCode, Scalleable,1 as PackQty from PluMaster where PluBarCode='{code}' or PluCode='{code}'";
            using var connection = _context.CreateConnection();
            var data = connection.Query<ItemModel>(query);
            return data;
        }
    }
}
