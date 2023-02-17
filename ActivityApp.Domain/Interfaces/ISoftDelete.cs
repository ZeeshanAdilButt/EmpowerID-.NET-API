namespace ActivityApp.Domain.Interfaces
{
    public interface ISoftDelete
    {
        public bool IsActive { get; set; }

    }
}