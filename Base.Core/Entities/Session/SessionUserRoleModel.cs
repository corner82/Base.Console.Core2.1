using System;

namespace Base.Core.Entities.Session
{
    [Serializable()]
    public class SessionUserRoleModel
    {
        public string id { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }
    }
}
