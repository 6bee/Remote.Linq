namespace Remote.Linq.Expressions
{
    using ProtoBuf.Meta;

    internal static class ExpressionTypeModel
    {
        public static RuntimeTypeModel ConfigureExpression(this RuntimeTypeModel typeModel)
        {
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

            return typeModel;
        }
    }
}
