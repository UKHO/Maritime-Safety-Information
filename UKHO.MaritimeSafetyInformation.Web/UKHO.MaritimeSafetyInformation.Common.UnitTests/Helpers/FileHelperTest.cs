using NUnit.Framework;
using UKHO.MaritimeSafetyInformation.Common.Helpers;

namespace UKHO.MaritimeSafetyInformation.Common.UnitTests.Helpers
{
    [TestFixture]
    public class FileHelperTest
    {
        [TestCase(100, ExpectedResult = "100 Bytes", Description = "When FormatSize Is Called Then Should Return Size In Bytes")]
        [TestCase(1232, ExpectedResult = "1 KB", Description = "When FormatSize Is Called Then Should Return Size In KB")]
        [TestCase(1234567, ExpectedResult = "1 MB", Description = "When FormatSize Is Called Then Should Return Size In MB")]
        [TestCase(1834567, ExpectedResult = "2 MB", Description = "When FormatSize Is Called Then Should Round Up Value")]
        [TestCase(1434567, ExpectedResult = "1 MB", Description = "When FormatSize Is Called Then Should Round Down Value")]
        public string WhenFormatSizeIsCalled_ThenShouldReturnSizeInBytes(long bytes)
        {
            return FileHelper.FormatSize(bytes);
        }
    }
}
