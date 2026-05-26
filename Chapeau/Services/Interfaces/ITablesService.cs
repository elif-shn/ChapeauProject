using Chapeau.Models;


namespace Chapeau.Services.Interfaces
{
    public interface ITablesService
    {
        List<Table> GetAllTables();
        List<Table> GetOccupiedTables();
    }
}
