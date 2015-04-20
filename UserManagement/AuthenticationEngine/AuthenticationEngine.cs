using Common;
using DataContracts;
using ServiceModelEx;
using System;
using System.Linq;
using System.Text;

//[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
public class AuthenticationEngine : IAuthenticationEngine
{
    #region Fields
    private const int MaxLoginAttempts = 5;
    private const int PasswordExpirationInDays = 45;
    private readonly Lazy<IUserAccessor> _DataAccess =
        new Lazy<IUserAccessor>(() => InProcFactory.CreateInstance<UserAccessor, IUserAccessor>());
    #endregion

    public AuthenticationResult Authenticate(string username, string password)
    {
        var results = AuthenticateUser(username, password);
        if (!results.IsSuccessful)
            return results;

        var user = results.User;

        // Credentials are valid.
        if (HasPasswordExpired(user))
            return new AuthenticationResult { IsSuccessful = false, Reason = "Password has expired." };

        //
        // TODO: JimK - Determine if this is a desired requirement!
        //
        //if (user.IsPasswordResetRequired)
        //    return new AuthenticationResult { IsSuccessful = false, Reason = "Password reset required." };

        // Login Successful!
        if (user.FailedLoginAttempts > 0)
        {
            user.FailedLoginAttempts = 0;
            _DataAccess.Value.UpdateUser(user);
        }

        results.AuthToken = GenerateAuthToken(user);
        results.Reason = "Login Successful!";
        return results;
    }

    private AuthenticationResult AuthenticateUser(string username, string password)
    {
        var user = _DataAccess.Value.GetUser(username);

        if (user == null || !user.Username.Equals(username, StringComparison.OrdinalIgnoreCase))
            return new AuthenticationResult { IsSuccessful = false, Reason = "Invalid credentials." };

        if (user.IsLocked)
            return new AuthenticationResult { IsSuccessful = false, Reason = "User Account Locked." };

        if (!AreCredentialsValid(user, password))
        {
            user.FailedLoginAttempts++;

            if (user.FailedLoginAttempts >= MaxLoginAttempts)
            {
                user.IsLocked = true;
                _DataAccess.Value.UpdateUser(user);
                return new AuthenticationResult { IsSuccessful = false, Reason = "Maximum number of failed login attempts exceeded." };
            }
            else
            {
                _DataAccess.Value.UpdateUser(user);
                return new AuthenticationResult { IsSuccessful = false, Reason = "Invalid credentials." };
            }
        }

        return new AuthenticationResult { IsSuccessful = true, User = user };
    }

    private bool HasPasswordExpired(User user)
    {
        if (!user.PasswordLastChangedDate.HasValue)
            return true;
        return (DateTime.UtcNow.Date.Subtract(user.PasswordLastChangedDate.Value.Date).TotalDays >= (double)PasswordExpirationInDays);
    }

    //[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage()]
    private string GenerateAuthToken(User user)
    {
        var token = Guid.NewGuid().ToString();
        //if (!CachingRepository<User>.StoreItem(token, user, TimeSpan.FromSeconds(Global.SessionExpirationInSeconds)))
        //    throw new ApplicationException("Could not store token for user =\"" + user.Username + "\" in Memcached!");
        return token;
    }

    private bool AreCredentialsValid(User user, string password)
    {
        string hashedPassword = Helpers.HashPassword(password);
        return hashedPassword == user.Password;
    }
}
