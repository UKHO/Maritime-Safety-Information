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
            long bytes = 100;
            string expected = "100 Bytes";
            string result = FileHelper.FormatSize(bytes);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void WhenFormatSizeIsCalled_ThenShouldReturnSizeInKB()
        {
            long bytes = 1232;
            string expected = "1 KB";
            string result = FileHelper.FormatSize(bytes);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void WhenFormatSizeIsCalled_ThenShouldReturnSizeInMB()
        {
            long bytes = 1234567;
            string expected = "1 MB";
            string result = FileHelper.FormatSize(bytes);
            Assert.AreEqual(expected, result);
        }

    }
}
