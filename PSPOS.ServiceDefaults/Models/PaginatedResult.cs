namespace PSPOS.ServiceDefaults.Models
{
    public class PaginatedResult<T>
    {
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public List<T>? Items { get; set; }
    }
}
