namespace CMS.Domain.Entities
{
    using Common;

    public class Product : BaseEntity
    {
        public int ProductId { get; set; }

        public string ProductCode { get; set; }

        public int CafeteriaLocationId { get; set; }

        public int CategoryId { get; set; }

        public string ProductName { get; set; }

        public string Description { get; set; }

        public int UnitId { get; set; }

        public decimal Quantity { get; set; }

        public decimal RawQuantity { get; set; }

        public decimal? VIPStaffPrice { get; set; }

        public decimal? StaffPrice { get; set; }

        public decimal Price { get; set; }
    }
}
