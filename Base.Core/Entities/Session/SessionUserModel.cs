﻿using System;
using System.Collections.Generic;

namespace Base.Core.Entities.Session
{
    [Serializable()]
    public class SessionUserModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
        public string SecurityStamp { get; set; }
        public string PhoneNumber { get; set; }
        public string ConcurrencyStamp { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string HmacToken { get; set; }
        public string UserAgent { get; set; }
        public List<SessionUserClaimModel> UserClaims { get; set; }
        //public List<Claim> RoleClaims { get; set; }
    }
}
