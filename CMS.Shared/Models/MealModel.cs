namespace CMS.Shared.Models
{
    using Utils;

    public class MealModel : BaseModel
    {
        public int MealId { get; set; }

        public string MealName { get; set; }

        public string MealCode { get; set; }

        public string Description { get; set; }
    }
}
