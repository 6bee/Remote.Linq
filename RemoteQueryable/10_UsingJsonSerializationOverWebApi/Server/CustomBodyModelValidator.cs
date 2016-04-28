// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server
{
    using Common.Model;
    using System;
    using System.Web.Http.Validation;

    public class CustomBodyModelValidator : DefaultBodyModelValidator
    {
        public override bool ShouldValidateType(Type type)
        {
            return type != typeof(Query) && base.ShouldValidateType(type);
        }
    }
}
