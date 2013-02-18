using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Remote.Linq.TestClassLibrary
{
    public class OtherObj
    {
        public int Id { get; set; }
        public string NameX { get; set; }
        public object this[string propertyName] { get { return null; } set { } }
    }
}
