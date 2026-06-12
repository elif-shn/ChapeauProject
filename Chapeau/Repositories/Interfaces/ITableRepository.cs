using Chapeau.Models;
using Chapeau.ViewModels;

namespace Chapeau.Repositories.Interfaces
{
    public interface ITableRepository
    {
        List<Table> GetAllTables();
        List<ActiveOrderViewModel> GetActiveOrders(int tableId);

        void MarkOrderAsServed(int orderId);

        void UpdateTableStatus(Table table);

        bool HasActiveOrders(int tableId);
    }
}
