namespace CMS.Shared.Utils
{
    public class BaseModifiedModel
    {
        public bool IsActive { get; set; }

        public int? ModifiedBy { get; set; }

        public string ModifiedByName { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public int TotalItems { get; set; }
    }
}
