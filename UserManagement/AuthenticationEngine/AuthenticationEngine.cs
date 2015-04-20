using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using DataContracts;


[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
public class AuthenticationEngine : IAuthenticationEngine
{
    public AuthenticationResult Authenticate(string username, string password)
    {
        return new AuthenticationResult { IsSuccessful = true, AuthToken = Guid.NewGuid().ToString() };
    }
}
