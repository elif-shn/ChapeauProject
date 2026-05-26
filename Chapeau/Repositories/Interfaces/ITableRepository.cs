using Chapeau.Models;

namespace Chapeau.Repositories.Interfaces
{
    public interface ITableRepository
    {
        List<Table> GetAllTables();
        List<Table> GetOccupiedTables();
    }
}
