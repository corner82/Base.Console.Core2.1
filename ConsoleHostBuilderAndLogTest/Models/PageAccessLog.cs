using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleHostBuilderAndLogTest.Models
{
    public class PageAccessLog
    {
        public int ID { get; set; }
        public string ActionTest { get; set; }
        public string Action { get; set; }
        public string Controller { get; set; }
        public DateTime AccessDate { get; set; }
    }
}
