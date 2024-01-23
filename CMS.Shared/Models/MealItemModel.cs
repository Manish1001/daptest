namespace CMS.Shared.Models
{
    using Utils;

    public class MealItemModel : BaseModel
    {
        public int MealItemId { get; set; }

        public int MealId { get; set; }

        public int ProductId { get; set; }

        public short Quantity { get; set; }
    }
}
