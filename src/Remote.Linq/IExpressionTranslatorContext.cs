// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using Aqua.Dynamic;
    using Aqua.TypeSystem;
    using System;
    using SystemLinq = System.Linq.Expressions;

    public interface IExpressionTranslatorContext : IExpressionFromRemoteLinqContext, IExpressionToRemoteLinqContext
    {
    }

    public interface IExpressionFromRemoteLinqContext : IExpressionValueMapperProvider
    {
        ITypeResolver TypeResolver { get; }
    }

    public interface IExpressionToRemoteLinqContext : IExpressionValueMapperProvider
    {
        ITypeInfoProvider TypeInfoProvider { get; }

        Func<object, bool> NeedsMapping { get; }

        Func<SystemLinq.Expression, bool>? CanBeEvaluatedLocally { get; }
    }

    public interface IExpressionValueMapperProvider
    {
        IDynamicObjectMapper ValueMapper { get; }
    }
}