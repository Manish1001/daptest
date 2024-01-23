namespace CMS.Shared.Models
{
    using Utils;

    public class LookupModel : BaseModel
    {
        public int LookupId { get; set; }

        public int LookupTypeId { get; set; }

        public string LookupName { get; set; }

        public string LookupCode { get; set; }

        public string LookupValue { get; set; }

        public string Description { get; set; }
    }
}
