namespace CMS.Shared.Utils
{
    public class BaseCreatedModel
    {
        public bool IsActive { get; set; }

        public int CreatedBy { get; set; }

        public string CreatedByName { get; set; }

        public DateTime CreatedDate { get; set; }

        public int TotalItems { get; set; }
    }
}
