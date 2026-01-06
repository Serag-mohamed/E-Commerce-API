using E_Commerce.Entities;
using E_Commerce.Repositories;

namespace E_Commerce.Services
{
    public class OrderService
    {
        private readonly IRepository<Order> _repository;

        public OrderService(IRepository<Order> repository)
        {
            _repository = repository;
        }
    }
}
