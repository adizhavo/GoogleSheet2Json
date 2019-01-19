using System.Collections.Generic;
using GoogleSheet2Json;
using NUnit.Framework;

namespace GoogleSheet2JsonTest
{
    [TestFixture]
    public class LexerTest
    {
        private MockParser mockParser;
        private Lexer lexer;

        private IList<object> keys;
        private IList<IList<object>> values;

        private ExportConfig mockExportConfig;

        [SetUp]
        public void Init()
        {
            // Init mocks and lexer
            mockParser = new MockParser();
            lexer = new Lexer(mockParser);
            mockExportConfig = new ExportConfig();

            // Init mock data 
            keys = new List<object> {"sampleKey"};
            // ugly annotation imposed by the Google API
            values = new List<IList<object>> {new List<object>()};
        }

        [Test]
        public void LexEmptyDataTest()
        {
            keys.Clear();
            lexer.Lex(keys, values, mockExportConfig);

            Assert.AreEqual("s n e", mockParser.transition);
        }

        [Test]
        public void LexEmptyValueDataTest()
        {
            values[0].Add("");
            lexer.Lex(keys, values, mockExportConfig);

            Assert.AreEqual("s n sp ep e", mockParser.transition);
        }

        [Test]
        public void LexSingleWordTest()
        {
            values[0].Add("single_word");
            lexer.Lex(keys, values, mockExportConfig);

            Assert.AreEqual("s n sp s_p n n e_p ep e", mockParser.transition);
        }

        [Test]
        public void LexMultipleWordTest()
        {
            values[0].Add("multplie words with numbers 1 2 3");
            lexer.Lex(keys, values, mockExportConfig);

            Assert.AreEqual("s n sp s_p n n e_p ep e", mockParser.transition);
        }

        [Test]
        public void LexMultipleWordsWithDotTest()
        {
            values[0].Add("word with special . dot");
            lexer.Lex(keys, values, mockExportConfig);

            Assert.AreEqual("s n sp s_p n n n e_p ep e", mockParser.transition);
        }

        [Test]
        public void LexSpecialCharactersAsSingleCharactersTest()
        {
            values[0].Add(". + : ; ' ! ± @ # $ % ˆ ˜ ` ? - < … æ ‘ “ “ ≥ æ ≤ ¡ ™ £ ¢ ∞ § ¶ • ª º – ≠ œ ∑ ´ ® ¥ ¨ ˆ π ¬ ˚ ∆ ˙ © ƒ ∂ ß å Ω ç ≈ √ ˜ µ §");
            lexer.Lex(keys, values, mockExportConfig);

            Assert.AreEqual("s n sp s_p n n n n n n n n n n n n n n n n n n n n n n n n n n n n n n n n n n n n n n n n n n n n n n n n e_p ep e", mockParser.transition);
        }

        [Test]
        public void LexMultipleWordsWithRangeTest()
        {
            values[0].Add("word with special > dash");
            lexer.Lex(keys, values, mockExportConfig);

            Assert.AreEqual("s n sp s_p n n d n e_p ep e", mockParser.transition);
        }

        [Test]
        public void LexMultipleWordsWithSquareBraceTest()
        {
            values[0].Add("word with special [] square braces");
            lexer.Lex(keys, values, mockExportConfig);

            Assert.AreEqual("s n sp s_p n n o_s_b c_s_b n e_p ep e", mockParser.transition);
        }

        [Test]
        public void LexMultipleWordsWithBraceTest()
        {
            values[0].Add("word with special () braces");
            lexer.Lex(keys, values, mockExportConfig);

            Assert.AreEqual("s n sp s_p n n o_b c_b n e_p ep e", mockParser.transition);
        }

        [Test]
        public void LexRangeOfNumbersTest()
        {
            values[0].Add("1>2");
            lexer.Lex(keys, values, mockExportConfig);

            Assert.AreEqual("s n sp s_p n n d n e_p ep e", mockParser.transition);
        }

        [Test]
        public void LexRangeOfNumbersWithSpaceTest()
        {
            values[0].Add(" 1 > 2 ");
            lexer.Lex(keys, values, mockExportConfig);

            Assert.AreEqual("s n sp s_p n n d n e_p ep e", mockParser.transition);
        }

        [Test]
        public void LexCollectionOfNumbersTest()
        {
            values[0].Add("[1,2,3,4]");
            lexer.Lex(keys, values, mockExportConfig);

            Assert.AreEqual("s n sp s_p n o_s_b n c n c n c n c_s_b e_p ep e", mockParser.transition);
        }

        [Test]
        public void LexCollectionOfNumbersWithSpacesTest()
        {
            values[0].Add(" [ 1 , 2 , 3 , 4 ] ");
            lexer.Lex(keys, values, mockExportConfig);

            Assert.AreEqual("s n sp s_p n o_s_b n c n c n c n c_s_b e_p ep e", mockParser.transition);
        }

        [Test]
        public void LexCollectionOfWordsWithSpacesTest()
        {
            values[0].Add(" [ one , two , three , four ] ");
            lexer.Lex(keys, values, mockExportConfig);

            Assert.AreEqual("s n sp s_p n o_s_b n c n c n c n c_s_b e_p ep e", mockParser.transition);
        }

        [Test]
        public void LexMapOfWordsAndNumberTest()
        {
            values[0].Add("(one, 1)");
            lexer.Lex(keys, values, mockExportConfig);

            Assert.AreEqual("s n sp s_p n o_b n c n c_b e_p ep e", mockParser.transition);
        }

        [Test]
        public void LexMapOfWordsAndNumberWithSpaceTest()
        {
            values[0].Add("( one , 1 )");
            lexer.Lex(keys, values, mockExportConfig);

            Assert.AreEqual("s n sp s_p n o_b n c n c_b e_p ep e", mockParser.transition);
        }

        [Test]
        public void LexArrayMapOfWordsAndNumberTest()
        {
            values[0].Add("(one, 1) (two, 2) (three, 3)");
            lexer.Lex(keys, values, mockExportConfig);

            Assert.AreEqual("s n sp s_p n o_b n c n c_b o_b n c n c_b o_b n c n c_b e_p ep e", mockParser.transition);
        }

        [Test]
        public void LexMultipleValueDataTest()
        {
            values[0].Add("1");
            values.Add(new List<object>());
            values[1].Add("1");
            lexer.Lex(keys, values, mockExportConfig);

            Assert.AreEqual("s n sp s_p n n e_p ep sp s_p n n e_p ep e", mockParser.transition);
        }

        [Test]
        public void LexMultipleKeyDataTest()
        {
            values[0].Add("1");
            values[0].Add("2");
            keys.Add("sampleKey 2");

            lexer.Lex(keys, values, mockExportConfig);

            Assert.AreEqual("s n sp s_p n n e_p s_p n n e_p ep e", mockParser.transition);
        }

        [Test]
        public void LexNegativeNumbersTest()
        {
            values[0].Add("-1");
            lexer.Lex(keys, values, mockExportConfig);

            Assert.AreEqual("s n sp s_p n n n e_p ep e", mockParser.transition);
        }

        [Test]
        public void LexNegativeNumbersRangeTest()
        {
            values[0].Add("-1 > -2");
            lexer.Lex(keys, values, mockExportConfig);

            Assert.AreEqual("s n sp s_p n n n d n n e_p ep e", mockParser.transition);
        }

        [Test]
        public void LexNegativeNumbersCollectionTest()
        {
            values[0].Add("[-1 , -2]");
            lexer.Lex(keys, values, mockExportConfig);

            Assert.AreEqual("s n sp s_p n o_s_b n n c n n c_s_b e_p ep e", mockParser.transition);
        }

        [Test]
        public void LexNegativeNumbersMapTest()
        {
            values[0].Add("(-1 , -2)");
            lexer.Lex(keys, values, mockExportConfig);

            Assert.AreEqual("s n sp s_p n o_b n n c n n c_b e_p ep e", mockParser.transition);
        }

        [Test]
        public void LexLiteralValueTest()
        {
            values[0].Add("{\"min\":1}");
            mockExportConfig.literalKeys.Add("sampleKey");
            lexer.Lex(keys, values, mockExportConfig);

            Assert.AreEqual("s n sp s_p n n e_p ep e", mockParser.transition);
        }

        [Test]
        public void LextSingleObjectTest()
        {
            values[0].Add("object_def");
            values[0].Add("object_value");
            mockExportConfig.isSingleObject = true;
            mockExportConfig.isArrayOfObjects = false;
            lexer.Lex(keys, values, mockExportConfig);

            Assert.AreEqual("s_s s_p n n e_p e", mockParser.transition);
        }

        [Test]
        public void LextSingleObjectWithLiteralValueTest()
        {
            values[0].Add("{\"min\":1}");
            values[0].Add("object_value");
            mockExportConfig.isSingleObject = true;
            mockExportConfig.isArrayOfObjects = false;
            mockExportConfig.literalKeys.Add("sampleKey");
            lexer.Lex(keys, values, mockExportConfig);

            Assert.AreEqual("s_s s_p n n e_p e", mockParser.transition);
        }
    }

    public class MockParser : IParser
    {
        public string transition;

        public void StartSingleObject()
        {
            transition += "s_s";
        }

        public void StartArrayOfObjects()
        {
            transition += "s";
        }

        public void End()
        {
            transition += " e";
        }

        public void Name(string name)
        {
            transition += " n";
        }

        public void StartProperty()
        {
            transition += " sp";
        }

        public void EndProperty()
        {
            transition += " ep";
        }

        public void StartField()
        {
            transition += " s_p";
        }

        public void EndField()
        {
            transition += " e_p";
        }

        public void Comma()
        {
            transition += " c";
        }

        public void OpenBracket()
        {
            transition += " o_b";
        }

        public void CloseBracket()
        {
            transition += " c_b";
        }

        public void OpenCurlyBracket()
        {
            transition += " o_cb";
        }

        public void CloseCurlyBracket()
        {
            transition += " c_cb";
        }

        public void OpenSquareBracket()
        {
            transition += " o_s_b";
        }

        public void CloseSquareBracket()
        {
            transition += " c_s_b";
        }

        public void Range()
        {
            transition += " d";
        }

        public void WhiteSpace()
        {
        }
    }
}