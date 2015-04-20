using DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using ServiceModelEx;
using System.Transactions;


//[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
public class UserManager : IUserManager
{
    #region Fields
    private readonly Lazy<IAuthenticationEngine> _Engine =
        new Lazy<IAuthenticationEngine>(() => InProcFactory.CreateInstance<AuthenticationEngine, IAuthenticationEngine>());

    private readonly Lazy<IUserAccessor> _DataAccess =
        new Lazy<IUserAccessor>(() => InProcFactory.CreateInstance<UserAccessor, IUserAccessor>());
    #endregion

    public AuthenticationResult Login(string username, string password)
    {
        return _Engine.Value.Authenticate(username, password);
    }

    public void CreateUser(User user)
    {
        _DataAccess.Value.CreateUser(user);
    }

    public User GetUser(string username)
    {
        return _DataAccess.Value.GetUser(username);
    }

    [OperationBehavior(TransactionScopeRequired = true)]
    public void UpdateUser(User user)
    {
        using (TransactionScope scope = new TransactionScope())
        {
            _DataAccess.Value.UpdateUser(user);
            scope.Complete();
        }
    }

    public void DeleteUser(User user)
    {
        _DataAccess.Value.DeleteUser(user);
    }

    public User[] GetUsers(string usernameFilter = null)
    {
        return _DataAccess.Value.GetUsers(usernameFilter);
    }
}

