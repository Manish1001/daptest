namespace CMS.Domain.Common
{
    public class BaseModifiedEntity
    {
        public bool IsActive { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }
    }
}
