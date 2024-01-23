namespace CMS.Api.Filters
{
    using System;
    using System.Collections.Generic;

    public class PolicySelectorWithLabel<T> where T : Attribute
    {
        public Func<IEnumerable<T>, IEnumerable<string>> Selector { get; set; }

        public string Label { get; set; }
    }
}
