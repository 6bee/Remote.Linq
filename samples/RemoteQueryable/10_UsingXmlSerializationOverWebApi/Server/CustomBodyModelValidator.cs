// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server
{
    using Remote.Linq.Expressions;
    using System;
    using System.Web.Http.Validation;

    public class CustomBodyModelValidator : DefaultBodyModelValidator
    {
        public override bool ShouldValidateType(Type type)
        {
            return !typeof(Expression).IsAssignableFrom(type) && base.ShouldValidateType(type);
        }
    }
}
