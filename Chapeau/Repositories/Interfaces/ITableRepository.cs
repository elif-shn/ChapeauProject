using Chapeau.Models;
using Chapeau.ViewModels;

namespace Chapeau.Repositories.Interfaces
{
    public interface ITableRepository
    {
        List<Table> GetAllTables();
        List<Table> GetOccupiedTables();

        List<ActiveOrderViewModel> GetActiveOrders(int tableId);

        void MarkOrderAsServed(int orderId);
    }
}
