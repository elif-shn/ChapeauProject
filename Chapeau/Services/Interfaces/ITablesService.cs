using Chapeau.Models;
using Chapeau.ViewModels;
using System.Security.Cryptography;


namespace Chapeau.Services.Interfaces
{
    public interface ITablesService
    {
        List<Table> GetAllTables();
        List<Table> GetOccupiedTables();

        List<ActiveOrderViewModel> GetActiveOrders(int tableId);

        void MarkOrderAsServed(int orderId);
    }
}
