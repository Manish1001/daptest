namespace CMS.Api.Contracts
{
    using AutoWrapper;

    public abstract class MapResponseObject
    {
        [AutoWrapperPropertyMap(Prop.Result)]
        public object Data { get; set; }

        [AutoWrapperPropertyMap(Prop.ResponseException)]
        public object Exception { get; set; }

        [AutoWrapperPropertyMap(Prop.ResponseException_ExceptionMessage)]
        public object Content { get; set; }
    }
}
