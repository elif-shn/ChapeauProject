using Chapeau.Enums;
using Chapeau.Models;
using Chapeau.Repositories.Interfaces;
using Chapeau.Services.Interfaces;

namespace Chapeau.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMenuService _menuService;

        public OrderService(IOrderRepository orderRepository, IMenuService menuService)
        {
            _orderRepository = orderRepository;
            _menuService = menuService;
        }

        public List<Order> GetRunningOrders(bool isFood)
        {
            return _orderRepository.GetRunningOrders(isFood);
        }
        public List<Order> GetRunningTableOrders(int tableId)
        {
            return _orderRepository.GetRunningTableOrders(tableId);
        }

        

        public List<Order> GetFinishedOrders(bool isFood)
        {
            return _orderRepository.GetFinishedOrders(isFood);
        }

        public void UpdateOrderStatus(Order order)
        {
            _orderRepository.UpdateOrderStatus(order);
        }

        public void UpdateOrderItemStatus(OrderItem orderItem)
        {
            _orderRepository.UpdateOrderItemStatus(orderItem);
        }

        public void UpdateCourseStatus(Order order, Category category, OrderItemStatus status)
        {
            _orderRepository.UpdateCourseStatus(order, category, status);
        }

        public Order? GetOrderById(Order order)
        {
            return _orderRepository.GetOrderById(order);
        }

        public Order? GetActiveOrderForTable(int tableId)
        {
            return _orderRepository.GetActiveOrderForTable(tableId);
        }

        public void AddItemToCurrentOrder(List<OrderItem> currentOrder, int menuItemId, string comment = "")
        {
            OrderItem ?existingItem = currentOrder.FirstOrDefault(i => i.MenuItem.MenuItemId == menuItemId &&
            (i.Comment ?? "") == (comment ?? "")
        );
            MenuItem menuItem = _menuService.GetMenuItemById(menuItemId);

            if (existingItem != null)
            {

                if (existingItem.OrderItemQuantity >= menuItem.Stock)
                {
                    throw new Exception("Not enough stock available.");
                }

                existingItem.Increase();
                return;
            }
            
                if(menuItem.Stock <= 0)
                {
                    throw new Exception("Not enough stock available.");
                } 
                currentOrder.Add(new OrderItem
                {
                    MenuItem = menuItem,
                    OrderItemQuantity = 1,
                    Comment = comment ?? "",
                    OrderItemStatus = OrderItemStatus.Ordered
                });           
        }

        public void DecreaseItemQuantityInCurrentOrder(List<OrderItem> currentOrder, int menuItemId, string comment = "")
        {
            OrderItem ?existingItem = currentOrder.FirstOrDefault(i => i.MenuItem.MenuItemId == menuItemId &&
            (i.Comment ?? "") == (comment ?? ""));

            if (existingItem == null)
                return;

            existingItem.Decrease();

            if (existingItem.OrderItemQuantity <= 0)
            {
                currentOrder.Remove(existingItem);
            }
        }

        public void SendOrder(Order newOrder)
        {
            Order? activeOrder = _orderRepository.GetActiveOrderForTable(newOrder.TableId);

            if (activeOrder == null)
            {
                _orderRepository.CreateOrderWithItems(newOrder);
            }
            else
            {
                _orderRepository.AddItemsToExistingOrder(activeOrder, newOrder.OrderItems);
            }
        }
        public void AddNote(List<OrderItem> currentItems, int menuItemId, string comment)
        {
            OrderItem item = currentItems.FirstOrDefault(i => i.MenuItem.MenuItemId == menuItemId);

            if (item != null)
            {
                item.Comment = comment;
            }
        }
        public void DeleteItem(List<OrderItem> currentItems, int menuItemId, string comment = "")
        {
            currentItems?.RemoveAll(item => item.MenuItem.MenuItemId == menuItemId &&
            (item.Comment ?? "") == (comment ?? ""));
        }

        public void MarkOrderAsServed(int orderId)
        {
            _orderRepository.MarkOrderAsServed(orderId);
        }

        public List<Order> GetActiveDrinkOrders(int tableId)
        {
            return _orderRepository.GetActiveDrinkOrders(tableId);
        }
        public List<Order> GetActiveFoodOrders(int tableId)
        {
            return _orderRepository.GetActiveFoodOrders(tableId);
        }


    }
}




    


