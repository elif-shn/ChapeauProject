using Chapeau.Models;
using Chapeau.ViewModels;
using System.Security.Cryptography;


namespace Chapeau.Services.Interfaces
{
    public interface ITablesService
    {
        List<Table> GetAllTables();


        void UpdateTableStatus(Table table);

        bool HasActiveOrders(int tableId);

        
    }
}
