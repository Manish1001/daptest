namespace CMS.Domain.Entities
{
    using Common;

    public class MealItem : BaseEntity
    {
        public int MealItemId { get; set; }

        public int MealId { get; set; }

        public int ProductId { get; set; }

        public short Quantity { get; set; }
    }
}
