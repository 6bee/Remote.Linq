// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.ProtoBuf.Expressions
{
    using Aqua.ProtoBuf;
    using global::ProtoBuf.Meta;
    using Remote.Linq.Expressions;

    internal static class ExpressionsTypeModel
    {
        public static RuntimeTypeModel ConfigureRemoteLinqExpressionTypes(this RuntimeTypeModel typeModel)
        {
            var n = 5;
            typeModel.RegisterBaseAndSubtypes<MemberBinding>(ref n);

            n = 10;
            typeModel.RegisterBaseAndSubtypes<Expression>(ref n);

            typeModel.GetType<ConstantExpression>().SetSurrogate<ConstantExpressionSurrogate>();

            return typeModel;
        }
    }
}
