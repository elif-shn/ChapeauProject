using Chapeau.Models;

namespace Chapeau.Repositories
{
    public interface ITableRepository
    {
        List<Table> GetAllTables();
        List<Table> GetOccupiedTables();
    }
}
