using Chapeau.Models;
using Chapeau.ViewModels;
using System.Security.Cryptography;


namespace Chapeau.Services.Interfaces
{
    public interface ITablesService
    {
        List<Table> GetAllTables();

        List<ActiveOrderViewModel> GetActiveOrders(int tableId);

        void MarkOrderAsServed(int orderId);

        void UpdateTableStatus(Table table);

        bool HasActiveOrders(int tableId);

        List<Order> GetRunningTableOrders(int tableId);
    }
}
