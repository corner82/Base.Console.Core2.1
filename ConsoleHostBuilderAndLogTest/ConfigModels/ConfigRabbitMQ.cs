using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleHostBuilderAndLogTest.ConfigModels
{
    public class ConfigRabbitMQ
    {
        public string HostName { get; set; }
        public string PageLogQueue { get; set; }
        public string ExceptionLogQueue { get; set; }
    }
}
