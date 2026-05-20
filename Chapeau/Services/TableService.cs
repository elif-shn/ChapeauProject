using Chapeau.Models;
using Chapeau.Repositories;

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
    }
}
