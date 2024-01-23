namespace CMS.Shared.Handlers
{
    using System.Linq;

    public static class ObjectHandler
    {
        public static object EmptyStringToNull(object model)
        {
            var properties = model.GetType().GetProperties().Where(m => m.PropertyType == typeof(string));
            foreach (var item in properties)
            {
                var value = item.GetValue(model)?.ToString();
                if (value == string.Empty)
                {
                    item.SetValue(model, null);
                }
            }

            return model;
        }
    }
}