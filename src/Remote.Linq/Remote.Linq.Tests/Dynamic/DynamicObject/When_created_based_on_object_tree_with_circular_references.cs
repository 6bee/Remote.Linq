// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Dynamic.DynamicObject
{
    using Remote.Linq.Dynamic;
    using Xunit;
    using Xunit.Should;

    public class When_created_based_on_object_tree_with_circular_references
    {
        abstract class BaseClass
        {
            public int Id { get; set; }
        }

        class SubClassA : BaseClass
        {
            public string Name { get; set; }
            public SubClassB SubClassBReference { get; set; }
        }

        class SubClassB : BaseClass
        {
            public BaseClass BaseClassReference { get; set; }
        }

        [Fact]
        public void Should_reflect_object_tree_onlcuding_circular_references()
        {
            var sourceB1 = new SubClassB { Id = 1 };
            var sourceB2 = new SubClassB { Id = 2 };
            var sourceA = new SubClassA { Id = 3 };

            sourceB1.BaseClassReference = sourceB1;
            sourceB2.BaseClassReference = sourceB1;
            sourceA.SubClassBReference = sourceB2;

            var dynamicObject = new DynamicObject(sourceA);

            dynamicObject["Id"].ShouldBe(sourceA.Id);

            var referenceFromAToB2 = dynamicObject["SubClassBReference"] as DynamicObject;
            referenceFromAToB2.ShouldBeInstanceOf(typeof(DynamicObject));
            referenceFromAToB2["Id"].ShouldBe(sourceB2.Id);

            var referenceFromB2ToB1 = referenceFromAToB2["BaseClassReference"] as DynamicObject;
            referenceFromB2ToB1.ShouldBeInstanceOf(typeof(DynamicObject));
            referenceFromB2ToB1["Id"].ShouldBe(sourceB1.Id);

            referenceFromB2ToB1["BaseClassReference"].ShouldBeSameAs(referenceFromB2ToB1);
        }
    }
}
