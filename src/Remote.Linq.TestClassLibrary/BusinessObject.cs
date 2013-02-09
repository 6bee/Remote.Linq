using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Remote.Linq.TestClassLibrary
{
    public class BusinessObject
    {
        public int Id { get; set; }
        public bool Flag { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public OtherObj OtherObj { get; set; }
        public BusinessObject Parent { get; set; }
        public object this[int index] { get { return null; } set { } }
        public object this[string propertyName] { get { return null; } set { } }
    }
}
