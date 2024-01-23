namespace CMS.Shared.Models
{
    using Utils;

    public class CafeteriaLocationModel : BaseModel
    {
        public int CafeteriaLocationId { get; set; }

        public int CafeteriaId { get; set; }

        public string LocationName { get; set; }

        public string StreetAddress { get; set; }

        public string AddressLine2 { get; set; }

        public string Landmark { get; set; }

        public string City { get; set; }

        public string District { get; set; }

        public string State { get; set; }

        public string Country { get; set; }

        public string Pincode { get; set; }

        public string Email { get; set; }

        public string Mobile { get; set; }

        public string Landline { get; set; }

        public string Extension { get; set; }

        public string HeadName { get; set; }

        public string HeadEmail { get; set; }

        public string HeadMobile { get; set; }
    }
}
