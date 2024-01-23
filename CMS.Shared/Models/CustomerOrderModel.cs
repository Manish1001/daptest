namespace CMS.Shared.Models
{
    public class CustomerOrderModel
    {
        public int CustomerOrderId { get; set; }

        public int MealId { get; set; }

        public int CustomerId { get; set; }

        public DateTime OrderDate { get; set; }

        public short Quantity { get; set; }

        public decimal SubTotal { get; set; }

        public decimal Discount { get; set; }

        public decimal Tax { get; set; }

        public decimal OrderTotal { get; set; }

        public int OrderStatusId { get; set; }

        public string OrderNo { get; set; }

        public bool IsDelivered { get; set; }

        public int? DeliveredBy { get; set; }

        public DateTime? DeliveryDate { get; set; }

        public string OrderComments { get; set; }

        public string DeliveryComments { get; set; }

        public bool IsPaid { get; set; }

        public DateTime? PaymentDate { get; set; }

        public int TotalItems { get; set; }
    }
}
