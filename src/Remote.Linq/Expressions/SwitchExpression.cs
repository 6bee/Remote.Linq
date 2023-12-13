// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions
{
    using Aqua.EnumerableExtensions;
    using Aqua.TypeSystem;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    [Serializable]
    [DataContract]
    public sealed class SwitchExpression : Expression
    {
        public SwitchExpression()
        {
        }

        public SwitchExpression(Expression switchValue, MethodInfo? comparison, Expression? defaultExpression, IEnumerable<SwitchCase>? cases)
        {
            SwitchValue = switchValue.CheckNotNull();
            Comparison = comparison;
            DefaultExpression = defaultExpression;
            Cases = cases?.ToList();
        }

        public SwitchExpression(Expression switchValue, System.Reflection.MethodInfo? comparison, Expression? defaultExpression, IEnumerable<SwitchCase> cases)
            : this(switchValue, comparison.AsMethodInfo(), defaultExpression, cases)
        {
        }

        public override ExpressionType NodeType => ExpressionType.Switch;

        [DataMember(Order = 1, IsRequired = true, EmitDefaultValue = false)]
        public Expression SwitchValue { get; set; } = null!;

        [DataMember(Order = 2, IsRequired = false, EmitDefaultValue = false)]
        public MethodInfo? Comparison { get; set; }

        [DataMember(Order = 3, IsRequired = false, EmitDefaultValue = false)]
        public Expression? DefaultExpression { get; set; }

        [DataMember(Order = 4, IsRequired = false, EmitDefaultValue = false)]
        public List<SwitchCase>? Cases { get; set; }

        public override string ToString() => $"switch({SwitchValue}) {{ {Cases.StringJoin("; ")}; {DefaultExpression} }}";
    }
}