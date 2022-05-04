using NUnit.Framework;
using System.Collections.Generic;
using UKHO.FileShareClient.Models;
using UKHO.MaritimeSafetyInformation.Common.Helpers;
using UKHO.MaritimeSafetyInformation.Common.Models.NoticesToMariners;

namespace UKHO.MaritimeSafetyInformation.Common.UnitTests.Helper
{
    public class NMHelperTest
    {
        [Test]
        public void WhenNMHelperCallsListFilesResponse_ThenConversionIsCorrect()
        {
            BatchSearchResponse SearchResult = SetSearchResult();

            List<ShowFilesResponseModel> expected = new() {
                new ShowFilesResponseModel() {
                        BatchId = "1",
                        Filename = "aaa.pdf",
                        FileDescription = "aaa",
                        FileExtension = ".pdf",
                        FileSize = 1232,
                        FileSizeinKB = "1.2 KB",
                        MimeType = "PDF",
                        Links = null
                },
                new ShowFilesResponseModel() {
                        BatchId = "1",
                        Filename = "bbb.pdf",
                        FileDescription = "bbb",
                        FileExtension = ".pdf",
                        FileSize = 1232,
                        FileSizeinKB = "1.2 KB",
                        MimeType = "PDF",
                        Links = null
                },
                new ShowFilesResponseModel() {
                        BatchId = "2",
                        Filename = "ccc.pdf",
                        FileDescription = "ccc",
                        FileExtension = ".pdf",
                        FileSize = 1232,
                        FileSizeinKB = "1.2 KB",
                        MimeType = "PDF",
                        Links = null
                },
                new ShowFilesResponseModel() {
                        BatchId = "2",
                        Filename = "ddd.pdf",
                        FileDescription = "ddd",
                        FileExtension = ".pdf",
                        FileSize = 1232,
                        FileSizeinKB = "1.2 KB",
                        MimeType = "PDF",
                        Links = null
                }
            };

            List<ShowFilesResponseModel> result = NMHelper.ListFilesResponse(SearchResult);

            Assert.Multiple(() =>
            {
                for (int i = 0; i < result.Count; i++)
                {
                    Assert.AreEqual(expected[i].BatchId, result[i].BatchId);
                    Assert.AreEqual(expected[i].Filename, result[i].Filename);
                    Assert.AreEqual(expected[i].FileDescription, result[i].FileDescription);
                    Assert.AreEqual(expected[i].FileExtension, result[i].FileExtension);
                    Assert.AreEqual(expected[i].FileSize, result[i].FileSize);
                    Assert.AreEqual(expected[i].FileSizeinKB, result[i].FileSizeinKB);
                    Assert.AreEqual(expected[i].MimeType, result[i].MimeType);
                }
            });
        }

        private static BatchSearchResponse SetSearchResult()
        {
            BatchSearchResponse SearchResult = new()
            {
                Count = 2,
                Links = null,
                Total = 0,
                Entries = new List<BatchDetails>() {
                    new BatchDetails() {
                        BatchId = "1",
                        Files = new List<BatchDetailsFiles>() {
                            new BatchDetailsFiles () {
                                Filename = "aaa.pdf",
                                FileSize=1232,
                                MimeType = "PDF",
                                Links = null
                            },
                            new BatchDetailsFiles () {
                                Filename = "bbb.pdf",
                                FileSize=1232,
                                MimeType = "PDF",
                                Links = null
                            }
                        }

                    },
                    new BatchDetails() {
                        BatchId = "2",
                        Files = new List<BatchDetailsFiles>() {
                            new BatchDetailsFiles () {
                                Filename = "ccc.pdf",
                                FileSize=1232,
                                MimeType = "PDF",
                                Links = null
                            },
                            new BatchDetailsFiles () {
                                Filename = "ddd.pdf",
                                FileSize=1232,
                                MimeType = "PDF",
                                Links = null
                            }
                        }

                    }
                }
            };

            return SearchResult;
        }

    }
}
