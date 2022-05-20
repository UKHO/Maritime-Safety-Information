using NUnit.Framework;
using System;
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
            Assert.AreEqual("310130 UTC Dec 22", result);
        }

        [Test]
        public void WhenCallRnwDateFormatWithNullDate_ThenReturnEmptyString()
        {
            string result = DateTimeExtensions.ToRnwDateFormat(null);
            Assert.True(string.IsNullOrEmpty(result));
        }
    }
}
