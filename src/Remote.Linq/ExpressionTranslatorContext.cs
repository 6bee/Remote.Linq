// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using Aqua.Dynamic;
    using Aqua.EnumerableExtensions;
    using Aqua.TypeSystem;
    using Remote.Linq.DynamicQuery;
    using System;
    using System.Linq;
    using System.Threading;
    using SystemLinq = System.Linq.Expressions;

    public class ExpressionTranslatorContext : DynamicObjectMapper, IExpressionTranslatorContext
    {
        private sealed class IsKnownTypeProvider : IIsKnownTypeProvider
        {
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

                return false;
            }

            private static bool IsUnmappedType(Type type)
            {
                var t = type.IsGenericType ? type.GetGenericTypeDefinition() : type;
                return _unmappedTypes.Any(x => x.IsAssignableFrom(t))
                    && !_excludeFromUnmappedTypes.Any(x => x.IsAssignableFrom(t));
            }
        }

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
            .ToDictionary(x => x, x => (object?)null)
            .ContainsKey;

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

        internal ExpressionTranslatorContext(ITypeInfoProvider? typeInfoProvider, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally)
            : this(null, typeInfoProvider, null)
            => CanBeEvaluatedLocally = canBeEvaluatedLocally;

        internal ExpressionTranslatorContext(ITypeResolver typeResolver)
            : this(typeResolver, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionTranslatorContext"/> class.
        /// </summary>
        public ExpressionTranslatorContext(ITypeResolver? typeResolver = null, ITypeInfoProvider? typeInfoProvider = null, IIsKnownTypeProvider? isKnownTypeProvider = null)
            : this(
                0,
                typeResolver ?? Aqua.TypeSystem.TypeResolver.Instance,
                typeInfoProvider ?? new TypeInfoProvider(false, false),
                isKnownTypeProvider ?? new IsKnownTypeProvider())
        {
        }

        private ExpressionTranslatorContext(int n, ITypeResolver typeResolver, ITypeInfoProvider typeInfoProvider, IIsKnownTypeProvider isKnownTypeProvider)
            : base(
                typeResolver,
                typeInfoProvider,
                isKnownTypeProvider: isKnownTypeProvider)
        {
            TypeResolver = typeResolver;
            TypeInfoProvider = typeInfoProvider;
            _isKnownTypeProvider = isKnownTypeProvider;
            NeedsMapping = value => !_isKnownTypeProvider.IsKnownType(value.CheckNotNull(nameof(value)).GetType());
        }

        public ITypeResolver TypeResolver { get; }

        public ITypeInfoProvider TypeInfoProvider { get; }

        public IDynamicObjectMapper ValueMapper => this;

        public virtual Func<object, bool> NeedsMapping { get; }

        public virtual Func<SystemLinq.Expression, bool>? CanBeEvaluatedLocally { get; }
    }
}