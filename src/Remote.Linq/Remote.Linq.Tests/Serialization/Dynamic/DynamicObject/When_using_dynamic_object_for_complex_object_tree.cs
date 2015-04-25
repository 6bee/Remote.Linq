namespace Remote.Linq.Tests.Serialization.Dynamic.DynamicObject
{
    using Remote.Linq.Dynamic;
    using System;
    using Xunit;
    using Xunit.Should;

    public class When_using_dynamic_object_for_complex_object_tree
    {
        const double DoubleValue = 1.234567e-987;
        const string StringValue = "eleven";

        DynamicObject serializedObject;

        public When_using_dynamic_object_for_complex_object_tree()
        {
            var originalObject = new DynamicObject()
            {
                { "DoubleValue", DoubleValue },
                { 
                    "Reference", new DynamicObject()
                    {
                        { "StringValue", StringValue },
                    }
                },
            };

            serializedObject = originalObject.Serialize();
        }

        [Fact]
        public void Clone_should_contain_simple_numeric_property()
        {
            serializedObject["DoubleValue"].ShouldBe(DoubleValue);
        }

        [Fact]
        public void Clone_should_contain_nested_string_property()
        {
            var nestedObject = serializedObject["Reference"] as DynamicObject;

            nestedObject["StringValue"].ShouldBe(StringValue);
        }
    }
}
