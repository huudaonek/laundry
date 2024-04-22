namespace Laundry.Models
{
    public class Promotion
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public PromotionPackage Package { get; set; } 
    }

    public enum PromotionPackage
    {
        Standard,
        Premium,
        VIP,
        Family,
        Referral
    }

}
