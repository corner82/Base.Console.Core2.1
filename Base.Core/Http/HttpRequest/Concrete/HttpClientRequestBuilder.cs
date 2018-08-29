using Base.Core.Token.Abstract;

namespace Base.Core.Http.HttpRequest.Concrete
{
    public class HttpClientRequestBuilder : RequestBuilderBase
    {
        protected  string _acceptHeader = "application/json";
        protected string _token = "";
        ITokenCreater _tokenCreator = null;

        public HttpClientRequestBuilder () :base()
        {
        }

        public string Test()
        {
            return "";
        }


    }
}
