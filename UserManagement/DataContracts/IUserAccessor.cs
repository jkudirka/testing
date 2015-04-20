﻿using System.ServiceModel;

namespace DataContracts
{
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
}
