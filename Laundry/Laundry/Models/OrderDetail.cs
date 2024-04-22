
namespace Laundry.Models
{
    public enum OrderStatus
    {
        WaitingForDelivery,
        Processing,
        ShippedToYou
    }

    public class OrderDetail
    {
        public int Id { get; set; }
        public string OrderCode { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string AppointmentDate { get; set; }
        public string Product { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.WaitingForDelivery;

    //    public static implicit operator OrderDetail(OrderDetail v)
    //    {
    //        throw new NotImplementedException();
    //    }
    }
}