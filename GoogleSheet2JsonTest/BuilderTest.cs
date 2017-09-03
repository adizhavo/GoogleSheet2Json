using GoogleSheet2Json;
using NUnit.Framework;

namespace GoogleSheet2JsonTest
{
    [TestFixture]
    public class BuilderTest
    {
        private Builder builder;

        [SetUp]
        public void SetUp()
        {
            builder = new Builder();
        }

        [Test]
        public void PropertyContainesOnlyTextTest()
        {
            builder.StartBuild();
            builder.StartProperty("property");
            builder.AddField("some value");
            builder.EndProperty();
            AssertPropertyContainsOnlyText(builder.buildData.properties[0]);
        }

        [Test]
        public void PropertyContainesMapTest()
        {
            builder.StartBuild();
            builder.StartProperty("property");
            builder.StartMap();    
            builder.AddKey("key");
            builder.AddValue("value");
            builder.EndProperty();
            AssertPropertyContainsMap(builder.buildData.properties[0]);
        }

        [Test]
        public void PropertyContainesArrayOfMapsTest()
        {
            builder.StartBuild();
            builder.StartProperty("property");
            
            builder.StartMap();    
            builder.AddKey("key");
            builder.AddValue("value");
            
            builder.StartMap();    
            builder.AddKey("key");
            builder.AddValue("value");
            
            builder.EndProperty();

            AssertPropertyContainsArrayOfMaps(builder.buildData.properties[0]);
        }
        
        [Test]
        public void PropertyContainesCollectionTest()
        {
            builder.StartBuild();
            builder.StartProperty("property");
            
            builder.StartCollection();
            builder.AddCollectionElement("1");
            builder.AddCollectionElement("2");
            
            builder.EndProperty();

            AssertPropertyContainsCollection(builder.buildData.properties[0]);
        }

        [Test]
        public void PropertyContainesRangeOnlyOfNumericVariablesTest()
        {
            builder.StartBuild();
            builder.StartProperty("property");
            
            builder.AddField("1");
            builder.TryAddMinRange("-");
            builder.TryAddMaxRange("2");

            builder.EndProperty();

            AssertPropertyContainsMinMaxRange(builder.buildData.properties[0]);
        }

        [Test]
        public void PropertyContainesTextIfTryToAddNotNumericRangeTest()
        {
            builder.StartBuild();
            builder.StartProperty("property");
            
            builder.AddField("a");
            builder.TryAddMinRange("-");
            builder.TryAddMaxRange("b");

            builder.EndProperty();
            
            AssertPropertyContainsOnlyText(builder.buildData.properties[0]);
        }

        private void AssertPropertyContainsOnlyText(PropertyNode propertyNode)
        {
            Assert.False(propertyNode.isArrayOfMaps);
            Assert.False(propertyNode.isMap);
            Assert.False(propertyNode.isCollection);
            Assert.IsNotEmpty(propertyNode.fieldValue);
            Assert.IsEmpty(propertyNode.value);
            Assert.IsEmpty(propertyNode.key);
            Assert.IsEmpty(propertyNode.values);
            Assert.IsEmpty(propertyNode.keys);
            Assert.IsEmpty(propertyNode.collectionValues);
            Assert.False(propertyNode.isRange);
            Assert.IsEmpty(propertyNode.min);
            Assert.IsEmpty(propertyNode.max);
        }
        
        private void AssertPropertyContainsMap(PropertyNode propertyNode)
        {
            Assert.True(propertyNode.isMap);
            Assert.IsNotEmpty(propertyNode.key);
            Assert.IsNotEmpty(propertyNode.value);
            Assert.False(propertyNode.isArrayOfMaps);
            Assert.False(propertyNode.isCollection);
            Assert.IsEmpty(propertyNode.fieldValue);
            Assert.IsEmpty(propertyNode.values);
            Assert.IsEmpty(propertyNode.keys);
            Assert.IsEmpty(propertyNode.collectionValues);
            Assert.False(propertyNode.isRange);
            Assert.IsEmpty(propertyNode.min);
            Assert.IsEmpty(propertyNode.max);
        }
        
        private void AssertPropertyContainsArrayOfMaps(PropertyNode propertyNode)
        {
            Assert.True(propertyNode.isArrayOfMaps);
            Assert.IsNotEmpty(propertyNode.keys);
            Assert.IsNotEmpty(propertyNode.values);
            Assert.False(propertyNode.isMap);
            Assert.False(propertyNode.isCollection);
            Assert.IsEmpty(propertyNode.fieldValue);
            Assert.IsEmpty(propertyNode.value);
            Assert.IsEmpty(propertyNode.key);
            Assert.IsEmpty(propertyNode.collectionValues);
            Assert.False(propertyNode.isRange);
            Assert.IsEmpty(propertyNode.min);
            Assert.IsEmpty(propertyNode.max);
        }
        
        private void AssertPropertyContainsCollection(PropertyNode propertyNode)
        {
            Assert.True(propertyNode.isCollection);
            Assert.IsNotEmpty(propertyNode.collectionValues);
            Assert.IsEmpty(propertyNode.keys);
            Assert.IsEmpty(propertyNode.values);
            Assert.False(propertyNode.isMap);
            Assert.False(propertyNode.isArrayOfMaps);
            Assert.IsEmpty(propertyNode.fieldValue);
            Assert.IsEmpty(propertyNode.value);
            Assert.IsEmpty(propertyNode.key);
            Assert.False(propertyNode.isRange);
            Assert.IsEmpty(propertyNode.min);
            Assert.IsEmpty(propertyNode.max);
        }
        
        private void AssertPropertyContainsMinMaxRange(PropertyNode propertyNode)
        {
            Assert.True(propertyNode.isRange);
            Assert.IsNotEmpty(propertyNode.min);
            Assert.IsNotEmpty(propertyNode.max);
            Assert.IsEmpty(propertyNode.keys);
            Assert.IsEmpty(propertyNode.values);
            Assert.False(propertyNode.isMap);
            Assert.False(propertyNode.isArrayOfMaps);
            Assert.False(propertyNode.isCollection);
            Assert.IsEmpty(propertyNode.fieldValue);
            Assert.IsEmpty(propertyNode.value);
            Assert.IsEmpty(propertyNode.key);
            Assert.IsEmpty(propertyNode.collectionValues);
        }
    }
}