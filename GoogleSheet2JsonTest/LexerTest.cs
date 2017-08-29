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
        
        [SetUp]
        public void Init()
        {
            // Init mocks and lexer
            mockParser = new MockParser();
            lexer = new Lexer(mockParser);
            
            // Init mock data 
            keys = new List<object> {"sampleKey"};
            // ugly annotation imposed by the Google API
            values = new List<IList<object>> {new List<object>()};
        }

        [Test]
        public void LexEmptyDataTest()
        {
            keys.Clear();
            lexer.Lex(keys, values);
            
            Assert.AreEqual("s n e", mockParser.transition);
        }
        
        [Test]
        public void LexEmptyValueDataTest()
        {
            values[0].Add("");
            lexer.Lex(keys, values);
            
            Assert.AreEqual("s n n s_p e_p e", mockParser.transition);
        }
        
        [Test]
        public void LexSingleWordTest()
        {
            values[0].Add("single_word");
            lexer.Lex(keys, values);
            
            Assert.AreEqual("s n n s_p n e_p e", mockParser.transition);
        }
        
        [Test]
        public void LexMultipleWordTest()
        {
            values[0].Add("multplie words with numbers 1 2 3");
            lexer.Lex(keys, values);
             
            Assert.AreEqual("s n n s_p n e_p e", mockParser.transition);
        }
        
        [Test]
        public void LexArrayOfNumbersWithTwoElementsTest()
        {
            values[0].Add("1, 2");
            lexer.Lex(keys, values);
            
            Assert.AreEqual("s n n s_p n c n e_p e", mockParser.transition);
        }
        
        [Test]
        public void LexArrayOfNumbersWithMultipleElementsTest()
        {
            values[0].Add("1, 2, 3, 4, 5");
            lexer.Lex(keys, values);
            
            Assert.AreEqual("s n n s_p n c n c n c n c n e_p e", mockParser.transition);
        }
        
        [Test]
        public void LexArrayOfWordsWithMultipleElementsTest()
        {
            values[0].Add("first, second, third, fourth, fifth");
            lexer.Lex(keys, values);
            
            Assert.AreEqual("s n n s_p n c n c n c n c n e_p e", mockParser.transition);
        }
        
        [Test]
        public void LexArrayOfNumbersWithMultipleElementsAndSpacesBetweenTest()
        {
            values[0].Add("1 , 2 , 3 , 4 , 5");
            lexer.Lex(keys, values);
            
            Assert.AreEqual("s n n s_p n c n c n c n c n e_p e", mockParser.transition);
        }

        [Test]
        public void LexRangeOfNumbersTest()
        {
            values[0].Add("1-2");
            lexer.Lex(keys, values);
            
            Assert.AreEqual("s n n s_p n d n e_p e", mockParser.transition);
        }
        
        [Test]
        public void LexRangeOfNumbersWithSpaceTest()
        {
            values[0].Add(" 1 - 2 ");
            lexer.Lex(keys, values);
            
            Assert.AreEqual("s n n s_p n d n e_p e", mockParser.transition);
        }

        [Test]
        public void LexCollectionOfNumbersTest()
        {
            values[0].Add("[1,2,3,4]");
            lexer.Lex(keys, values);
            
            Assert.AreEqual("s n n s_p o_s_b n c n c n c n c_s_b e_p e", mockParser.transition);
        }
        
        [Test]
        public void LexCollectionOfNumbersWithSpacesTest()
        {
            values[0].Add(" [ 1 , 2 , 3 , 4 ] ");
            lexer.Lex(keys, values);
            
            Assert.AreEqual("s n n s_p o_s_b n c n c n c n c_s_b e_p e", mockParser.transition);
        }
        
        [Test]
        public void LexCollectionOfWordsWithSpacesTest()
        {
            values[0].Add(" [ one , two , three , four ] ");
            lexer.Lex(keys, values);
            
            Assert.AreEqual("s n n s_p o_s_b n c n c n c n c_s_b e_p e", mockParser.transition);
        }
        
        [Test]
        public void LexMapOfWordsAndNumberTest()
        {
            values[0].Add("(one, 1)");
            lexer.Lex(keys, values);
            
            Assert.AreEqual("s n n s_p o_b n c n c_b e_p e", mockParser.transition);
        }
        
        [Test]
        public void LexMapOfWordsAndNumberWithSpaceTest()
        {
            values[0].Add("( one , 1 )");
            lexer.Lex(keys, values);
            
            Assert.AreEqual("s n n s_p o_b n c n c_b e_p e", mockParser.transition);
        }
        
        [Test]
        public void LexArrayMapOfWordsAndNumberTest()
        {
            values[0].Add("(one, 1) (two, 2) (three, 3)");
            lexer.Lex(keys, values);
            
            Assert.AreEqual("s n n s_p o_b n c n c_b o_b n c n c_b o_b n c n c_b e_p e", mockParser.transition);
        }
        
        [Test]
        public void LexMultipleValueDataTest()
        {
            values[0].Add("1");
            values.Add(new List<object>());
            values[1].Add("1");
            lexer.Lex(keys, values);
            
            Assert.AreEqual("s n n s_p n e_p n s_p n e_p e", mockParser.transition);
        }
        
        [Test]
        public void LexMultipleKeyDataTest()
        {
            values[0].Add("1");
            values[0].Add("2");
            keys.Add("sampleKey 2");
            
            lexer.Lex(keys, values);
            
            Assert.AreEqual("s n n s_p n e_p n s_p n e_p e", mockParser.transition);
        }
    }

    public class MockParser : IParser
    {
        public string transition;
        
        public void Start()
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
            transition += " s_p";
        }

        public void EndProperty()
        {
            transition += " e_p";
        }

        public void Comma()
        {
            transition += " c";
        }

        public void OpenBrace()
        {
            transition += " o_b";
        }

        public void CloseBrace()
        {
            transition += " c_b";
        }

        public void OpenSquareBrackets()
        {
            transition += " o_s_b";
        }

        public void CloseSquareBrackets()
        {
            transition += " c_s_b";
        }

        public void Dash()
        {
            transition += " d";
        }
    }
}

