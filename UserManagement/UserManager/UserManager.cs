using DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using ServiceModelEx;
using System.Transactions;
using Common;


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
    public bool UpdateUser(User user)
    {
        using (TransactionScope scope = new TransactionScope())
        {
            //var result = _DataAccess.Value.UpdateUser(user);
            //scope.Complete();
            //return result;

            var existingUser = _DataAccess.Value.GetUser(user.Username);
            if (existingUser == null)
                return false;

            // TODO: JimK - Determine if this is a requirement.
            // (i.e. LastPasswordChangeDate or LastPassword1-3 would not be valid).
            //
            //existingUser.IsPasswordResetRequired = user.IsPasswordResetRequired;

            // Currently the User is locked and the update is to unlock the User...clear the failed attempt count.
            if (existingUser.IsLocked && !user.IsLocked)
                existingUser.FailedLoginAttempts = 0;

            existingUser.IsLocked = user.IsLocked;

            // Check if the password is passed in and is different than the existing user so we know to update the Password.
            var hashedPassword = Helpers.HashPassword(user.Password);
            if (!string.IsNullOrEmpty(user.Password) && !existingUser.Password.Equals(hashedPassword))
            {
                existingUser.LastPassword3 = existingUser.LastPassword2;
                existingUser.LastPassword2 = existingUser.LastPassword1;
                existingUser.LastPassword1 = existingUser.Password;
                existingUser.Password = hashedPassword;
            }

            _DataAccess.Value.UpdateUser(existingUser);
            return true;

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

