// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.ExpressionTranslator.NoMappingContext;

using Aqua.Dynamic;
using Shouldly;
using System;
using Xunit;

public class When_calling_value_mapper_transformation_operations
{
    private IDynamicObjectMapper ValueMapper => ExpressionTranslatorContext.NoMappingContext.ValueMapper;

    [Fact]
    public void Mapping_context_should_provide_value_mapper()
    {
        ValueMapper.ShouldNotBeNull();
    }

    [Fact]
    public void Value_mapper_should_throw_on_map()
    {
        var ex = Should.Throw<NotSupportedException>(() => ValueMapper.Map(default(DynamicObject)));
        ex.Message.ShouldBe("Operation must not be called as no value mapping should occure.");
    }

    [Fact]
    public void Value_mapper_should_throw_on_map_object()
    {
        var ex = Should.Throw<NotSupportedException>(() => ValueMapper.MapObject(default(object)));
        ex.Message.ShouldBe("Operation must not be called as no value mapping should occure.");
    }
}