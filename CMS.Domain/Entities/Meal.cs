namespace CMS.Domain.Entities
{
    using Common;

    public class Meal : BaseEntity
    {
        public int MealId { get; set; }

        public string MealName { get; set; }

        public string MealCode { get; set; }

        public string Description { get; set; }
    }
}
