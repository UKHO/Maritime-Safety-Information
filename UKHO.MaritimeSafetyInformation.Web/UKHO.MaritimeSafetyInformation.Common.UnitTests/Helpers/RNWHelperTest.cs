using NUnit.Framework;
using UKHO.MaritimeSafetyInformation.Common.Helpers;

namespace UKHO.MaritimeSafetyInformation.Common.UnitTests.Helpers
{
    [TestFixture]
    public class RnwHelperTest
    {

        [Test]
        public void WhenCallFormatContentWithContentLengthGreaterThan300Char_ThenWrapTheContent()
        {
            const string content = "1. NAVAREA I WARNINGS IN FORCE AT 221000 UTC APR 22:     2021 SERIES: 031.  2022 SERIES: 033, 041, 043, 044."
                 + "NOTES:  A. Texts of NAVAREA I Warnings issued each week are published in weekly editions of the ADMIRALTY Notices to"
                 + "Mariners bulletin (ANMB).  B. NAVAREA I Warnings less than 42 days old (033/22 onward) are promulgated via Enhanced"
                 + "Group Call (EGC) and/or relevant NAVTEX transmitters.  C. The complete texts of all in-force NAVAREA I warnings,"
                 + "including those which are no longer being broadcast, are reprinted in Section III of ANMB in weeks 1, 13, 26 and 39"
                 + "and are also available from the UKHO website at: www.admiralty.co.uk/RNW.  Alternatively, these may be requested by"
                 + "e-mail from NAVAREA I Co-ordinator at: navwarnings@ukho.gov.uk    2. Cancel NAVAREA I 042/22.";

            string result = RnwHelper.FormatContent(content);
            Assert.That(result.Length <= 303);
            Assert.That(result.Contains("..."));
        }

        [Test]
        public void WhenCallFormatContentWithContentLengthLessThan300Char_ThenDoNotWrapTheContent()
        {
            const string content = "Test Content String";
            string result = RnwHelper.FormatContent(content);
            Assert.That(content, Is.EqualTo(result));
            Assert.That(!result.Contains("..."));
        }

        [Test]
        public void WhenCallFormatContentWithContentLengthEqual300Char_ThenDoNotWrapTheContent()
        {
            string content = new('x', 300);
            string result = RnwHelper.FormatContent(content);
            Assert.That(300, Is.EqualTo(result.Length));
            Assert.That(!result.Contains("..."));
        }

        [Test]
        public void WhenCallFormatContentWithNullContent_ThenReturnEmptyContent()
        {
            string result = RnwHelper.FormatContent(null);
            Assert.That(string.Empty, Is.EqualTo(result));
        }
    }
}
