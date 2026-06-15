using Chapeau.Models;
using Chapeau.ViewModels;

namespace Chapeau.Repositories.Interfaces
{
    public interface ITableRepository
    {
        List<Table> GetAllTables();



        void UpdateTableStatus(Table table);

        bool HasActiveOrders(int tableId);


    }
}
