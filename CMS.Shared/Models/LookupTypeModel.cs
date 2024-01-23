namespace CMS.Shared.Models
{
    using Utils;

    public class LookupTypeModel : BaseModel
    {
        public int LookupTypeId { get; set; }

        public string LookupTypeName { get; set; }

        public string Description { get; set; }
    }
}
