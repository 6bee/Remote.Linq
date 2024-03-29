﻿// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Newtonsoft.Json.Converters;

using Aqua.Newtonsoft.Json;
using Aqua.Newtonsoft.Json.Converters;
using Aqua.TypeSystem;
using global::Newtonsoft.Json;
using Remote.Linq.DynamicQuery;
using System.Collections.Generic;

public sealed class VariableQueryArgumentListConverter : ObjectConverter<VariableQueryArgumentList>
{
    public VariableQueryArgumentListConverter(KnownTypesRegistry knownTypeRegistry)
        : base(knownTypeRegistry)
    {
    }

    protected override void ReadObjectProperties(JsonReader reader, VariableQueryArgumentList result, Dictionary<string, Property> properties, JsonSerializer serializer)
    {
        TypeInfo? elementTypeInfo;
        void SetResult(List<object?>? values = null)
        {
            reader.AssertEndObject();
            result.CheckNotNull().ElementType = elementTypeInfo;
            result.Values = values ?? new List<object?>();
        }

        reader.CheckNotNull().AssertProperty(nameof(VariableQueryArgumentList.ElementType));
        elementTypeInfo = reader.Read<TypeInfo>(serializer) ?? throw reader.CreateException($"{nameof(VariableQueryArgumentList.ElementType)} must not be null.");

        reader.Advance();
        if (reader.TokenType is JsonToken.Null)
        {
            SetResult();
            return;
        }

        if (reader.TokenType is not JsonToken.StartArray)
        {
            throw reader.CreateException($"Expected array");
        }

        bool TryReadNextItem(out object? value)
        {
            if (!reader.TryRead(elementTypeInfo!, serializer, out value))
            {
                if (reader.TokenType is JsonToken.EndArray)
                {
                    return false;
                }

                throw reader.CreateException("Unexpected token structure.");
            }

            return true;
        }

        var values = new List<object?>();
        while (TryReadNextItem(out object? value))
        {
            values.Add(value);
        }

        SetResult(values);
    }

    protected override void WriteObjectProperties(JsonWriter writer, VariableQueryArgumentList instance, IReadOnlyCollection<Property> properties, JsonSerializer serializer)
    {
        writer.CheckNotNull().WritePropertyName(nameof(VariableQueryArgumentList.ElementType));
        serializer.CheckNotNull().Serialize(writer, instance.CheckNotNull().ElementType);

        writer.WritePropertyName(nameof(VariableQueryArgumentList.Values));
        serializer.Serialize(writer, instance.Values, instance.ElementType?.ToType());
    }
}