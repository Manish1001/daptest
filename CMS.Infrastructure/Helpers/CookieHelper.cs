namespace CMS.Infrastructure.Helpers
{
    using System;
    using Domain.Helpers;
    using Microsoft.AspNetCore.Http;

    public class CookieHelper : ICookieHelper
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public CookieHelper(IHttpContextAccessor httpContextAccessor) => this.httpContextAccessor = httpContextAccessor;

        public void Create(string key, string value, int? expireTime)
        {
            var options = new CookieOptions
            {
                Expires = expireTime.HasValue ? DateTime.Now.AddMinutes(expireTime.Value) : DateTime.Now.AddMilliseconds(10)
            };

            this.httpContextAccessor.HttpContext.Response.Cookies.Append(key, value, options);
        }

        public string Get(string key)
        {
            try
            {
                return this.httpContextAccessor.HttpContext.Request.Cookies[key];
            }
            catch
            {
                return string.Empty;
            }
        }

        public void Remove(string key)
        {
            this.httpContextAccessor.HttpContext.Response.Cookies.Delete(key);
        }

        public void DeleteAll(string key)
        {
            var cookiesKeys = this.httpContextAccessor.HttpContext.Request.Cookies.Keys;
            foreach (var cookiesKey in cookiesKeys)
            {
                if (cookiesKey.Contains(key))
                {
                    this.httpContextAccessor.HttpContext.Response.Cookies.Delete(cookiesKey);
                }
            }
        }

        public void DeleteAll()
        {
            var cookiesKeys = this.httpContextAccessor.HttpContext.Request.Cookies.Keys;
            foreach (var cookiesKey in cookiesKeys)
            {
                this.httpContextAccessor.HttpContext.Response.Cookies.Delete(cookiesKey);
            }
        }
    }
}