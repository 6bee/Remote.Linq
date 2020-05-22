// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

#if COREFX

namespace Remote.Linq.Tests.ProtoBuf
{
    using global::ProtoBuf;
    using global::ProtoBuf.Meta;
    using Remote.Linq.DynamicQuery;
    using Shouldly;
    using System;
    using System.IO;
    using System.Linq;
    using Xunit;
    using static Remote.Linq.Tests.Serialization.ProtobufNetSerializationHelper;

    /// <summary>
    /// This class is rather a documentation of know limitations to <i>Remote.Linq</i>'s support for <i>protobuf-net</i> that actual tests.
    /// </summary>
    public class Issues_arise
    {
        [Fact]
        public void When_serializing_method_call_expression_with_parameter_and_variable_argument_A()
        {
            var keys = new[] { 1, 2, 3 };
            System.Linq.Expressions.Expression<Func<int, bool>> expression = x => keys.Contains(x);
            var remoteExpression = expression.ToRemoteLinqExpression();
            Should.Throw<ProtoException>(remoteExpression.Serialize);
        }

        [Fact]
        public void When_serializing_method_call_expression_with_parameter_and_variable_argument_B()
        {
            var m = typeof(Math).GetMethod(nameof(Math.Pow), System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            var v = System.Linq.Expressions.Expression.Constant(new VariableQueryArgument(2.0, typeof(double)));
            var b = System.Linq.Expressions.Expression.Convert(
                System.Linq.Expressions.Expression.MakeMemberAccess(v, typeof(VariableQueryArgument).GetProperty(nameof(VariableQueryArgument.Value))),
                typeof(double));
            var pExp = System.Linq.Expressions.Expression.Parameter(typeof(double), "exp");
            var c = System.Linq.Expressions.Expression.Call(m, b, pExp);
            var l = System.Linq.Expressions.Expression.Lambda(c, pExp);
            var r = (double)l.Compile().DynamicInvoke(8);
            var remoteExpression = l.ToRemoteLinqExpression();

            // throws on deep clone
            Should.Throw<ProtoException>(remoteExpression.Serialize);

            var config = ProtoBufTypeModel.ConfigureRemoteLinq();
            using var memorystream = new MemoryStream();

            // while serialization seems fine ...
            config.Serialize(memorystream, remoteExpression);

            memorystream.Position = 0;
            var schema = config.GetSchema(remoteExpression.GetType(), ProtoSyntax.Proto3);

            // ... actually throws on deserialize.
            Should.Throw<ProtoException>(() => config.Deserialize(memorystream, null, remoteExpression.GetType()));
        }
    }
}

#endif // COREFX