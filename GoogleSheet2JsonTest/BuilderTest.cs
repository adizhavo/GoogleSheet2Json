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
        public void FieldContainesOnlyTextTest()
        {
            builder.StartBuild();
            builder.StartProperty();
            builder.StartField("property");
            builder.SetField("some value");
            builder.EndField();
            builder.EndProperty();
            
            AssertFieldContainsOnlyText(builder.BuildData.properties[0].fields[0]);
        }

        [Test]
        public void FieldContainesMapTest()
        {
            builder.StartBuild();
            builder.StartProperty();
            builder.StartField("property");
            builder.StartMap();    
            builder.AddKey("key");
            builder.AddValue("value");
            builder.EndField();
            builder.EndProperty();
            
            AssertFieldContainsMap(builder.BuildData.properties[0].fields[0]);
        }

        [Test]
        public void FieldContainesArrayOfMapsTest()
        {
            builder.StartBuild();
            builder.StartProperty();
            builder.StartField("property");
            
            builder.StartMap();    
            builder.AddKey("key");
            builder.AddValue("value");
            
            builder.StartMap();    
            builder.AddKey("key");
            builder.AddValue("value");
            
            builder.EndField();
            builder.EndProperty();

            AssertFieldContainsArrayOfMaps(builder.BuildData.properties[0].fields[0]);
        }
        
        [Test]
        public void FieldContainesCollectionTest()
        {
            builder.StartBuild();
            builder.StartProperty();
            builder.StartField("property");
            
            builder.StartCollection();
            builder.AddCollectionElement("1");
            builder.AddCollectionElement("2");
            
            builder.EndField();
            builder.EndProperty();

            AssertFieldContainsCollection(builder.BuildData.properties[0].fields[0]);
        }

        [Test]
        public void FieldContainesRangeOnlyOfNumericVariablesTest()
        {
            builder.StartBuild();
            builder.StartProperty();
            builder.StartField("property");
            
            builder.SetField("1");
            builder.TryAddMinRange("-");
            builder.TryAddMaxRange("2");

            builder.EndField();
            builder.EndProperty();

            AssertFieldContainsMinMaxRange(builder.BuildData.properties[0].fields[0]);
        }

        [Test]
        public void FieldContainesTextIfTryToAddNotNumericRangeTest()
        {
            builder.StartBuild();
            builder.StartProperty();
            builder.StartField("property");
            
            builder.SetField("a");
            builder.TryAddMinRange("-");
            builder.TryAddMaxRange("b");

            builder.EndField();
            builder.EndProperty();
            
            AssertFieldContainsOnlyText(builder.BuildData.properties[0].fields[0]);
        }

        private void AssertFieldContainsOnlyText(FieldNode fieldNode)
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
        
        private void AssertFieldContainsMap(FieldNode fieldNode)
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
        
        private void AssertFieldContainsArrayOfMaps(FieldNode fieldNode)
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
        
        private void AssertFieldContainsCollection(FieldNode fieldNode)
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
        
        private void AssertFieldContainsMinMaxRange(FieldNode fieldNode)
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