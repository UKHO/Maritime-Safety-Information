using FluentValidation.TestHelper;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UKHO.MaritimeSafetyInformation.Common.Models.WebhookRequest;
using UKHO.MaritimeSafetyInformation.Web.Validation;

namespace UKHO.MaritimeSafetyInformation.Web.UnitTests.Validation
{
    [TestFixture]
    public class EnterpriseEventCacheDataRequestValidatorTests
    {
        private EnterpriseEventCacheDataRequestValidator _enterpriseEventCacheDataRequestValidator;
        private FSSNewFilesPublishedEventData _enterpriseEventCacheDataRequest;

        [SetUp]
        public void Setup()
        {
            _enterpriseEventCacheDataRequestValidator = new EnterpriseEventCacheDataRequestValidator();
            _enterpriseEventCacheDataRequest = GetEnterpriseEventCacheData();
        }

        [Test]
        public void WhenEnterpriseEventCacheDataRequestValidatorIsCalledWithNullBusinessUnit_ThenShouldReturnErrorMessage()
        {
            _enterpriseEventCacheDataRequest.BusinessUnit = null;

            TestValidationResult<FSSNewFilesPublishedEventData> result = _enterpriseEventCacheDataRequestValidator.TestValidate(_enterpriseEventCacheDataRequest);
            result.ShouldHaveValidationErrorFor("BusinessUnit");

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "Business unit cannot be blank or null."));
            Assert.IsTrue(result.Errors.Any(x => x.ErrorCode == "OK"));
        }

        [Test]
        public void WhenEnterpriseEventCacheDataRequestValidatorIsCalledWithNullAttributeValue_ThenShouldReturnErrorMessage()
        {
            _enterpriseEventCacheDataRequest.Attributes.FirstOrDefault().Value = null;

            TestValidationResult<FSSNewFilesPublishedEventData> result = _enterpriseEventCacheDataRequestValidator.TestValidate(_enterpriseEventCacheDataRequest);
            result.ShouldHaveValidationErrorFor("Attributes");

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "Attribute key or value cannot be null."));
            Assert.IsTrue(result.Errors.Any(x => x.ErrorCode == "OK"));
        }

        [Test]
        public void WhenEnterpriseEventCacheDataRequestValidatorIsCalledWithNullAttributeKey_ThenShouldReturnErrorMessage()
        {
            _enterpriseEventCacheDataRequest.Attributes.FirstOrDefault().Key = null;

            TestValidationResult<FSSNewFilesPublishedEventData> result = _enterpriseEventCacheDataRequestValidator.TestValidate(_enterpriseEventCacheDataRequest);
            result.ShouldHaveValidationErrorFor("Attributes");

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "Attribute key or value cannot be null."));
            Assert.IsTrue(result.Errors.Any(x => x.ErrorCode == "OK"));
        }

        [Test]
        public void WhenEnterpriseEventCacheDataRequestValidatorIsCalledWithNullAttributes_ThenShouldReturnErrorMessage()
        {
            _enterpriseEventCacheDataRequest.Attributes = null;

            TestValidationResult<FSSNewFilesPublishedEventData> result = _enterpriseEventCacheDataRequestValidator.TestValidate(_enterpriseEventCacheDataRequest);
            result.ShouldHaveValidationErrorFor("Attributes");

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "Attributes cannot be blank or null."));
            Assert.IsTrue(result.Errors.Any(x => x.ErrorCode == "OK"));
        }

        [Test]
        public void WhenEnterpriseEventCacheDataRequestValidatorIsCalledValidRequest_ThenReceiveSuccessfulResponse()
        {
            TestValidationResult<FSSNewFilesPublishedEventData> result = _enterpriseEventCacheDataRequestValidator.TestValidate(_enterpriseEventCacheDataRequest);

            Assert.AreEqual(0, result.Errors.Count);
        }
        
        private static FSSNewFilesPublishedEventData GetEnterpriseEventCacheData()
        {
            return new()
            {
                Links = new()
                {
                    BatchDetails = new()
                    {
                        Href = "https://filesqa.admiralty.co.uk/batch/83d08093-7a67-4b3a-b431-92ba42feaea0"
                    },
                    BatchStatus = new()
                    {
                        Href = "https://filesqa.admiralty.co.uk/batch/83d08093-7a67-4b3a-b431-92ba42feaea0/status"
                    }
                },
                BatchId = "83d08093-7a67-4b3a-b431-92ba42feqw12",
                BusinessUnit = "MaritimeSafetyInformation",
                Attributes = new List<Common.Models.WebhookRequest.Attribute>()
                {  
                    new Common.Models.WebhookRequest.Attribute() { Key = "Product Type", Value = "Notices to Mariners" }
                },
                BatchPublishedDate = Convert.ToDateTime("2022-04-04T11:22:18.2943076Z"),
                Files = new List<File>()
                {
                    new File() {
                        Filename = "S631-1_Update_Wk45_21_Only.zip",
                        FileSize= 99073923,
                        MimeType= "application/zip",
                        Hash= "yNpJTWFKhD3iasV8B/ePKw==",
                        Attributes = new List<Common.Models.WebhookRequest.Attribute>()
                        {   new Common.Models.WebhookRequest.Attribute() {Key = "TestKey", Value= "Test Value"}
                        },
                        Links = new() {
                        Get= new() {
                                Href = "https://filesqa.admiralty.co.uk/batch/83d08093-7a67-4b3a-b431-92ba42feaea0/files/S631-1_Update_Wk45_21_Only.zip"
                            }
                        }
                    }
                }
            };
        }
    }
}
