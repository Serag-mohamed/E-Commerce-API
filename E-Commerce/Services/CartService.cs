using E_Commerce.Entities;
using E_Commerce.Repositories;

namespace E_Commerce.Services
{
    public class CartService
    {
        private readonly IRepository<Cart> _repository;
        public CartService(IRepository<Cart> repository)
        {
            _repository = repository;
        }


    }
}
