using Common;
using DataContracts;
using ServiceModelEx;
using System;
using System.Diagnostics;
using System.ServiceModel;

[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Single)]
public class AuthenticationEngine : IAuthenticationEngine
{
    #region Constants

    private const int _MaxLoginAttempts = 5;
    private const int _PasswordExpirationInDays = 45;
    private const string _InvalidCredentials = "Invalid credentials.";
    private const string _PasswordHasExpired = "Password has expired.";
    private const string _UserAccountLocked = "User Account Locked.";
    private const string _MaxAttemptsExceeded = "Maximum number of failed login attempts exceeded.";
    private const string _LoginSuccessful = "Login Successful!";

    #endregion

    #region Fields

    private readonly Lazy<IUserAccessor> _DataAccess =
        new Lazy<IUserAccessor>(() => InProcFactory.CreateInstance<UserAccessor, IUserAccessor>());

    #endregion

    public AuthenticationResult Authenticate(string username, string password)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            return new AuthenticationResult { IsSuccessful = false, Reason = _InvalidCredentials };

        Debug.Assert(!string.IsNullOrEmpty(username));
        Debug.Assert(!string.IsNullOrEmpty(password));
        Debug.Assert(_DataAccess.Value != null && _DataAccess.IsValueCreated);

        var results = AuthenticateUser(username, password);
        Debug.Assert(results != null);

        if (!results.IsSuccessful)
            return results;

        var user = results.User;
        Debug.Assert(user != null);

        // Credentials are valid.
        if (HasPasswordExpired(user))
            return new AuthenticationResult { IsSuccessful = false, Reason = _PasswordHasExpired };

        // Login Successful!
        if (user.FailedLoginAttempts > 0)
        {
            user.FailedLoginAttempts = 0;
            _DataAccess.Value.UpdateUser(user);
        }

        results.AuthToken = GenerateAuthToken(user);
        results.Reason = _LoginSuccessful;
        return results;
    }

    private AuthenticationResult AuthenticateUser(string username, string password)
    {
        Debug.Assert(!string.IsNullOrEmpty(username));
        Debug.Assert(!string.IsNullOrEmpty(password));
        Debug.Assert(_DataAccess.Value != null && _DataAccess.IsValueCreated);

        var user = _DataAccess.Value.GetUser(username);

        if (user == null || !user.Username.Equals(username, StringComparison.OrdinalIgnoreCase))
            return new AuthenticationResult { IsSuccessful = false, Reason = _InvalidCredentials };

        if (user.IsLocked)
            return new AuthenticationResult { IsSuccessful = false, Reason = _UserAccountLocked };

        if (!AreCredentialsValid(user, password))
        {
            user.FailedLoginAttempts++;

            if (user.FailedLoginAttempts >= _MaxLoginAttempts)
            {
                user.IsLocked = true;
                _DataAccess.Value.UpdateUser(user);
                return new AuthenticationResult { IsSuccessful = false, Reason = _MaxAttemptsExceeded };
            }
            else
            {
                _DataAccess.Value.UpdateUser(user);
                return new AuthenticationResult { IsSuccessful = false, Reason = _InvalidCredentials };
            }
        }

        return new AuthenticationResult { IsSuccessful = true, User = user };
    }

    private bool HasPasswordExpired(User user)
    {
        Debug.Assert(user != null);
        Debug.Assert(user.PasswordLastChangedDate != null);

        return (DateTime.UtcNow.Date.Subtract(user.PasswordLastChangedDate.Date).TotalDays >= (double)_PasswordExpirationInDays);
    }

    //[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage()]
    private string GenerateAuthToken(User user)
    {
        Debug.Assert(user != null);

        var token = Guid.NewGuid().ToString();
        //if (!CachingRepository<User>.StoreItem(token, user, TimeSpan.FromSeconds(Global.SessionExpirationInSeconds)))
        //    throw new ApplicationException("Could not store token for user =\"" + user.Username + "\" in Memcached!");
        return token;
    }

    private bool AreCredentialsValid(User user, string password)
    {
        Debug.Assert(user != null);
        Debug.Assert(!string.IsNullOrEmpty(password));
        Debug.Assert(!string.IsNullOrEmpty(user.Password));

        string hashedPassword = Helpers.HashPassword(password);
        return hashedPassword == user.Password;
    }
}
