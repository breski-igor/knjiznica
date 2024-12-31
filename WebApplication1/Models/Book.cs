namespace WebApplication1.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }
        public string? Availability { get; set; }
        public int Quantity { get; set; }
        public ICollection<Review> Reviews { get; set; } = new List<Review>();

    }
}
