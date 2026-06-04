using Chapeau.Models;
using Chapeau.Repositories.Interfaces;
using Chapeau.Services.Interfaces;
using Chapeau.ViewModels;

namespace Chapeau.Services
{
    public class TableService : ITablesService
    {
        private ITableRepository _tableRepository;

        public TableService(ITableRepository tableRepository)
        {
            this._tableRepository = tableRepository;
        }

        public List <Table> GetAllTables()
        {
            return _tableRepository.GetAllTables();
        }
        public List<Table> GetOccupiedTables()
        {
            return _tableRepository.GetOccupiedTables();
        }

        public List<ActiveOrderViewModel> GetActiveOrders(int tableId)
        {
            return _tableRepository.GetActiveOrders(tableId);
        }

        public void MarkOrderAsServed(int orderId)
        {
            _tableRepository.MarkOrderAsServed(orderId);
        }
    }
}
