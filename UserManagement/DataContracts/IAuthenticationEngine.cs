using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace DataContracts
{
    [ServiceContract]
    public interface IAuthenticationEngine
    {
        [OperationContract]
        AuthenticationResult Authenticate(string username, string password);
    }
}
