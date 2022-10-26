// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using Aqua.Dynamic;
    using Aqua.TypeExtensions;
    using Aqua.TypeSystem;
    using Remote.Linq.DynamicQuery;
    using System;
    using System.Collections;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using MethodInfo = System.Reflection.MethodInfo;
    using SystemLinq = System.Linq.Expressions;

    /// <summary>
    /// Context for translating expressions from <i>System.Linq</i> to <i>Remote.Linq</i> and vice versa.
    /// </summary>
    public class ExpressionTranslatorContext : IExpressionTranslatorContext
    {
        private sealed class NoMappingContextImpl : IExpressionTranslatorContext, IDynamicObjectMapper
        {
            public ITypeInfoProvider TypeInfoProvider => new TypeInfoProvider();

            public Func<object, bool> NeedsMapping => _ => false;

            public IDynamicObjectMapper ValueMapper => this;

            public Func<SystemLinq.Expression, bool>? CanBeEvaluatedLocally => _ => false;

            object? IDynamicObjectMapper.Map(DynamicObject? obj, Type? targetType) => throw NotSupportedException;

            DynamicObject? IDynamicObjectMapper.MapObject(object? obj, Func<Type, bool>? setTypeInformation) => throw NotSupportedException;

            public ITypeResolver TypeResolver => Aqua.TypeSystem.TypeResolver.Instance;

            private NotSupportedException NotSupportedException => new NotSupportedException("Operation must not be called as no value mapping should occure.");

            public IExpressionTranslator ExpressionTranslator => new ExpressionTranslator(this);
        }

        private sealed class IsKnownTypeProviderDecorator : IIsKnownTypeProvider
        {
            private readonly Func<Type, bool> _parent;

            public IsKnownTypeProviderDecorator(IIsKnownTypeProvider? parent)
                => _parent = parent is null
                ? _ => false
                : parent.IsKnownType;

            public bool IsKnownType(Type type)
            {
                if (_isPrimitiveType(type))
                {
                    return true;
                }

                if (IsUnmappedType(type))
                {
                    return true;
                }

                if (_parent(type))
                {
                    return true;
                }

                return false;
            }

            private static bool IsUnmappedType(Type type)
            {
                var t = type.IsGenericType ? type.GetGenericTypeDefinition() : type;
                return _unmappedTypes.Any(x => x.IsAssignableFrom(t))
                    && !_excludeFromUnmappedTypes.Any(x => x.IsAssignableFrom(t));
            }
        }

        protected class ExpressionTranslatorContextObjectMapper : DynamicObjectMapper
        {
            public ExpressionTranslatorContextObjectMapper(ExpressionTranslatorContext expressionTranslatorContext)
                : base(expressionTranslatorContext.TypeResolver, expressionTranslatorContext.TypeInfoProvider, isKnownTypeProvider: expressionTranslatorContext.IsKnownTypeProvider)
            {
            }

            public ExpressionTranslatorContextObjectMapper(ITypeResolver typeResolver, ITypeInfoProvider typeInfoProvider, IIsKnownTypeProvider isKnownTypeProvider)
                : base(typeResolver, typeInfoProvider, isKnownTypeProvider: isKnownTypeProvider)
            {
            }

            protected override bool ShouldMapToDynamicObject(IEnumerable collection)
                => collection.CheckNotNull(nameof(collection)).GetType().Implements(typeof(IGrouping<,>))
                || base.ShouldMapToDynamicObject(collection);

            protected override DynamicObject? MapToDynamicObjectGraph(object? obj, Func<Type, bool> setTypeInformation)
            {
                var genericTypeArguments = default(Type[]);
                if (obj?.GetType().Implements(typeof(IGrouping<,>), out genericTypeArguments) is true)
                {
                    obj = MapGroupToDynamicObjectGraphMethod(genericTypeArguments!, obj);
                }

                return base.MapToDynamicObjectGraph(obj, setTypeInformation);
            }
        }

        private const BindingFlags PrivateStatic = BindingFlags.NonPublic | BindingFlags.Static;

        private static readonly MethodInfo _mapGroupToDynamicObjectGraphMethodDefinition =
            typeof(ExpressionTranslatorContext).GetMethodEx(nameof(MapGroupToDynamicObjectGraph));

        private static readonly Func<Type, bool> _isPrimitiveType = new[]
            {
                typeof(string),
                typeof(int),
                typeof(uint),
                typeof(byte),
                typeof(sbyte),
                typeof(short),
                typeof(ushort),
                typeof(long),
                typeof(ulong),
                typeof(float),
                typeof(double),
                typeof(decimal),
                typeof(char),
                typeof(bool),
                typeof(Guid),
                typeof(DateTime),
                typeof(TimeSpan),
                typeof(DateTimeOffset),
                typeof(System.Numerics.BigInteger),
                typeof(System.Numerics.Complex),
                typeof(byte[]),
            }
            .SelectMany(x => x.IsValueType ? new[] { x, typeof(Nullable<>).MakeGenericType(x) } : new[] { x })
            .ToHashSet()
            .Contains;

        private static readonly Type[] _unmappedTypes = new[]
            {
                typeof(CancellationToken),
                typeof(ConstantQueryArgument),
                typeof(VariableQueryArgument),
                typeof(VariableQueryArgumentList),
                typeof(QueryableResourceDescriptor),
                typeof(VariableQueryArgument<>),
                typeof(SystemLinq.Expression),
                typeof(IQueryable),
                typeof(IRemoteLinqQueryable),
            };

        private static readonly Type[] _excludeFromUnmappedTypes = new[]
            {
                typeof(EnumerableQuery),
            };

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionTranslatorContext"/> class.
        /// </summary>
        /// <param name="typeInfoProvider">Type info provider is typically used when translating from <i>System.Linq</i> to <i>Remote.Linq</i>.</param>
        /// <param name="canBeEvaluatedLocally">Function to check whether expressions or parts thereof are eligibly for local evaluation.</param>
        public ExpressionTranslatorContext(ITypeInfoProvider? typeInfoProvider, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally)
            : this(null, typeInfoProvider, null, canBeEvaluatedLocally)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionTranslatorContext"/> class.
        /// </summary>
        /// <param name="typeResolver">Type resolver is typically used when translating from <i>Remote.Linq</i> to <i>System.Linq</i>.</param>
        /// <param name="canBeEvaluatedLocally">Function to check whether expressions or parts thereof are eligibly for local evaluation.</param>
        public ExpressionTranslatorContext(ITypeResolver? typeResolver, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally)
            : this(typeResolver, null, null, canBeEvaluatedLocally)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionTranslatorContext"/> class.
        /// </summary>
        /// <param name="typeResolver">Type resolver is typically used when translating from <i>Remote.Linq</i> to <i>System.Linq</i>.</param>
        /// <param name="typeInfoProvider">Type info provider is typically used when translating from <i>System.Linq</i> to <i>Remote.Linq</i>.</param>
        /// <param name="isKnownTypeProvider">Provider for checking types to be known. Unknown types require mapping (i.e. substitution) on translating expressions.</param>
        /// <param name="canBeEvaluatedLocally">Function to check whether expressions or parts thereof are eligibly for local evaluation.</param>
        /// <param name="valueMapper">Mapper to map values from and to <see cref="DynamicObject"/>.</param>
        public ExpressionTranslatorContext(
            ITypeResolver? typeResolver = null,
            ITypeInfoProvider? typeInfoProvider = null,
            IIsKnownTypeProvider? isKnownTypeProvider = null,
            Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null,
            IDynamicObjectMapper? valueMapper = null)
        {
            IsKnownTypeProvider = new IsKnownTypeProviderDecorator(isKnownTypeProvider);
            TypeResolver = typeResolver ?? Aqua.TypeSystem.TypeResolver.Instance;
            TypeInfoProvider = typeInfoProvider ?? new TypeInfoProvider(false, false);
            CanBeEvaluatedLocally = canBeEvaluatedLocally;
            ValueMapper = valueMapper ?? new ExpressionTranslatorContextObjectMapper(this);
            NeedsMapping = value => !IsKnownTypeProvider.IsKnownType(value.CheckNotNull(nameof(value)).GetType());
        }

        /// <summary>
        /// Gets a defaut expression translator context that can be used to translate, process and prepare expressions for serialization.
        /// </summary>
        public static IExpressionTranslatorContext Default => new ExpressionTranslatorContext();

        /// <summary>
        /// Gets an expression translator context that does not translates any values other than expression and type information.
        /// </summary>
        /// <remarks>
        /// This context is typically used for in-memory translation and processing that does not include serialization of expressions.
        /// </remarks>
        public static IExpressionTranslatorContext NoMappingContext => new NoMappingContextImpl();

        /// <summary>
        /// Gets a provider for checking types to be known. Unknown types require mapping (i.e. substitution) on translating expressions.
        /// </summary>
        protected IIsKnownTypeProvider IsKnownTypeProvider { get; }

        /// <inheritdoc/>
        public ITypeResolver TypeResolver { get; }

        /// <inheritdoc/>
        public ITypeInfoProvider TypeInfoProvider { get; }

        /// <inheritdoc/>
        public virtual IDynamicObjectMapper ValueMapper { get; }

        /// <inheritdoc/>
        public Func<object, bool> NeedsMapping { get; }

        /// <inheritdoc/>
        public Func<SystemLinq.Expression, bool>? CanBeEvaluatedLocally { get; }

        /// <inheritdoc/>
        public virtual IExpressionTranslator ExpressionTranslator => new ExpressionTranslator(this);

        private static object? MapGroupToDynamicObjectGraphMethod(Type[] genericTypeArguments, object group)
            => _mapGroupToDynamicObjectGraphMethodDefinition
            .MakeGenericMethod(genericTypeArguments)
            .Invoke(null, new[] { group });

        private static Grouping<TKey, TElement> MapGroupToDynamicObjectGraph<TKey, TElement>(IGrouping<TKey, TElement> group)
            => group is Grouping<TKey, TElement> grouping
            ? grouping
            : new Grouping<TKey, TElement>
            {
                Key = group.Key,
                Elements = group.ToArray(),
            };
    }
}