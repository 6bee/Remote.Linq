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

    public class ExpressionTranslatorContext : IExpressionTranslatorContext
    {
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
                    obj = _mapGroupToDynamicObjectGraphMethod(genericTypeArguments!).Invoke(null, new[] { obj });
                }

                return base.MapToDynamicObjectGraph(obj, setTypeInformation);
            }
        }

        private const BindingFlags PrivateStatic = BindingFlags.NonPublic | BindingFlags.Static;

        private static readonly MethodInfo _mapGroupToDynamicObjectGraphMethodDefinition =
            typeof(ExpressionTranslatorContext).GetMethod(nameof(MapGroupToDynamicObjectGraph), PrivateStatic) !;

        private static readonly Func<Type[], MethodInfo> _mapGroupToDynamicObjectGraphMethod =
            genericTypeArguments => _mapGroupToDynamicObjectGraphMethodDefinition.MakeGenericMethod(genericTypeArguments);

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
                typeof(IRemoteResource),
            };

        private static readonly Type[] _excludeFromUnmappedTypes = new[]
            {
                typeof(EnumerableQuery),
            };

        private readonly IIsKnownTypeProvider _isKnownTypeProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionTranslatorContext"/> class.
        /// </summary>
        public ExpressionTranslatorContext(ITypeInfoProvider? typeInfoProvider, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally)
            : this(null, typeInfoProvider, null, canBeEvaluatedLocally)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionTranslatorContext"/> class.
        /// </summary>
        public ExpressionTranslatorContext(ITypeResolver? typeResolver, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally)
            : this(typeResolver, null, null, canBeEvaluatedLocally)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionTranslatorContext"/> class.
        /// </summary>
        public ExpressionTranslatorContext(
            ITypeResolver? typeResolver = null,
            ITypeInfoProvider? typeInfoProvider = null,
            IIsKnownTypeProvider? isKnownTypeProvider = null,
            Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null,
            IDynamicObjectMapper? valueMapper = null)
        {
            TypeResolver = typeResolver ?? Aqua.TypeSystem.TypeResolver.Instance;
            TypeInfoProvider = typeInfoProvider ?? new TypeInfoProvider(false, false);
            _isKnownTypeProvider = new IsKnownTypeProviderDecorator(isKnownTypeProvider);
            CanBeEvaluatedLocally = canBeEvaluatedLocally;
            ValueMapper = valueMapper
                ?? CreateObjectMapper(TypeResolver, TypeInfoProvider, _isKnownTypeProvider)
                ?? throw new RemoteLinqException($"Method {nameof(CreateObjectMapper)} must not return null.");
            NeedsMapping = value => !_isKnownTypeProvider.IsKnownType(value.CheckNotNull(nameof(value)).GetType());
        }

        protected virtual IDynamicObjectMapper CreateObjectMapper(ITypeResolver typeResolver, ITypeInfoProvider typeInfoProvider, IIsKnownTypeProvider isKnownTypeProvider)
            => new ExpressionTranslatorContextObjectMapper(typeResolver, typeInfoProvider, isKnownTypeProvider);

        public ITypeResolver TypeResolver { get; }

        public ITypeInfoProvider TypeInfoProvider { get; }

        public IDynamicObjectMapper ValueMapper { get; }

        public Func<object, bool> NeedsMapping { get; }

        public Func<SystemLinq.Expression, bool>? CanBeEvaluatedLocally { get; }

        private static Grouping<TKey, TElement> MapGroupToDynamicObjectGraph<TKey, TElement>(IGrouping<TKey, TElement> group)
            => (group as Grouping<TKey, TElement>) ??
                new Grouping<TKey, TElement>
                {
                    Key = group.Key,
                    Elements = group.ToArray(),
                };
    }
}