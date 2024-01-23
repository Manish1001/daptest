namespace CMS.Api.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.OpenApi.Models;
    using Swashbuckle.AspNetCore.SwaggerGen;

    public class AppendAuthorizeToSummaryOperationFilter<T> : IOperationFilter where T : Attribute
    {
        private readonly IEnumerable<PolicySelectorWithLabel<T>> policySelectors;

        public AppendAuthorizeToSummaryOperationFilter(IEnumerable<PolicySelectorWithLabel<T>> policySelectors) => this.policySelectors = policySelectors;

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var authorizeAttributes = context.MethodInfo.DeclaringType?.GetCustomAttributes(true)
               .Union(context.MethodInfo.GetCustomAttributes(true))
               .OfType<T>()
               .ToList();

            if (authorizeAttributes != null && authorizeAttributes.Any() && this.policySelectors.Any())
            {
                var authorizationDescription = new StringBuilder(string.Empty);
                foreach (var policySelector in this.policySelectors)
                {
                    AppendPolicies(authorizeAttributes, authorizationDescription, policySelector);
                }

                operation.Summary += authorizationDescription.ToString().TrimEnd(';');
            }
        }

        /// <summary>
        /// The append policies.
        /// </summary>
        /// <param name="authorizeAttributes">
        /// The authorize attributes.
        /// </param>
        /// <param name="authorizationDescription">
        /// The authorization description.
        /// </param>
        /// <param name="policySelector">
        /// The policy selector.
        /// </param>
        private static void AppendPolicies(IEnumerable<T> authorizeAttributes, StringBuilder authorizationDescription, PolicySelectorWithLabel<T> policySelector)
        {
            var policies = policySelector.Selector(authorizeAttributes).OrderBy(policy => policy).ToList();
            if (policies.Any())
            {
                authorizationDescription.Append($" {policySelector.Label}: {string.Join(", ", policies)};");
            }
        }
    }
}
