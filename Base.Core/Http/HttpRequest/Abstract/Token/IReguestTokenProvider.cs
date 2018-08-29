using Base.Core.Token.Abstract;

namespace Base.Core.Http.HttpRequest.Abstract.Token
{
    public interface IReguestTokenProvider
    {
        RequestBuilderBase AddTokenCreator(ITokenCreater tokenCreator);
    }
}
