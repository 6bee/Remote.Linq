// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Text.Json.Converters;

using Aqua.Text.Json;
using Aqua.Text.Json.Converters;
using Aqua.TypeSystem;
using Remote.Linq.DynamicQuery;
using System.Collections.Generic;
using System.Text.Json;

public sealed class VariableQueryArgumentListConverter : ObjectConverter<VariableQueryArgumentList>
{
    public VariableQueryArgumentListConverter(KnownTypesRegistry knownTypesRegistry)
        : base(knownTypesRegistry)
    {
    }

    protected override void ReadObjectProperties(ref Utf8JsonReader reader, VariableQueryArgumentList result, Dictionary<string, Property> properties, JsonSerializerOptions options)
    {
        reader.AssertProperty(nameof(VariableQueryArgumentList.ElementType));
        var elementTypeInfo = reader.Read<TypeInfo>(options) ?? throw reader.CreateException($"{nameof(VariableQueryArgumentList.ElementType)} must not be null.");

        void SetResult(ref Utf8JsonReader reader, List<object?>? values = null)
        {
            reader.AssertEndObject();
            result.ElementType = elementTypeInfo;
            result.Values = values ?? new List<object?>();
        }

        reader.Advance();
        if (reader.TokenType is JsonTokenType.Null)
        {
            SetResult(ref reader);
            return;
        }

        if (reader.TokenType is not JsonTokenType.StartArray)
        {
            throw reader.CreateException($"Expected array");
        }

        bool TryReadNextItem(ref Utf8JsonReader reader, out object? value)
        {
            if (!reader.TryRead(elementTypeInfo, options, out value))
            {
                if (reader.TokenType is JsonTokenType.EndArray)
                {
                    return false;
                }

                throw reader.CreateException("Unexpected token structure.");
            }

            return true;
        }

        var values = new List<object?>();
        while (TryReadNextItem(ref reader, out object? value))
        {
            values.Add(value);
        }

        SetResult(ref reader, values);
    }

    protected override void WriteObjectProperties(Utf8JsonWriter writer, VariableQueryArgumentList instance, IReadOnlyCollection<Property> properties, JsonSerializerOptions options)
    {
        writer.WritePropertyName(nameof(VariableQueryArgumentList.ElementType));
        writer.Serialize(instance.ElementType, options);

        writer.WritePropertyName(nameof(VariableQueryArgumentList.Values));
        writer.Serialize(instance.Values, options);
    }
}