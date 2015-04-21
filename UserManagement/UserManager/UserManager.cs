using Common;
using DataContracts;
using ServiceModelEx;
using System;
using System.Diagnostics;
using System.ServiceModel;
using System.Transactions;


[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Single)]
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
        // Allows nulls to be passed on to the Engine for validation.
        // Debug.Assert(!string.IsNullOrEmpty(username));
        // Debug.Assert(!string.IsNullOrEmpty(password));
        Debug.Assert(_Engine.Value != null && _Engine.IsValueCreated);

        return _Engine.Value.Authenticate(username, password);
    }

    public void CreateUser(User user)
    {
        Debug.Assert(user != null);
        Debug.Assert(!string.IsNullOrEmpty(user.Password));

        user.Password = Helpers.HashPassword(user.Password);
        user.PasswordLastChangedDate = DateTime.UtcNow.Date;
        _DataAccess.Value.CreateUser(user);
    }

    public User GetUser(string username)
    {
        Debug.Assert(!string.IsNullOrEmpty(username));
        Debug.Assert(_DataAccess.Value != null && _DataAccess.IsValueCreated);

        var user = _DataAccess.Value.GetUser(username);
        user.Password = string.Empty; // Blank out the encrypted password before returning the User.
        return user;
    }

    [OperationBehavior(TransactionScopeRequired = true)]
    public bool UpdateUser(User user)
    {
        Debug.Assert(user != null);
        Debug.Assert(!string.IsNullOrEmpty(user.Username));
        Debug.Assert(_DataAccess.Value != null && _DataAccess.IsValueCreated);

        var existingUser = _DataAccess.Value.GetUser(user.Username);
        if (existingUser == null)
            return false;

        // Currently the User is locked and the update is to unlock the User...clear the failed attempt count.
        if (existingUser.IsLocked && !user.IsLocked)
            existingUser.FailedLoginAttempts = 0;
        else
            existingUser.FailedLoginAttempts = user.FailedLoginAttempts;

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

    public void DeleteUser(User user)
    {
        Debug.Assert(user != null);
        Debug.Assert(_DataAccess.Value != null && _DataAccess.IsValueCreated);

        _DataAccess.Value.DeleteUser(user);
    }

    public User[] GetUsers(string usernameFilter = null)
    {
        Debug.Assert(_DataAccess.Value != null && _DataAccess.IsValueCreated);

        var users = _DataAccess.Value.GetUsers(usernameFilter);
        foreach(var user in users)
            user.Password = string.Empty; // Blank out the encrypted password before returning the User.
        
        return users;
    }
}

