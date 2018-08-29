using System;
using System.Collections.Generic;
using System.Text;

namespace Base.Core.Culture
{
    class JsonLocalization
    {
        public string Key { get; set; }   
        public Dictionary<string, string> LocalizedValue = new Dictionary<string, string>();

    }
}
