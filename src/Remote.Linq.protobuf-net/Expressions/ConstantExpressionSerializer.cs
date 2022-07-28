// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.ProtoBuf.Expressions
{
    using global::ProtoBuf;
    using global::ProtoBuf.Serializers;
    using Remote.Linq.Expressions;

    public sealed class ConstantExpressionSerializer : ISerializer<ConstantExpression>, ISubTypeSerializer<ConstantExpression>
    {
        public SerializerFeatures Features => SerializerFeatures.CategoryMessage | SerializerFeatures.WireTypeString;

        public ConstantExpression Read(ref ProtoReader.State state, ConstantExpression value)
        {
            var serializer = state.GetSerializer<ConstantExpressionSurrogate>();

            var dto2 = serializer.Read(ref state, default!);
            var value2 = ConstantExpressionSurrogate.Convert(dto2);
            return value2!;
        }

        public ConstantExpression ReadSubType(ref ProtoReader.State state, SubTypeState<ConstantExpression> value)
            => Read(ref state, default!);

        public void Write(ref ProtoWriter.State state, ConstantExpression value)
        {
            var serializer = state.GetSerializer<ConstantExpressionSurrogate>();

            var dto = ConstantExpressionSurrogate.Convert(value);
            serializer.Write(ref state, dto!);
        }

        public void WriteSubType(ref ProtoWriter.State state, ConstantExpression value)
            => Write(ref state, value);
    }
}
