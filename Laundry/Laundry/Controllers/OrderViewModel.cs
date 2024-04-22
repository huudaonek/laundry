using Laundry.Models;

namespace Laundry.Controllers
{
    public class OrderViewModel
    {
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string AppointmentDate { get; set; }
        public List<LaundryItem> LaundryItems { get; set; }
        public IEnumerable<int> SelectedProducts { get; set; } 
        public Dictionary<int, int> Quantities { get; set; }
        public decimal TotalPrice { get; set; }


    }

}