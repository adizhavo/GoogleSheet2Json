using GoogleSheet2Json;
using NUnit.Framework;

namespace GoogleSheet2JsonTest
{
    public class ParserTest
    {
        private MockBuilder mockBuilder;
        private Parser parser;

        [SetUp]
        public void SetUp()
        {
            mockBuilder = new MockBuilder();
            parser = new Parser(mockBuilder);
        }

        [Test]
        public void ParseSetupTest()
        {
            parser.StartArrayOfObjects();
            parser.Name("root");
            parser.StartProperty();
            parser.EndProperty();
            parser.End();
            
            Assert.AreEqual("s rn s_p e_p eb", mockBuilder.buildPrint);
        }

        [Test]
        public void ParsePropertyDefinitionTest()
        {
            parser.StartArrayOfObjects();
            parser.Name("root");
            parser.StartProperty();
            parser.StartField();
            parser.Name("field definition");
            parser.EndField();
            parser.EndProperty();
            parser.End();
            
            Assert.AreEqual("s rn s_p sp ep e_p eb", mockBuilder.buildPrint);
        }
        
        [Test]
        public void ParsePropertyValueTest()
        {
            parser.StartArrayOfObjects();
            parser.Name("root");
            parser.StartProperty();
            parser.StartField();
            parser.Name("field definition");
            parser.Name("field value");
            parser.EndField();
            parser.EndProperty();
            parser.End();
            
            Assert.AreEqual("s rn s_p sp f ep e_p eb", mockBuilder.buildPrint);
        }
        
        [Test]
        public void ParsePropertyValueWithSymbolsTest()
        {
            parser.StartArrayOfObjects();
            parser.Name("root");
            parser.StartProperty();
            parser.StartField();
            parser.Name("field definition");
            parser.Name("field valie");
            parser.Comma();
            parser.Range();
            parser.CloseBrace();
            parser.OpenBrace();
            parser.OpenSquareBrackets();
            parser.CloseSquareBrackets();
            parser.EndField();
            parser.EndProperty();
            parser.End();
            
            Assert.AreEqual("s rn s_p sp f f min f f f f ep e_p eb", mockBuilder.buildPrint);
        }

        [Test]
        public void ParsePropertyRangeTest()
        {
            parser.StartArrayOfObjects();
            parser.Name("root");
            parser.StartProperty();
            parser.StartField();
            parser.Name("field definition");
            parser.Name("1");
            parser.Range();
            parser.Name("2");
            parser.EndField();
            parser.EndProperty();
            parser.End();
            
            Assert.AreEqual("s rn s_p sp f min max ep e_p eb", mockBuilder.buildPrint);
        }

        [Test]
        public void ParsePropertyCollectionTest()
        {
            parser.StartArrayOfObjects();
            parser.Name("root");
            parser.StartProperty();
            parser.StartField();
            parser.Name("field definition");
            parser.OpenSquareBrackets();
            parser.Name("1");
            parser.Comma();
            parser.Name("2");
            parser.CloseSquareBrackets();
            parser.EndField();
            parser.EndProperty();
            parser.End();
            
            Assert.AreEqual("s rn s_p sp sc f ac f ac ep e_p eb", mockBuilder.buildPrint);
        }

        [Test]
        public void ParsePropertyMapTest()
        {
            parser.StartArrayOfObjects();
            parser.Name("root");
            parser.StartProperty();
            parser.StartField();
            parser.Name("field definition");
            parser.OpenBrace();
            parser.Name("key");
            parser.Comma();
            parser.Name("value");
            parser.CloseBrace();
            parser.EndField();
            parser.EndProperty();
            parser.End();
            
            Assert.AreEqual("s rn s_p sp sm ak av ep e_p eb", mockBuilder.buildPrint);
        }
        
        [Test]
        public void ParsePropertyArrayOfMapTest()
        {
            parser.StartArrayOfObjects();
            parser.Name("root");
            parser.StartProperty();
            parser.StartField();
            parser.Name("field definition");
            parser.OpenBrace();
            parser.Name("key");
            parser.Comma();
            parser.Name("value");
            parser.CloseBrace();
            parser.OpenBrace();
            parser.Name("key");
            parser.Comma();
            parser.Name("value");
            parser.CloseBrace();
            parser.EndField();
            parser.EndProperty();
            parser.End();
            
            Assert.AreEqual("s rn s_p sp sm ak av sm ak av ep e_p eb", mockBuilder.buildPrint);
        }

        [Test]
        public void ParseSingleObjectTest()
        {
            parser.StartSingleObject();
            parser.StartField();
            parser.Name("field definition");
            parser.Name("field value");
            parser.EndField();
            parser.End();
            
            Assert.AreEqual("s_o sp f ep eb", mockBuilder.buildPrint);
        }
        
        [Test]
        public void ParseSingleObjectWithMultipleFieldsTest()
        {
            parser.StartSingleObject();
            parser.StartField();
            parser.Name("field definition 1");
            parser.Name("field value 1");
            parser.EndField();
            parser.StartField();
            parser.Name("field definition 2");
            parser.Name("field value 2");
            parser.EndField();
            parser.End();
            
            Assert.AreEqual("s_o sp f ep sp f ep eb", mockBuilder.buildPrint);
        }

        public class MockBuilder : IBuilder
        {
            public string buildPrint = string.Empty;

            public void StartBuildSingleObject()
            {
                buildPrint += "s_o";
            }
            
            public void StartBuildArrayOfObjects()
            {
                buildPrint += "s";
            }

            public void SetRootName(string name)
            {
                buildPrint += " rn";
            }

            public void StartProperty()
            {
                buildPrint += " s_p";
            }

            public void EndProperty()
            {
                buildPrint += " e_p";
            }
            
            public void StartField(string fieldDefintion)
            {
                buildPrint += " sp";
            }

            public void EndField()
            {
                buildPrint += " ep";
            }

            public void EndBuild()
            {
                buildPrint += " eb";
            }

            public void SetField(string name)
            {
                buildPrint += " f";
            }

            public void TryAddMinRange(string divider)
            {
                buildPrint += " min";
            }

            public void TryAddMaxRange(string value)
            {
                buildPrint += " max";
            }

            public void StartCollection()
            {
                buildPrint += " sc";
            }

            public void AddFieldToCollection()
            {
                buildPrint += " ac";
            }

            public void StartMap()
            {
                buildPrint += " sm";
            }

            public void AddKey(string key)
            {
                buildPrint += " ak";
            }

            public void AddValue(string value)
            {
                buildPrint += " av";
            }

            public void AppendToKey(string key)
            {
                buildPrint += " app_k";
            }

            public void AppendToValue(string value)
            {
                buildPrint += " app_v";
            }

            public void AppendToLastElementOfCollection(string append)
            {
                buildPrint += " app_c";
            }
        }
    }
}