namespace CMS.Api.Contracts
{
    using System;
    using System.Globalization;

    public class HttpException : Exception
    {
        public HttpException()
        {
        }

        public HttpException(string message) : base(message)
        {
        }

        public HttpException(string message, params object[] args) : base(string.Format(CultureInfo.CurrentCulture, message, args))
        {
        }
    }
}