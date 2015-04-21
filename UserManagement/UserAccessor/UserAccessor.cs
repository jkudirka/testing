using Common;
using DataContracts;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;


[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Single)]
public class UserAccessor : IUserAccessor
{
    #region Fields

    private static readonly string _FilePath = ConfigurationManager.AppSettings["filePath"] ?? @"C:\UserStore.dat";
    private static readonly object _SyncObject = new object();

    /// <summary>
    /// Field for interacting with a file-based User Store.  
    /// The constructor loads the persisted file everytime.
    /// </summary>
    private readonly Lazy<UserStore> _UserStore = new Lazy<UserStore>(() =>
    {
        Debug.Assert(!string.IsNullOrEmpty(_FilePath));
        Debug.Assert(_SyncObject != null);

        lock (_SyncObject)
        {
            return new UserStore(_FilePath);
        }
    });

    #endregion

    #region IUserAccessor Members

    [OperationBehavior(TransactionScopeRequired = false)]
    public void CreateUser(User user)
    {
        Debug.Assert(_SyncObject != null);
        Debug.Assert(_UserStore.Value != null && _UserStore.IsValueCreated);
        Debug.Assert(user != null);

        lock (_SyncObject)
        {
            _UserStore.Value.AddUser(user);
        }
    }

    [OperationBehavior(TransactionScopeRequired = false)]
    public User GetUser(string username)
    {
        Debug.Assert(_SyncObject != null);
        Debug.Assert(_UserStore.Value != null && _UserStore.IsValueCreated);
        Debug.Assert(!string.IsNullOrEmpty(username));

        lock (_SyncObject)
        {
            return _UserStore.Value.GetUser(username);
        }
    }

    [OperationBehavior(TransactionScopeRequired = true)]
    public void UpdateUser(User user)
    {
        Debug.Assert(_SyncObject != null);
        Debug.Assert(_UserStore.Value != null && _UserStore.IsValueCreated);
        Debug.Assert(user != null);

        lock (_SyncObject)
        {
            _UserStore.Value.UpdateUser(user);
        }
    }

    [OperationBehavior(TransactionScopeRequired = false)]
    public void DeleteUser(User user)
    {
        Debug.Assert(_SyncObject != null);
        Debug.Assert(_UserStore.Value != null && _UserStore.IsValueCreated);
        Debug.Assert(user != null);

        lock (_SyncObject)
        {
            _UserStore.Value.DeleteUser(user);
        }
    }

    [OperationBehavior(TransactionScopeRequired = false)]
    public User[] GetUsers(string usernameFilter = null)
    {
        Debug.Assert(_SyncObject != null);
        Debug.Assert(_UserStore.Value != null && _UserStore.IsValueCreated);

        lock (_SyncObject)
        {
            return _UserStore.Value.GetUsers(usernameFilter).ToArray();
        }
    }

    #endregion
}
