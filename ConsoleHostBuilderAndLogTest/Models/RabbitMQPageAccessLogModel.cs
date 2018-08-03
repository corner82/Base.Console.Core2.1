using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleHostBuilderAndLogTest.Models
{
    class RabbitMQPageAccessLogModel
    {
        public string UserName { get; set; }
        public string Action { get; set; }
        public string Controller { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string UserAgent { get; set; }
        public string UserIP { get; set; }
        public string Method { get; set; }
        public string SessionID { get; set; }
        public string UserToken { get; set; }
        public string UserPublicKey { get; set; }
        public string UserPrivateKey { get; set; }
    }
}
