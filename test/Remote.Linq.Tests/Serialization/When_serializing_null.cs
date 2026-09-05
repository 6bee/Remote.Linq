// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization;

using Aqua.TypeExtensions;
using Xunit;
using BindingFlags = System.Reflection.BindingFlags;
using MethodInfo = System.Reflection.MethodInfo;

// NOTES:
// - Binary formatter does not support null object graph.
// - Protobuf serialization does not support null object graph for arbitrary types.
//   aqua-core explicitly supports null values for System.Object types. However, the protobuf mapper for System.Object only supports specific runtime types.
public abstract class When_serializing_null(MethodInfo serialize)
{
    public class With_data_contract_serializer() : When_serializing_null(GetSerializationMethod(typeof(DataContractSerializationHelper), nameof(DataContractSerializationHelper.Clone)));

    public class With_newtonsoft_json_serializer() : When_serializing_null(GetSerializationMethod(typeof(NewtonsoftJsonSerializationHelper), nameof(NewtonsoftJsonSerializationHelper.Clone)));

    public class With_system_text_json_serializer() : When_serializing_null(GetSerializationMethod(typeof(SystemTextJsonSerializationHelper), nameof(SystemTextJsonSerializationHelper.Clone)));

    public class With_messagepack_serializer() : When_serializing_null(GetSerializationMethod(typeof(MessagePackSerializationHelper), nameof(MessagePackSerializationHelper.Clone)));

    public class With_xml_serializer() : When_serializing_null(GetSerializationMethod(typeof(XmlSerializationHelper), nameof(XmlSerializationHelper.Clone)));

#if NETFRAMEWORK
    public class With_net_data_contract_serializer() : When_serializing_null(GetSerializationMethod(typeof(NetDataContractSerializationHelper), nameof(NetDataContractSerializationHelper.Clone)));
#endif // NETFRAMEWORK

#pragma warning disable S2094 // Classes should not be empty
    /// <summary>Type definition used in generic type filters.</summary>
    private sealed class T;
#pragma warning restore S2094 // Classes should not be empty

    private static MethodInfo GetSerializationMethod(Type type, string name) => type.GetMethodEx(name, [typeof(T)], [typeof(T)], BindingFlags.Public | BindingFlags.Static);

    protected virtual object SerializeNull(Type type)
    {
        var method = serialize.MakeGenericMethod(type);
        var retult = method.InvokeAndUnwrap(null, [null]);
        return retult;
    }

    [Theory]
    [MemberData(nameof(Types))]
    public void Should_roundtrip_null(Type type)
    {
        if (this.TestIs<With_xml_serializer>())
        {
            Skip.If(type.Implements(typeof(System.Collections.IEnumerable)), $"{type} serialization is not supported");
            Skip.If(type.Implements(typeof(Remote.Linq.Closure<>)), $"{type} serialization is not supported");
        }

        var copy = SerializeNull(type);
        copy.ShouldBeNull();
    }

    public static TheoryData<Type> Types =>
    [

        // --- Query-related types (Remote.Linq) ---
        typeof(Remote.Linq.Closure<int>),
        typeof(Remote.Linq.Grouping<int, int>),

        // --- Expression types (Remote.Linq) ---
        typeof(Remote.Linq.Expressions.TryExpression),
        typeof(Remote.Linq.Expressions.BinaryExpression),
        typeof(Remote.Linq.Expressions.LambdaExpression),
        typeof(Remote.Linq.Expressions.BlockExpression),
        typeof(Remote.Linq.Expressions.ConditionalExpression),
        typeof(Remote.Linq.Expressions.ConstantExpression),
        typeof(Remote.Linq.Expressions.DefaultExpression),
        typeof(Remote.Linq.Expressions.ElementInit),
        typeof(Remote.Linq.Expressions.Expression),
        typeof(Remote.Linq.Expressions.ExpressionType?),
        typeof(Remote.Linq.Expressions.GotoExpression),
        typeof(Remote.Linq.Expressions.GotoExpressionKind?),
        typeof(Remote.Linq.Expressions.InvokeExpression),
        typeof(Remote.Linq.Expressions.LabelExpression),
        typeof(Remote.Linq.Expressions.LabelTarget),
        typeof(Remote.Linq.Expressions.ListInitExpression),
        typeof(Remote.Linq.Expressions.LoopExpression),
        typeof(Remote.Linq.Expressions.MemberAssignment),
        typeof(Remote.Linq.Expressions.MemberBinding),
        typeof(Remote.Linq.Expressions.MemberBindingType?),
        typeof(Remote.Linq.Expressions.MemberExpression),
        typeof(Remote.Linq.Expressions.MemberInitExpression),
        typeof(Remote.Linq.Expressions.MemberListBinding),
        typeof(Remote.Linq.Expressions.MemberMemberBinding),
        typeof(Remote.Linq.Expressions.MethodCallExpression),
        typeof(Remote.Linq.Expressions.NewArrayExpression),
        typeof(Remote.Linq.Expressions.NewArrayType?),
        typeof(Remote.Linq.Expressions.NewExpression),
        typeof(Remote.Linq.Expressions.ParameterExpression),
        typeof(Remote.Linq.Expressions.SortExpression),
        typeof(Remote.Linq.Expressions.SortDirection?),
        typeof(Remote.Linq.Expressions.SwitchCase),
        typeof(Remote.Linq.Expressions.SwitchExpression),
        typeof(Remote.Linq.Expressions.TypeBinaryExpression),
        typeof(Remote.Linq.Expressions.UnaryExpression),
        typeof(Remote.Linq.Expressions.UnaryOperator?),
        typeof(Remote.Linq.Expressions.CatchBlock),
        typeof(Remote.Linq.Expressions.BinaryOperator?),

        // --- DynamicQuery types (Remote.Linq) ---
        typeof(Remote.Linq.DynamicQuery.ConstantQueryArgument),
        typeof(Remote.Linq.DynamicQuery.QueryableResourceDescriptor),
        typeof(Remote.Linq.DynamicQuery.SubstitutionValue),
        typeof(Remote.Linq.DynamicQuery.VariableQueryArgument),
        typeof(Remote.Linq.DynamicQuery.VariableQueryArgument<int>),
        typeof(Remote.Linq.DynamicQuery.VariableQueryArgumentList),

        // --- SimpleQuery types (Remote.Linq) ---
        typeof(Remote.Linq.SimpleQuery.Query),
        typeof(Remote.Linq.SimpleQuery.Query<int>),

        // --- Async.Queryable types (Remote.Linq.Async.Queryable) ---
        typeof(Remote.Linq.AsyncEnumerable<int>),
        typeof(Remote.Linq.AsyncGrouping<int, int>),
    ];
}
