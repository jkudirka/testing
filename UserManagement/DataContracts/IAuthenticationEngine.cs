using System.ServiceModel;

namespace DataContracts
{
    [ServiceContract]
    public interface IAuthenticationEngine
    {
        [OperationContract]
        AuthenticationResult Authenticate(string username, string password);
    }
}
