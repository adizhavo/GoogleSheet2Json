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
            builder.StartField("property");
            builder.SetField("some value");
            builder.EndField();
            AssertPropertyContainsOnlyText(builder.buildData.properties[0]);
        }

        [Test]
        public void PropertyContainesMapTest()
        {
            builder.StartBuild();
            builder.StartField("property");
            builder.StartMap();    
            builder.AddKey("key");
            builder.AddValue("value");
            builder.EndField();
            AssertPropertyContainsMap(builder.buildData.properties[0]);
        }

        [Test]
        public void PropertyContainesArrayOfMapsTest()
        {
            builder.StartBuild();
            builder.StartField("property");
            
            builder.StartMap();    
            builder.AddKey("key");
            builder.AddValue("value");
            
            builder.StartMap();    
            builder.AddKey("key");
            builder.AddValue("value");
            
            builder.EndField();

            AssertPropertyContainsArrayOfMaps(builder.buildData.properties[0]);
        }
        
        [Test]
        public void PropertyContainesCollectionTest()
        {
            builder.StartBuild();
            builder.StartField("property");
            
            builder.StartCollection();
            builder.AddCollectionElement("1");
            builder.AddCollectionElement("2");
            
            builder.EndField();

            AssertPropertyContainsCollection(builder.buildData.properties[0]);
        }

        [Test]
        public void PropertyContainesRangeOnlyOfNumericVariablesTest()
        {
            builder.StartBuild();
            builder.StartField("property");
            
            builder.SetField("1");
            builder.TryAddMinRange("-");
            builder.TryAddMaxRange("2");

            builder.EndField();

            AssertPropertyContainsMinMaxRange(builder.buildData.properties[0]);
        }

        [Test]
        public void PropertyContainesTextIfTryToAddNotNumericRangeTest()
        {
            builder.StartBuild();
            builder.StartField("property");
            
            builder.SetField("a");
            builder.TryAddMinRange("-");
            builder.TryAddMaxRange("b");

            builder.EndField();
            
            AssertPropertyContainsOnlyText(builder.buildData.properties[0]);
        }

        private void AssertPropertyContainsOnlyText(FieldNode fieldNode)
        {
            Assert.False(fieldNode.isArrayOfMaps);
            Assert.False(fieldNode.isMap);
            Assert.False(fieldNode.isCollection);
            Assert.IsNotEmpty(fieldNode.fieldValue);
            Assert.IsEmpty(fieldNode.value);
            Assert.IsEmpty(fieldNode.key);
            Assert.IsEmpty(fieldNode.values);
            Assert.IsEmpty(fieldNode.keys);
            Assert.IsEmpty(fieldNode.collectionValues);
            Assert.False(fieldNode.isRange);
            Assert.IsEmpty(fieldNode.min);
            Assert.IsEmpty(fieldNode.max);
        }
        
        private void AssertPropertyContainsMap(FieldNode fieldNode)
        {
            Assert.True(fieldNode.isMap);
            Assert.IsNotEmpty(fieldNode.key);
            Assert.IsNotEmpty(fieldNode.value);
            Assert.False(fieldNode.isArrayOfMaps);
            Assert.False(fieldNode.isCollection);
            Assert.IsEmpty(fieldNode.fieldValue);
            Assert.IsEmpty(fieldNode.values);
            Assert.IsEmpty(fieldNode.keys);
            Assert.IsEmpty(fieldNode.collectionValues);
            Assert.False(fieldNode.isRange);
            Assert.IsEmpty(fieldNode.min);
            Assert.IsEmpty(fieldNode.max);
        }
        
        private void AssertPropertyContainsArrayOfMaps(FieldNode fieldNode)
        {
            Assert.True(fieldNode.isArrayOfMaps);
            Assert.IsNotEmpty(fieldNode.keys);
            Assert.IsNotEmpty(fieldNode.values);
            Assert.False(fieldNode.isMap);
            Assert.False(fieldNode.isCollection);
            Assert.IsEmpty(fieldNode.fieldValue);
            Assert.IsEmpty(fieldNode.value);
            Assert.IsEmpty(fieldNode.key);
            Assert.IsEmpty(fieldNode.collectionValues);
            Assert.False(fieldNode.isRange);
            Assert.IsEmpty(fieldNode.min);
            Assert.IsEmpty(fieldNode.max);
        }
        
        private void AssertPropertyContainsCollection(FieldNode fieldNode)
        {
            Assert.True(fieldNode.isCollection);
            Assert.IsNotEmpty(fieldNode.collectionValues);
            Assert.IsEmpty(fieldNode.keys);
            Assert.IsEmpty(fieldNode.values);
            Assert.False(fieldNode.isMap);
            Assert.False(fieldNode.isArrayOfMaps);
            Assert.IsEmpty(fieldNode.fieldValue);
            Assert.IsEmpty(fieldNode.value);
            Assert.IsEmpty(fieldNode.key);
            Assert.False(fieldNode.isRange);
            Assert.IsEmpty(fieldNode.min);
            Assert.IsEmpty(fieldNode.max);
        }
        
        private void AssertPropertyContainsMinMaxRange(FieldNode fieldNode)
        {
            Assert.True(fieldNode.isRange);
            Assert.IsNotEmpty(fieldNode.min);
            Assert.IsNotEmpty(fieldNode.max);
            Assert.IsEmpty(fieldNode.keys);
            Assert.IsEmpty(fieldNode.values);
            Assert.False(fieldNode.isMap);
            Assert.False(fieldNode.isArrayOfMaps);
            Assert.False(fieldNode.isCollection);
            Assert.IsEmpty(fieldNode.fieldValue);
            Assert.IsEmpty(fieldNode.value);
            Assert.IsEmpty(fieldNode.key);
            Assert.IsEmpty(fieldNode.collectionValues);
        }
    }
}