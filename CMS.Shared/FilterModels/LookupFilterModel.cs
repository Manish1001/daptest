namespace CMS.Shared.FilterModels
{
    using Utils;

    public class LookupFilterModel : BaseFilter
    {
        public int LookupId { get; set; }

        public int? LookupTypeId { get; set; }

        public string LookupCode { get; set; }

        public string LookupName { get; set; }

        public string LookupValue { get; set; }
    }
}
