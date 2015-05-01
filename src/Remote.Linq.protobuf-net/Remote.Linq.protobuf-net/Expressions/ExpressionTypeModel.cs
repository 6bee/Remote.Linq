// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions
{
    using System.Linq;
    using ProtoBuf.Meta;

    internal static class ExpressionTypeModel
    {
        public static RuntimeTypeModel ConfigureExpression(this RuntimeTypeModel typeModel)
        {
            typeModel.Add(typeof(UnaryOperator), true);
            typeModel.Add(typeof(BinaryOperator), true);

            var baseType = typeModel[typeof(Expression)];
            var fieldNumber = 100;

            baseType.AddSubType(++fieldNumber, typeof(BinaryExpression));
            baseType.AddSubType(++fieldNumber, typeof(CollectionExpression));
            baseType.AddSubType(++fieldNumber, typeof(ConditionalExpression));
            baseType.AddSubType(++fieldNumber, typeof(ConstantExpression));
            baseType.AddSubType(++fieldNumber, typeof(ConversionExpression));
            baseType.AddSubType(++fieldNumber, typeof(LambdaExpression));
            baseType.AddSubType(++fieldNumber, typeof(ListInitExpression));
            baseType.AddSubType(++fieldNumber, typeof(MemberExpression));
            baseType.AddSubType(++fieldNumber, typeof(MemberInitExpression));
            baseType.AddSubType(++fieldNumber, typeof(MethodCallExpression));
            baseType.AddSubType(++fieldNumber, typeof(NewExpression));
            baseType.AddSubType(++fieldNumber, typeof(NewArrayExpression));
            baseType.AddSubType(++fieldNumber, typeof(ParameterExpression));
            baseType.AddSubType(++fieldNumber, typeof(UnaryExpression));

            typeModel.GetType<BinaryExpression>()
                .GetMember("LeftOperand")
                .MakeDynamicType();

            typeModel.GetType<BinaryExpression>()
                .GetMember("RightOperand")
                .MakeDynamicType();


            typeModel.GetType<ConditionalExpression>()
                .GetMember("Test")
                .MakeDynamicType();

            typeModel.GetType<ConditionalExpression>()
                .GetMember("IfTrue")
                .MakeDynamicType();

            typeModel.GetType<ConditionalExpression>()
                .GetMember("IfFalse")
                .MakeDynamicType();


            typeModel.GetType<ConstantExpression>()
                .GetMember("Value")
                .MakeDynamicType();


            typeModel.GetType<ConversionExpression>()
                .GetMember("Operand")
                .MakeDynamicType();


            typeModel.GetType<ElementInit>()
                .GetMember("Arguments")
                .MakeDynamicType();

            
            typeModel.GetType<LambdaExpression>()
                .GetMember("Expression")
                .MakeDynamicType();

            // TODO: there are more...

            return typeModel;
        }
    }
}
