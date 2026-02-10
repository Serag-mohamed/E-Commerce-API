using E_Commerce.Contract;

namespace E_Commerce.Entities
{
    public class Category : ISoftDeleteable
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string Name { get; set; } = null!;
        public Guid? ParentCategoryId { get; set; }

        public Category? ParentCategory { get; set; }
        public ICollection<Category> Categories { get; set; } = [];
        public ICollection<Product> Products { get; set; } = [];
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }

        public void Delete()
        {
            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;
        }

        public void UndoDelete()
        {
            IsDeleted = false;
            DeletedAt = null;
        }
    }
}

