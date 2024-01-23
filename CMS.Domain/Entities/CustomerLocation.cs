namespace CMS.Domain.Entities
{
    using Common;

    public class CustomerLocation : BaseEntity
    {
        public int CustomerLocationId { get; set; }

        public int CustomerId { get; set; }

        public string LocationName { get; set; }

        public string StreetAddress { get; set; }

        public string AddressLine2 { get; set; }

        public string Landmark { get; set; }

        public string City { get; set; }

        public string District { get; set; }

        public string State { get; set; }

        public string Country { get; set; }

        public string Pincode { get; set; }

        public bool IsHome { get; set; }
    }
}
