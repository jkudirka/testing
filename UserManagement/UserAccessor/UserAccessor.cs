using DataContracts;
using System;
using System.Configuration;
using System.Linq;
using System.ServiceModel;

#region Service Contract
[ServiceContract]
public interface IUserAccessor
{
    [OperationContract, TransactionFlow(TransactionFlowOption.Allowed)]
    void CreateUser(User user);

    [OperationContract, TransactionFlow(TransactionFlowOption.Allowed)]
    User GetUser(string username);

    [OperationContract, TransactionFlow(TransactionFlowOption.Allowed)]
    void UpdateUser(User user);

    [OperationContract, TransactionFlow(TransactionFlowOption.Allowed)]
    void DeleteUser(User user);

    [OperationContract, TransactionFlow(TransactionFlowOption.Allowed)]
    User[] GetUsers(string usernameFilter = null);
}
#endregion

[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
public class UserAccessor : IUserAccessor
{
    #region Fields

    private static readonly string _FilePath = ConfigurationManager.AppSettings["filePath"] ?? @"C:\UserStore.dat";
    private static object _SyncObject = new object();

    /// <summary>
    /// Field for interacting with a file-based User Store.  
    /// The constructor loads the persisted file everytime.
    /// </summary>
    private Lazy<UserStore> _UserStore = new Lazy<UserStore>(() =>
    {
        lock (_SyncObject)
        {
            return new UserStore(_FilePath);
        }
    });

    #endregion

    #region IUserAccessor Members

    [OperationBehavior(TransactionScopeRequired = true)]
    public void CreateUser(User user)
    {
        lock (_SyncObject)
        {
            _UserStore.Value.AddUser(user);
        }
    }

    [OperationBehavior(TransactionScopeRequired = false)]
    public User GetUser(string username)
    {
        lock (_SyncObject)
        {
            return _UserStore.Value.GetUser(username);
        }
    }

    [OperationBehavior(TransactionScopeRequired = true)]
    public void UpdateUser(User user)
    {
        lock (_SyncObject)
        {
            _UserStore.Value.UpdateUser(user);
        }
    }

    [OperationBehavior(TransactionScopeRequired = true)]
    public void DeleteUser(User user)
    {
        lock (_SyncObject)
        {
            _UserStore.Value.DeleteUser(user);
        }
    }

    [OperationBehavior(TransactionScopeRequired = false)]
    public User[] GetUsers(string usernameFilter = null)
    {
        lock (_SyncObject)
        {
            return _UserStore.Value.GetUsers(usernameFilter).ToArray();
        }
    }

    #endregion
}
