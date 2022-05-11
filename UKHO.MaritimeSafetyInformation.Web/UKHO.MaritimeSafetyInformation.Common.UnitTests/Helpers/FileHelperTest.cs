using NUnit.Framework;
using UKHO.MaritimeSafetyInformation.Common.Helpers;

namespace UKHO.MaritimeSafetyInformation.Common.UnitTests.Helpers
{
    [TestFixture]
    public class FileHelperTest
    {
        [Test]
        public void WhenFormatSizeIsCalled_ThenShouldReturnSizeInBytes()
        {
            const long bytes = 100;
            const string expected = "100 Bytes";
            string result = FileHelper.FormatSize(bytes);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void WhenFormatSizeIsCalled_ThenShouldReturnSizeInKB()
        {
            const long bytes = 1232;
            const string expected = "1 KB";
            string result = FileHelper.FormatSize(bytes);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void WhenFormatSizeIsCalled_ThenShouldReturnSizeInMB()
        {
            const long bytes = 1234567;
            const string expected = "1 MB";
            string result = FileHelper.FormatSize(bytes);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void WhenFormatSizeIsCalled_ThenShouldRoundUpValue()
        {
            const long bytes = 1834567;
            const string expected = "2 MB";
            string result = FileHelper.FormatSize(bytes);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void WhenFormatSizeIsCalled_ThenShouldRoundDownValue()
        {
            const long bytes = 1434567;
            const string expected = "1 MB";
            string result = FileHelper.FormatSize(bytes);
            Assert.AreEqual(expected, result);
        }
    }
}
