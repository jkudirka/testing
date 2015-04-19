using DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

#region Service Contract

[ServiceContract]
public interface IUserManager
{
    [OperationContract]
    void Login(string username, string password);

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
public class UserManager : IUserManager
{
    public void Login(string username, string password)
    {
        throw new NotImplementedException();
    }

    public void CreateUser(User user)
    {
        throw new NotImplementedException();
    }

    public User GetUser(string username)
    {
        throw new NotImplementedException();
    }

    public void UpdateUser(User user)
    {
        throw new NotImplementedException();
    }

    public void DeleteUser(User user)
    {
        throw new NotImplementedException();
    }

    public User[] GetUsers(string usernameFilter = null)
    {
        throw new NotImplementedException();
    }
}

