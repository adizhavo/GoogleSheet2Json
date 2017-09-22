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
            parser.Start();
            parser.Name("root");
            parser.End();
            
            Assert.AreEqual("s rn eb", mockBuilder.buildPrint);
        }

        [Test]
        public void ParsePropertyDefinitionTest()
        {
            parser.Start();
            parser.Name("root");
            parser.Name("property");
            parser.StartField();
            parser.EndField();
            parser.End();
            
            Assert.AreEqual("s rn sp ep eb", mockBuilder.buildPrint);
        }
        
        [Test]
        public void ParsePropertyValueTest()
        {
            parser.Start();
            parser.Name("root");
            parser.Name("property");
            parser.StartField();
            parser.Name("field");
            parser.EndField();
            parser.End();
            
            Assert.AreEqual("s rn sp f ep eb", mockBuilder.buildPrint);
        }
        
        [Test]
        public void ParsePropertyValueWithSymbolsTest()
        {
            parser.Start();
            parser.Name("root");
            parser.Name("property");
            parser.StartField();
            parser.Name("field");
            parser.Name("field 2");
            parser.Comma();
            parser.Range();
            parser.CloseBrace();
            parser.OpenBrace();
            parser.OpenSquareBrackets();
            parser.CloseSquareBrackets();
            parser.EndField();
            parser.End();
            
            Assert.AreEqual("s rn sp f f f min f f f f ep eb", mockBuilder.buildPrint);
        }

        [Test]
        public void ParsePropertyRangeTest()
        {
            parser.Start();
            parser.Name("root");
            parser.Name("property");
            parser.StartField();
            parser.Name("1");
            parser.Range();
            parser.Name("2");
            parser.EndField();
            parser.End();
            
            Assert.AreEqual("s rn sp f min max ep eb", mockBuilder.buildPrint);
        }

        [Test]
        public void ParsePropertyCollectionTest()
        {
            parser.Start();
            parser.Name("root");
            parser.Name("property");
            parser.StartField();
            parser.OpenSquareBrackets();
            parser.Name("1");
            parser.Comma();
            parser.Name("2");
            parser.CloseSquareBrackets();
            parser.EndField();
            parser.End();
            
            Assert.AreEqual("s rn sp sc ac ac ep eb", mockBuilder.buildPrint);
        }

        [Test]
        public void ParsePropertyMapTest()
        {
            parser.Start();
            parser.Name("root");
            parser.Name("property");
            parser.StartField();
            parser.OpenBrace();
            parser.Name("key");
            parser.Comma();
            parser.Name("value");
            parser.CloseBrace();
            parser.EndField();
            parser.End();
            
            Assert.AreEqual("s rn sp sm ak av ep eb", mockBuilder.buildPrint);
        }

        public class MockBuilder : IBuilder
        {
            public string buildPrint = string.Empty;
            
            public void StartBuild()
            {
                buildPrint += "s";
            }

            public void SetRootName(string name)
            {
                buildPrint += " rn";
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

            public void AddCollectionElement(string element)
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
        }
    }
}