namespace WebApplication1.Models
{
    public class Member
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public double? Amount { get; set; } = 0.00;
    }
}
