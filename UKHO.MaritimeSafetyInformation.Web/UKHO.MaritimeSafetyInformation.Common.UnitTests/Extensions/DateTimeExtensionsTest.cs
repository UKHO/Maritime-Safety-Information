using System;
using NUnit.Framework;
using UKHO.MaritimeSafetyInformation.Common.Extensions;

namespace UKHO.MaritimeSafetyInformation.Common.UnitTests.Extensions
{
    [TestFixture]
    public class DateTimeExtensionsTest
    {
        [Test]
        public void WhenCallRnwDateFormat_ThenReturnValidRnwDateFormat()
        {
            DateTime dateTime = new(2022, 12, 31, 01, 30, 40);
            string result = DateTimeExtensions.ToRnwDateFormat(dateTime);
            Assert.That("310130 UTC Dec 22", Is.EqualTo(result));
        }

        [Test]
        public void WhenCallRnwDateFormatWithNullDate_ThenReturnEmptyString()
        {
            string result = DateTimeExtensions.ToRnwDateFormat(null);
            Assert.That(string.IsNullOrEmpty(result));
        }
    }
}
