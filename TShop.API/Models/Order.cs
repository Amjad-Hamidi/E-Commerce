namespace TShop.API.Models
{
    public enum OrderStatus
    {
        Pending, // order يضيف client اول ما ال
        Cancelled, // لغاه client كنسله او ال Admin ال
        Approved, // بعدها بوافق عليهن orders بشوف ال Admin ال
        Shipped, // (في حالة الشحن (بعد الموافقة
        Completed, // client وصل عند ال
    }
    public enum PaymentMethodType
    {
        Visa,Cash
    }
    public class Order
    {
        // order
        public int Id { get; set; }
        public OrderStatus  OrderStatus { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime ShippedDate { get; set; }
        public decimal TotalPrice { get; set; } // الواحد order لل
        // payment (اذا كان كاش , لا يوجد سشن اي دي ولا ترانزاكشن اي دي)
        public PaymentMethodType PaymentMethodType {  get; set; }
        public string? SessionId { get; set; } // رقم عملية الدفع , سواء نجحت او فشلت
        public string? TransactionId { get; set; } // فقط اذا تمت العملية بنجاح      
        // carrier (معلومات شركة الشحن)
        public string? Carrier {  get; set; } // order اسم شركة التوصيل الي اخذت هاد ال
        public string? TrackingNumber { get; set; } // shipped في حالة ال order رقم التتبع بكون جاي من الشركة لما يكون ال      
        // relation (Navigation Property)
        public ApplicationUser ApplicationUser { get; set; }
        public string ApplicationUserId { get; set; }
    }
}
