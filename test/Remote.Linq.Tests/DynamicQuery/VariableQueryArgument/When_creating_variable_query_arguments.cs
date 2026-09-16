// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.DynamicQuery.VariableQueryArgument;

using VariableArgument = Remote.Linq.DynamicQuery.VariableQueryArgument;
using VariableArgumentList = Remote.Linq.DynamicQuery.VariableQueryArgumentList;

public class When_creating_variable_query_arguments
{
    [Fact]
    public void Should_infer_or_use_the_specified_value_type()
    {
        new VariableArgument(null, (Type)null).Type.ToType().ShouldBe(typeof(object));
        new VariableArgument(42, (Type)null).Type.ToType().ShouldBe(typeof(int));
        new VariableArgument(42, typeof(long)).Type.ToType().ShouldBe(typeof(long));
        new VariableArgument("value", (Type)null).ToString().ShouldBe("VariableQueryArgument(\"value\")");
    }

    [Fact]
    public void Should_infer_element_type_copy_values_and_render_lists()
    {
        var source = new List<int> { 1, 2 };
        var inferred = new VariableArgumentList(source, (Type)null);
        var explicitType = new VariableArgumentList(source, typeof(long));

        inferred.ElementType.ToType().ShouldBe(typeof(int));
        inferred.Values.ShouldBe([1, 2]);
        inferred.Values.ShouldNotBeSameAs(source);
        inferred.ToString().ShouldBe("VariableQueryArgumentListOfInt32[2]");
        explicitType.ElementType.ToType().ShouldBe(typeof(long));
        Should.Throw<ArgumentNullException>(() => new VariableArgumentList(null, (Type)null));
    }
}
