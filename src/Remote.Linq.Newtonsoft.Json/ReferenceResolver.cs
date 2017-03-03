// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using Aqua;
    using Newtonsoft.Json.Serialization;
    using System.Collections.Generic;
    using System.Linq;

    internal class ReferenceResolver : IReferenceResolver
    {
        private readonly Dictionary<object, int> _references = new Dictionary<object, int>(ReferenceEqualityComparer<object>.Default);

        public void AddReference(object context, string reference, object value)
        {
            var number = int.Parse(reference);
            lock (_references)
            {
                _references.Add(value, number);
            }
        }

        public string GetReference(object context, object value)
        {
            int number;
            lock (_references)
            {
                if (!_references.TryGetValue(value, out number))
                {
                    number = _references.Count + 1;
                    _references.Add(value, number);
                }
            }

            return number.ToString();
        }

        public bool IsReferenced(object context, object value)
        {
            lock (_references)
            {
                return _references.ContainsKey(value);
            }
        }

        public object ResolveReference(object context, string reference)
        {
            var number = int.Parse(reference);
            lock (_references)
            {
                return _references.Single(x => x.Value == number).Key;
            }
        }
    }
}
