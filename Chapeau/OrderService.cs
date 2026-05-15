// Ensure OrderService implements IOrderService
public class OrderService : IOrderService
{
    private readonly DbOrderRepository _repository;

    public OrderService(DbOrderRepository repository)
    {
        _repository = repository;
    }

    public List<RunningOrderViewModel> GetRunningOrders()
    {
        return _repository.GetRunningOrders();
    }
}