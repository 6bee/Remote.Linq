// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Text.Json.Converters
{
    using Aqua.Text.Json;
    using Remote.Linq.Expressions;
    using System;

    public sealed class ExpressionConverter : ExpressionConverter<Expression>
    {
        public ExpressionConverter(KnownTypesRegistry knownTypesRegistry)
            : base(knownTypesRegistry)
        {
        }

        public override bool CanConvert(Type typeToConvert)
            => typeof(Expression).IsAssignableFrom(typeToConvert);
    }
}