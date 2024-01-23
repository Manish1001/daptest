namespace CMS.Shared.Utils
{
    public class BaseFilter : BasePaginationFilter
    {
        public string IsActive { get; set; }

        public string FromDate { get; set; }

        public string ToDate { get; set; }

        public string SortOrder { get; set; }
    }
}
