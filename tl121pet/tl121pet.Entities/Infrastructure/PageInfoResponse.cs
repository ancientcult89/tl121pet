namespace tl121pet.Entities.Infrastructure
{
    public class PageInfoResponse
    {
        public int TotalItems { get; set; }
        public int ItemsPerPage { get; set; }
        public int CurrentPage { get; set; }

        public int TotalPages => (int)Math.Ceiling((decimal)TotalItems / (ItemsPerPage > 0 ? ItemsPerPage : 1));
    }
}
