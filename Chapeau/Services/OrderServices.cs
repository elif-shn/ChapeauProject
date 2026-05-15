using Chapeau.Repositories;
using Chapeau.ViewModels;

namespace Chapeau.Services
{
        public class OrderService
        {
            private readonly DbOrderRepository _repository;

            public OrderService(DbOrderRepository repository)
            {
                _repository = repository;
            }

            public List<RunningOrderViewModel> GetRunningOrders()
            {
                return _repository.GetRunningOrder();
            }
        }
    }

