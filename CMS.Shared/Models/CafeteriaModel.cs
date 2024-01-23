namespace CMS.Shared.Models
{
    using Utils;

    public class CafeteriaModel : BaseModifiedModel
    {
        public int CafeteriaId { get; set; }

        public string CafeteriaName { get; set; }

        public string LogoPath { get; set; }

        public string About { get; set; }

        public string SocialLinks { get; set; }

        public string WebsiteUrl { get; set; }

        public string Email { get; set; }

        public string Mobile { get; set; }

        public string Landline { get; set; }

        public string Extension { get; set; }

        public string HeadName { get; set; }

        public string HeadEmail { get; set; }

        public string HeadMobile { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
