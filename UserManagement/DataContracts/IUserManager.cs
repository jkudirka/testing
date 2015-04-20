using System.ServiceModel;

namespace DataContracts
{
    [ServiceContract]
    public interface IUserManager
    {
        [OperationContract]
        AuthenticationResult Login(string username, string password);

        [OperationContract, TransactionFlow(TransactionFlowOption.Allowed)]
        void CreateUser(User user);

        [OperationContract, TransactionFlow(TransactionFlowOption.Allowed)]
        User GetUser(string username);

        [OperationContract, TransactionFlow(TransactionFlowOption.Allowed)]
        bool UpdateUser(User user);

        [OperationContract, TransactionFlow(TransactionFlowOption.Allowed)]
        void DeleteUser(User user);

        [OperationContract, TransactionFlow(TransactionFlowOption.Allowed)]
        User[] GetUsers(string usernameFilter = null);
    }
}
