namespace CMS.Api.Contracts
{
    // ReSharper disable UnusedAutoPropertyAccessor.Global
    public class HttpResponse
    {
        public HttpResponse()
        {
            this.IsSuccess = true;
            this.StatusCode = 204;
        }

        public HttpResponse(object data)
        {
            this.IsSuccess = true;
            this.Data = data;
            this.StatusCode = 200;
        }

        public HttpResponse(string message, object data)
        {
            this.IsSuccess = true;
            this.Data = data;
            this.Message = message;
            this.StatusCode = 200;
        }

        public int StatusCode { get; set; }

        public bool IsSuccess { get; set; }

        public object Data { get; set; }

        public string Message { get; set; }

        public Exception Exception { get; set; }

        public static HttpResponse Success()
        {
            return new HttpResponse { IsSuccess = true, StatusCode = 204 };
        }

        public static HttpResponse Success(object data)
        {
            return new HttpResponse { IsSuccess = true, Data = data, StatusCode = 200 };
        }

        public static HttpResponse Success(string message, object data)
        {
            return new HttpResponse { IsSuccess = true, Data = data, Message = message, StatusCode = 200 };
        }

        public static HttpResponse Fail()
        {
            return new HttpResponse { IsSuccess = false, StatusCode = 500 };
        }

        public static HttpResponse Fail(string message)
        {
            return new HttpResponse { IsSuccess = false, Message = message, StatusCode = 500 };
        }

        public static HttpResponse Fail(Exception exception)
        {
            return new HttpResponse { IsSuccess = false, Exception = exception, StatusCode = 500 };
        }

        public static HttpResponse Fail(int statusCode, string message)
        {
            return new HttpResponse { IsSuccess = false, Message = message, StatusCode = 500 };
        }

        public static HttpResponse Fail(string message, Exception exception)
        {
            return new HttpResponse { IsSuccess = false, Message = message, Exception = exception, StatusCode = 500 };
        }

        public override string ToString()
        {
            return this.IsSuccess ? this.Message : this.Exception == null ? this.Message : $"{this.Message} : {this.Exception.Message}";
        }
    }
}