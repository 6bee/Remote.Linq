// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.DynamicQuery;

using RemoteExpression = Remote.Linq.Expressions.Expression;
using SystemExpression = System.Linq.Expressions.Expression;

public interface IExpressionTranslator
{
    RemoteExpression TranslateExpression(SystemExpression expression);
}