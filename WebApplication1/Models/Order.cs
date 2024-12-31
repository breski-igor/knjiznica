using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace WebApplication1.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public string Email { get; set; }
        public string Adress { get; set; }
        public string Book_Name { get; set; }
        public string Writer_Name { get; set; }
        public DateOnly Order_Date { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public DateOnly? Date_Sent { get; set; }
        public DateOnly? Date_Returned { get; set; }

        public bool IsDateSentRed { get; set; }

    }
}
