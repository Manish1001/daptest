namespace CMS.Shared.Models
{
    public class CustomerInvoiceModel
    {
        public int CustomerInvoiceId { get; set; }

        public int CustomerOrderId { get; set; }

        public int CustomerId { get; set; }

        public DateTime InvoiceDate { get; set; }

        public short Quantity { get; set; }

        public decimal SubTotal { get; set; }

        public decimal Discount { get; set; }

        public decimal Tax { get; set; }

        public decimal InvoiceTotal { get; set; }

        public bool IsActive { get; set; }

        public string InvoiceNo { get; set; }

        public int CreatedBy { get; set; }

        public int TotalItems { get; set; }
    }
}
