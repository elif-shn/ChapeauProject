using Chapeau.Models;
using Chapeau.Repositories.Interfaces;
using Chapeau.Services.Interfaces;

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
        
    }
}
