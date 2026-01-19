// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.ExpressionExecution;

using Aqua.Dynamic;
using Aqua.TypeExtensions;
using System.Reflection;
using MethodInfo = System.Reflection.MethodInfo;

public class DefaultExpressionExecutor : ExpressionExecutor<IQueryable, DynamicObject>
{
    private const BindingFlags PrivateInstance = BindingFlags.NonPublic | BindingFlags.Instance;

    private static readonly MethodInfo _mapArrayMethodInfo = typeof(DefaultExpressionExecutor)
        .GetMethods(PrivateInstance)
        .Single(x => string.Equals(x.Name, nameof(MapArray), StringComparison.InvariantCulture) && x.IsGenericMethod);

    private readonly IDynamicObjectMapper _mapper;
    private readonly Func<Type, bool> _setTypeInformation;

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultExpressionExecutor"/> class.
    /// </summary>
    public DefaultExpressionExecutor(Func<Type, IQueryable> queryableProvider, IExpressionFromRemoteLinqContext? context = null, Func<Type, bool>? setTypeInformation = null)
        : base(queryableProvider, context)
    {
        _mapper = (context ?? ExpressionTranslatorContext.Default).ValueMapper
            ?? throw new ArgumentException($"{nameof(IExpressionValueMapperProvider.ValueMapper)} property must not be null.", nameof(context));
        _setTypeInformation = setTypeInformation ?? (t => !t.IsAnonymousType());
    }

    /// <summary>
    /// Converts the query result into a collection of <see cref="DynamicObject"/>.
    /// </summary>
    /// <param name="queryResult">The reult of the query execution.</param>
    /// <returns>The mapped query result.</returns>
    protected override DynamicObject ConvertResult(object? queryResult)
    {
        if (queryResult is not null)
        {
            var type = queryResult.GetType();
            if (type.IsArray)
            {
                var mappedArray = MapArray(queryResult, type.GetElementType()!);
                return new DynamicObject(
                    type,
                    new PropertySet(new[]
                    {
                        new Property(string.Empty, mappedArray),
                    }));
            }
        }

        return Map(queryResult, ExecutionContext.SystemExpression?.Type);
    }

    private DynamicObject Map(object? value, Type? type)
        => value is null
        ? DynamicObject.CreateDefault(type)
        : _mapper.MapObject(value, _setTypeInformation);

    private object[] MapArray(object array, Type elementType)
        => (object[])_mapArrayMethodInfo.MakeGenericMethod(elementType).Invoke(this, [array])!;

    private object[] MapArray<T>(T[] array)
        => [.. array.Select(x => (object)Map(x, typeof(T)))];
}