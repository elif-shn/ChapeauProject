namespace Chapeau.Services
{
    public interface ITakeOrderService
    {
        void AddItemToTable(int tableId, int menuItemId, string comment);
    }
}
