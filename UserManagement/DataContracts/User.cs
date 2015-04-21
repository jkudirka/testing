﻿using System;
using System.Runtime.Serialization;

namespace DataContracts
{
    [DataContract, Serializable]
    public class User
    {
        [DataMember]
        public string Username { get; set; }
        [DataMember]
        public string Password { get; set; }
        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public string LastName { get; set; }
        [DataMember]
        public bool IsLocked { get; set; }
        [DataMember]
        public DateTime PasswordLastChangedDate { get; set; }
        [DataMember]
        public string LastPassword1 { get; set; }
        [DataMember]
        public string LastPassword2 { get; set; }
        [DataMember]
        public string LastPassword3 { get; set; }
        [DataMember]
        public int FailedLoginAttempts { get; set; }
    }
}
