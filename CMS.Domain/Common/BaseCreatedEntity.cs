namespace CMS.Domain.Common
{
    public class BaseCreatedEntity
    {
        public bool IsActive { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
