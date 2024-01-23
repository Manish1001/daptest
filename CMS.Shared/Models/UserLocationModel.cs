namespace CMS.Shared.Models
{
    using Utils;

    public class UserLocationModel : BaseModel
    {
        public int UserLocationId { get; set; }

        public int UserId { get; set; }

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
