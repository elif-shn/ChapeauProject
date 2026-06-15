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
       

        public void UpdateTableStatus(Table table)
        {
            _tableRepository.UpdateTableStatus(table);
        }

        public bool HasActiveOrders(int tableId)
        {
            return _tableRepository.HasActiveOrders(tableId);
        }

    }
}
