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
        private EnterpriseEventCacheDataRequest _enterpriseEventCacheDataRequest;

        [SetUp]
        public void Setup()
        {
            _enterpriseEventCacheDataRequestValidator = new EnterpriseEventCacheDataRequestValidator();
            _enterpriseEventCacheDataRequest = GetEnterpriseEventCacheData();
        }

        #region BatchId
        [Test]
        public void WhenNullBatchIdInRequest_ThenReceiveSuccessfulResponse()
        {
            _enterpriseEventCacheDataRequest.BatchId = null;

            TestValidationResult<EnterpriseEventCacheDataRequest> result = _enterpriseEventCacheDataRequestValidator.TestValidate(_enterpriseEventCacheDataRequest);
            result.ShouldHaveValidationErrorFor("BatchId");

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "BatchId cannot be blank or null."));
            Assert.IsTrue(result.Errors.Any(x => x.ErrorCode == "OK"));
        }
        #endregion

        #region BusinessUnit
        [Test]
        public void WhenNullBusinessUnitInRequest_ThenReceiveSuccessfulResponse()
        {
            _enterpriseEventCacheDataRequest.BusinessUnit = null;

            TestValidationResult<EnterpriseEventCacheDataRequest> result = _enterpriseEventCacheDataRequestValidator.TestValidate(_enterpriseEventCacheDataRequest);
            result.ShouldHaveValidationErrorFor("BusinessUnit");

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "Business unit cannot be blank or null."));
            Assert.IsTrue(result.Errors.Any(x => x.ErrorCode == "OK"));
        }
        #endregion

        #region BatchPublishedDate
        [Test]
        public void WhenNullBatchPublishedDateInRequest_ThenReceiveSuccessfulResponse()
        {
            _enterpriseEventCacheDataRequest.BatchPublishedDate = null;

            TestValidationResult<EnterpriseEventCacheDataRequest> result = _enterpriseEventCacheDataRequestValidator.TestValidate(_enterpriseEventCacheDataRequest);
            result.ShouldHaveValidationErrorFor("BatchPublishedDate");

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "Batch published date cannot be blank or null."));
            Assert.IsTrue(result.Errors.Any(x => x.ErrorCode == "OK"));
        }
        #endregion

        #region Links
        [Test]
        public void WhenNullLinksValueInRequest_ThenReceiveSuccessfulResponse()
        {
            _enterpriseEventCacheDataRequest.Links = null;

            TestValidationResult<EnterpriseEventCacheDataRequest> result = _enterpriseEventCacheDataRequestValidator.TestValidate(_enterpriseEventCacheDataRequest);
            result.ShouldHaveValidationErrorFor("Links");

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "Links detail cannot be blank or null."));
            Assert.IsTrue(result.Errors.Any(x => x.ErrorCode == "OK"));
        }
        #endregion

        #region BatchDetails
        [Test]
        public void WhenNullBatchDetailsInRequest_ThenReceiveSuccessfulResponse()
        {
            _enterpriseEventCacheDataRequest.Links.BatchDetails = null;

            TestValidationResult<EnterpriseEventCacheDataRequest> result = _enterpriseEventCacheDataRequestValidator.TestValidate(_enterpriseEventCacheDataRequest);
            result.ShouldHaveValidationErrorFor("BatchDetails");

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "Links batch detail cannot be blank or null."));
            Assert.IsTrue(result.Errors.Any(x => x.ErrorCode == "OK"));
        }
        #endregion

        #region BatchStatus
        [Test]
        public void WhenNullBatchStatusInRequest_ThenReceiveSuccessfulResponse()
        {
            _enterpriseEventCacheDataRequest.Links.BatchStatus = null;

            TestValidationResult<EnterpriseEventCacheDataRequest> result = _enterpriseEventCacheDataRequestValidator.TestValidate(_enterpriseEventCacheDataRequest);
            result.ShouldHaveValidationErrorFor("BatchStatus");

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "Links batch status cannot be blank or null."));
            Assert.IsTrue(result.Errors.Any(x => x.ErrorCode == "OK"));
        }
        #endregion

        #region BatchDetailsUri
        [Test]
        public void WhenNullBatchDetailsUriInRequest_ThenReceiveSuccessfulResponse()
        {
            _enterpriseEventCacheDataRequest.Links.BatchDetails.Href = null;

            TestValidationResult<EnterpriseEventCacheDataRequest> result = _enterpriseEventCacheDataRequestValidator.TestValidate(_enterpriseEventCacheDataRequest);
            result.ShouldHaveValidationErrorFor("BatchDetailsUri");

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "Links batch detail uri cannot be blank or null."));
            Assert.IsTrue(result.Errors.Any(x => x.ErrorCode == "OK"));
        }
        #endregion

        #region BatchStatusUri
        [Test]
        public void WhenNullBatchStatusUriInRequest_ThenReceiveSuccessfulResponse()
        {
            _enterpriseEventCacheDataRequest.Links.BatchStatus.Href = null;

            TestValidationResult<EnterpriseEventCacheDataRequest> result = _enterpriseEventCacheDataRequestValidator.TestValidate(_enterpriseEventCacheDataRequest);
            result.ShouldHaveValidationErrorFor("BatchStatusUri");

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "Links batch status uri cannot be blank or null."));
            Assert.IsTrue(result.Errors.Any(x => x.ErrorCode == "OK"));
        }
        #endregion

        #region Files
        [Test]
        public void WhenNullLinksInRequest_ThenReceiveSuccessfulResponse()
        {
            _enterpriseEventCacheDataRequest.Files.FirstOrDefault().Links = null;

            TestValidationResult<EnterpriseEventCacheDataRequest> result = _enterpriseEventCacheDataRequestValidator.TestValidate(_enterpriseEventCacheDataRequest);
            result.ShouldHaveValidationErrorFor("Links");

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "File links cannot be blank or null."));
            Assert.IsTrue(result.Errors.Any(x => x.ErrorCode == "OK"));
        }
        #endregion

        #region MimeType
        [Test]
        public void WhenNullMimeTypeInRequest_ThenReceiveSuccessfulResponse()
        {
            _enterpriseEventCacheDataRequest.Files.FirstOrDefault().MimeType = null;

            TestValidationResult<EnterpriseEventCacheDataRequest> result = _enterpriseEventCacheDataRequestValidator.TestValidate(_enterpriseEventCacheDataRequest);
            result.ShouldHaveValidationErrorFor("MimeType");

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "File Mime type cannot be null."));
            Assert.IsTrue(result.Errors.Any(x => x.ErrorCode == "OK"));
        }
        #endregion

        #region Hash
        [Test]
        public void WhenNullHashInRequest_ThenReceiveSuccessfulResponse()
        {
            _enterpriseEventCacheDataRequest.Files.FirstOrDefault().Hash = null;

            TestValidationResult<EnterpriseEventCacheDataRequest> result = _enterpriseEventCacheDataRequestValidator.TestValidate(_enterpriseEventCacheDataRequest);
            result.ShouldHaveValidationErrorFor("Hash");

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "File hash cannot be null."));
            Assert.IsTrue(result.Errors.Any(x => x.ErrorCode == "OK"));
        }
        #endregion

        #region Filename
        [Test]
        public void WhenNullFilenameInRequest_ThenReceiveSuccessfulResponse()
        {
            _enterpriseEventCacheDataRequest.Files.FirstOrDefault().Filename = null;

            TestValidationResult<EnterpriseEventCacheDataRequest> result = _enterpriseEventCacheDataRequestValidator.TestValidate(_enterpriseEventCacheDataRequest);
            result.ShouldHaveValidationErrorFor("Filename");

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "File name cannot be null."));
            Assert.IsTrue(result.Errors.Any(x => x.ErrorCode == "OK"));
        }
        #endregion

        #region FileSize
        [Test]
        public void WhenNullFileSizeInRequest_ThenReceiveSuccessfulResponse()
        {
            _enterpriseEventCacheDataRequest.Files.FirstOrDefault().FileSize = 0;

            TestValidationResult<EnterpriseEventCacheDataRequest> result = _enterpriseEventCacheDataRequestValidator.TestValidate(_enterpriseEventCacheDataRequest);
            result.ShouldHaveValidationErrorFor("FileSize");

            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage == "File size cannot be null."));
            Assert.IsTrue(result.Errors.Any(x => x.ErrorCode == "OK"));
        }
        #endregion

        #region EnterpriseEventCacheDataRequest
        [Test]
        public void WhenValidRequest_ThenReceiveSuccessfulResponse()
        {
            TestValidationResult<EnterpriseEventCacheDataRequest> result = _enterpriseEventCacheDataRequestValidator.TestValidate(_enterpriseEventCacheDataRequest);

            Assert.AreEqual(0, result.Errors.Count);
        }
        #endregion

        private static EnterpriseEventCacheDataRequest GetEnterpriseEventCacheData()
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
                Files = new List<CacheFile>()
                {
                    new CacheFile() {
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
