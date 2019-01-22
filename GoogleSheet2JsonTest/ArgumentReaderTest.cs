using GoogleSheet2Json;
using NUnit.Framework;

namespace GoogleSheet2JsonTest
{
    [TestFixture]
    public class ArgumentReaderTest
    {
        private ArgumentReader argumenReader;
        private string[] args;

        [SetUp]
        public void SetUp()
        {
            argumenReader = new ArgumentReader(false);
        }

        [Test]
        public void TestInitialisation()
        {
            Assert.True(argumenReader.exportConfig.isArrayOfObjects);
            Assert.False(argumenReader.exportConfig.isSingleObject);
            Assert.IsEmpty(argumenReader.exportConfig.literalKeys);
            Assert.IsEmpty(argumenReader.exportConfig.keyRange);
            Assert.IsEmpty(argumenReader.exportConfig.valueRange);
            Assert.IsEmpty(argumenReader.exportConfig.sheetTab);
            Assert.IsEmpty(argumenReader.exportConfig.configPath);
            Assert.IsEmpty(argumenReader.exportConfig.outputDir);
            Assert.AreEqual(argumenReader.exportConfig.outputFileName, "FILE_NAME_NOT_SET");
        }

        [Test]
        public void IsArrayOfObjectsTest()
        {
            args = new string[1] {"-isSingleObject"};
            argumenReader.Parse(args);

            Assert.True(argumenReader.exportConfig.isSingleObject);
            Assert.False(argumenReader.exportConfig.isArrayOfObjects);
        }

        [Test]
        public void SingleLiteralKeyTest()
        {
            args = new string[1] {"-isLiteral=sample_key"};
            argumenReader.Parse(args);

            Assert.True(argumenReader.exportConfig.literalKeys.Contains("sample_key"));
        }

        [Test]
        public void MultipleLiteralKeysTest()
        {
            args = new string[1] {"-isLiteral=sample_key_1,sample_key_2"};
            argumenReader.Parse(args);

            Assert.True(argumenReader.exportConfig.literalKeys.Contains("sample_key_1"));
            Assert.True(argumenReader.exportConfig.literalKeys.Contains("sample_key_2"));
        }

        [Test]
        public void SheetTabTest()
        {
            args = new string[1] {"-sheetTab=SAMPLE_TAB"};
            argumenReader.Parse(args);

            Assert.AreEqual(argumenReader.exportConfig.sheetTab, "SAMPLE_TAB");
        }

        [Test]
        public void KeyRangeTest()
        {
            args = new string[1] {"-keyRange=A2:Q"};
            argumenReader.Parse(args);

            Assert.AreEqual(argumenReader.exportConfig.keyRange, "A2:Q");
        }

        [Test]
        public void ValueRangeTest()
        {
            args = new string[1] {"-valueRange=A2:Q"};
            argumenReader.Parse(args);

            Assert.AreEqual(argumenReader.exportConfig.valueRange, "A2:Q");
        }

        [Test]
        public void ConfigPathTest()
        {
            args = new string[1] {"-configPath=/Sample/Path"};
            argumenReader.Parse(args);

            Assert.AreEqual(argumenReader.exportConfig.configPath, "/Sample/Path");
        }

        [Test]
        public void OutputDirPathTest()
        {
            args = new string[1] {"-outputDir=/Sample/Path"};
            argumenReader.Parse(args);

            Assert.AreEqual(argumenReader.exportConfig.outputDir, "/Sample/Path");
        }

        [Test]
        public void OutputDirFileNameTest()
        {
            args = new string[1] {"-outputFileName=FileName"};
            argumenReader.Parse(args);

            Assert.AreEqual(argumenReader.exportConfig.outputFileName, "FileName");
        }

        [Test]
        public void ClITest()
        {
            args = new string[8]
            {
            "-isSingleObject",
            "-isLiteral=sample_key_1",
            "-sheetTab=SAMPLE_TAB",
            "-keyRange=A2:Q",
            "-valueRange=A2:Q",
            "-configPath=/Sample/Path",
            "-outputDir=/Sample/Path",
            "-outputFileName=FileName"
            };
            argumenReader.Parse(args);

            Assert.False(argumenReader.exportConfig.isArrayOfObjects);
            Assert.True(argumenReader.exportConfig.isSingleObject);
            Assert.True(argumenReader.exportConfig.literalKeys.Contains("sample_key_1"));
            Assert.AreEqual(argumenReader.exportConfig.valueRange, "A2:Q");
            Assert.AreEqual(argumenReader.exportConfig.keyRange, "A2:Q");
            Assert.AreEqual(argumenReader.exportConfig.sheetTab, "SAMPLE_TAB");
            Assert.AreEqual(argumenReader.exportConfig.configPath, "/Sample/Path");
            Assert.AreEqual(argumenReader.exportConfig.outputDir, "/Sample/Path");
            Assert.AreEqual(argumenReader.exportConfig.outputFileName, "FileName");
        }
    }
}