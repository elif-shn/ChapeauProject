using Chapeau.Repositories;
using Chapeau.Services;
using Chapeau.ViewModels;
using Microsoft.AspNetCore.Mvc;

public class OrderService : IOrderServices
{
    private readonly IOrderRepository _orderRepository;

    public OrderService(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public List<RunningOrderViewModel> GetRunningOrders()
    {
        return _orderRepository.GetRunningOrder();
    }
}