namespace CMS.Shared.Utils
{
    public class BaseModel
    {
        public bool IsActive { get; set; }

        public int? CreatedBy { get; set; }

        public string CreatedByName { get; set; }

        public DateTime CreatedDate { get; set; }

        public int? ModifiedBy { get; set; }

        public string ModifiedByName { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public DateTime LastModifiedDate { get; set; }

        public int TotalItems { get; set; }
    }
}
