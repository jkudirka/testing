using System;
using System.Runtime.Serialization;

namespace DataContracts
{
    [DataContract, Serializable]
    public class AuthenticationResult
    {
        [DataMember]
        public bool IsSuccessful { get; set; }

        [DataMember]
        public string AuthToken { get; set; }

        [DataMember]
        public string Reason { get; set; }

        [IgnoreDataMember]
        public User User { get; set; }
    }
}
