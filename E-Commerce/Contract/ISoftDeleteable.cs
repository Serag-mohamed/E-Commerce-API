namespace E_Commerce.Contract
{
    public interface ISoftDeleteable
    {
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        public void Delete();
        public void UndoDelete();
        
    }
}
