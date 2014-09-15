// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Dynamic.DynamicObject
{
    using Remote.Linq.Dynamic;
    using Remote.Linq.TypeSystem;
    using System;
    using System.Collections.Generic;
    using Xunit;
    using Xunit.Should;

    // TODO: refactor tests
    public class DynamicObjectTests
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

        class ClassWithIndexerAndItemProperty
        {
            private readonly IDictionary<string, object> _data = new Dictionary<string, object>();

            [System.Runtime.CompilerServices.IndexerName("MyIndexer")]
            public object this[string key]
            {
                get
                {
                    return _data[key];
                }
                set
                {
                    _data[key] = value;
                }
            }

            public string Item { get; set; }
        }

        class ClassWithDictionaryProperty
        {
            public IDictionary<string, string> Dictionary { get; set; }
        }

        [Fact]
        public void ShouldBeEmptyWhenUsingParameterlessConstructor()
        {
            var dynamicObject = new DynamicObject();
            dynamicObject.MemberCount.ShouldBe(0);
        }

        [Fact]
        public void ShouldNotSetTypePropertyWhenUsingParameterlessConstructor()
        {
            var dynamicObject = new DynamicObject();
            dynamicObject.Type.ShouldBeNull();
        }

        [Fact]
        public void ShouldSetTypePropertyWhenPassingTypeToConstructor()
        {
            var dynamicObject = new DynamicObject(typeof(ClassWithIndexerAndItemProperty));

            dynamicObject.Type.Type.ShouldBe(typeof(ClassWithIndexerAndItemProperty));
        }

        [Fact]
        public void ShouldSetTypePropertyWhenPassingTypeInfoToConstructor()
        {
            var typeInfo = new TypeInfo(typeof(ClassWithIndexerAndItemProperty));
            var dynamicObject = new DynamicObject(typeInfo);

            dynamicObject.Type.ShouldBe(typeInfo);
        }

        [Fact]
        public void ShouldInitializeWithCollectionInitializer()
        {
            var value1 = "Value1";
            var value2 = new object();
            var value3 = DateTime.Now;

            var dynamicObject = new DynamicObject
            {
                { "Property1",  value1 },
                { "Property2",  value2 },
                { "P 3", value3 },
            };

            dynamicObject.MemberCount.ShouldBe(3);

            dynamicObject.MemberNames.ShouldContain("Property1");
            dynamicObject.MemberNames.ShouldContain("Property2");
            dynamicObject.MemberNames.ShouldContain("P 3");

            dynamicObject["Property1"].ShouldBe(value1);
            dynamicObject["Property2"].ShouldBe(value2);
            dynamicObject["P 3"].ShouldBe(value3);
        }

        [Fact]
        public void ShouldSupportCyclcReferences()
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

        [Fact]
        public void ShouldCreateDynamicObjectBasedOnSourceObjectWithWithIndexer()
        {
            var key1 = "K1";
            var value1 = new object();

            var sourceObject = new ClassWithIndexerAndItemProperty();
            sourceObject.Item = "ItemValue1";
            sourceObject[key1] = value1;


            var dynamicObject = new DynamicObject(sourceObject);

            Assert.Equal(1, dynamicObject.MemberCount);

            Assert.Contains("Item", dynamicObject.MemberNames);

            Assert.Equal("ItemValue1", dynamicObject["Item"]);

            Assert.NotNull(dynamicObject.Type);
            Assert.Equal(typeof(ClassWithIndexerAndItemProperty), dynamicObject.Type.Type);
        }

        [Fact]
        public void ShouldCreateObjectWithIndexerBasedOnDynamicObject()
        {
            var dynamicObject = new DynamicObject
            {
                {"Item", "ItemValue1"}
            };

            var obj = dynamicObject.CreateObject<ClassWithIndexerAndItemProperty>();

            Assert.Equal("ItemValue1", obj.Item);
        }

        [Fact]
        public void ShouldCreateDynamicObjectBasedOnObjectWithDictyonaryProperty()
        {
            var source = new ClassWithDictionaryProperty
            {
                Dictionary = new Dictionary<string, string>
                {
                    { "K1", "V1" },
                    { "K2", "V2" },
                    { "K3", "V3" },
                }
            };

            var dynamicObject = new DynamicObject(source);
            dynamicObject.MemberCount.ShouldBe(1);
        }

        [Fact]
        public void ShouldCreateObjectWithDictyonaryPropertyBasedOnDynamicObject()
        {
            var dictionary = new Dictionary<string, string>
            {
                { "K1", "V1" },
                { "K2", "V2" },
                { "K3", "V3" },
            };

            var dynamicObject = new DynamicObject
            {
                { "Dictionary", dictionary }
            };

            var obj = dynamicObject.CreateObject<ClassWithDictionaryProperty>();

            obj.Dictionary.Keys.ShouldContain("K1");
            obj.Dictionary.Keys.ShouldContain("K2");
            obj.Dictionary.Keys.ShouldContain("K3");

            obj.Dictionary["K1"].ShouldBe("V1");
            obj.Dictionary["K2"].ShouldBe("V2");
            obj.Dictionary["K3"].ShouldBe("V3");
        }
    }
}

