namespace CMS.Domain.Entities
{
    using Common;

    public class LookupType : BaseEntity
    {
        public int LookupTypeId { get; set; }

        public string LookupTypeName { get; set; }

        public string Description { get; set; }
    }
}
