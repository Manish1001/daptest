namespace CMS.Domain.Entities
{
    public class CustomerInvoiceTransaction
    {
        public int CustomerInvoiceTransactionId { get; set; }

        public int CustomerInvoiceId { get; set; }

        public int ProductId { get; set; }

        public short Quantity { get; set; }

        public decimal SubTotal { get; set; }

        public decimal Discount { get; set; }

        public decimal Tax { get; set; }

        public decimal Total { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
