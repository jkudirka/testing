using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

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
    }
}
