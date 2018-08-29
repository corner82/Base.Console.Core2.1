
namespace Base.Core.Token.Abstract
{
    public interface ITokenArgs
    {
        string PublicKey { get; set; }
        string PrivateKey { get; set; }
        string Salt { get; set; }
    }
}
