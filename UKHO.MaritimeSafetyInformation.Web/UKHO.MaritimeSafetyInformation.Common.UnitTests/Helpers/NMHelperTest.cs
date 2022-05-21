using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UKHO.FileShareClient.Models;
using UKHO.MaritimeSafetyInformation.Common.Helpers;
using UKHO.MaritimeSafetyInformation.Common.Models.NoticesToMariners;

namespace UKHO.MaritimeSafetyInformation.Common.UnitTests.Helpers
{
    public class NMHelperTest
    {

        [Test]
        public void WhenNMHelperCallsListFilesResponse_ThenConversionIsCorrect()
        {
            BatchSearchResponse searchResult = SetSearchResultForWeekly();

            List<ShowFilesResponseModel> expected = new()
            {
                new ShowFilesResponseModel()
                {
                    BatchId = "1",
                    Filename = "aaa.pdf",
                    FileDescription = "aaa",
                    FileExtension = ".pdf",
                    FileSize = 1232,
                    FileSizeinKB = "1 KB",
                    MimeType = "PDF",
                    Links = null
                },
                new ShowFilesResponseModel()
                {
                    BatchId = "1",
                    Filename = "bbb.pdf",
                    FileDescription = "bbb",
                    FileExtension = ".pdf",
                    FileSize = 1232,
                    FileSizeinKB = "1 KB",
                    MimeType = "PDF",
                    Links = null
                },
                new ShowFilesResponseModel()
                {
                    BatchId = "2",
                    Filename = "ccc.pdf",
                    FileDescription = "ccc",
                    FileExtension = ".pdf",
                    FileSize = 1232,
                    FileSizeinKB = "1 KB",
                    MimeType = "PDF",
                    Links = null
                },
                new ShowFilesResponseModel()
                {
                    BatchId = "2",
                    Filename = "ddd.pdf",
                    FileDescription = "ddd",
                    FileExtension = ".pdf",
                    FileSize = 1232,
                    FileSizeinKB = "1 KB",
                    MimeType = "PDF",
                    Links = null
                }
            };

            List<ShowFilesResponseModel> result = NMHelper.ListFilesResponse(searchResult);

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

        [Test]
        public void WhenGetDailyShowFilesResponseIsCalled_ThenConversionIsCorrect()
        {
            BatchSearchResponse searchResult = SetSearchResultForDaily();

            List<ShowDailyFilesResponseModel> expected = new()
            {
                new ShowDailyFilesResponseModel
                {
                    YearWeek = "2022 / 17",
                    WeekNumber = "17",
                    Year = "2022",
                    DailyFilesData = new()
                    {
                        new DailyFilesDataModel()
                        {
                            DataDate = "2022-04-21",
                            BatchId = "68970ffc-4820-47eb-be76-aaa3209eb3b6",
                            Filename = "Daily 21-04-22.zip",
                            FileDescription = "Daily 21-04-22.zip",
                            FileExtension = ".zip",
                            FileSizeInKB = FileHelper.FormatSize(299170),
                            MimeType = "application/gzip",
                            Links = null,
                            AllFilesZipSize = 299170
                        },
                        new DailyFilesDataModel()
                        {
                            DataDate = "2022-04-22",
                            BatchId = "2cd869e1-a1e2-4a7d-94bb-1f60fddec9fe",
                            Filename = "Daily 22-04-22.zip",
                            FileDescription = "Daily 22-04-22.zip",
                            FileExtension = ".zip",
                            FileSizeInKB = FileHelper.FormatSize(346040),
                            MimeType = "application/gzip",
                            Links = null,
                            AllFilesZipSize = 346040
                        }
                    }
                },
                new ShowDailyFilesResponseModel
                {
                    YearWeek = "2022 / 18",
                    WeekNumber = "18",
                    Year = "2022",
                    DailyFilesData = new()
                    {
                        new DailyFilesDataModel()
                        {
                            DataDate = "2022-04-24",
                            BatchId = "68970ffc-4820-47eb-be76-aaa3209eb3b6",
                            Filename = "Daily 24-04-22.zip",
                            FileDescription = "Daily 24-04-22.zip",
                            FileExtension = ".zip",
                            FileSizeInKB = FileHelper.FormatSize(299170),
                            MimeType = "application/gzip",
                            Links = null,
                            AllFilesZipSize = 299170
                        },
                        new DailyFilesDataModel()
                        {
                            DataDate = "2022-04-23",
                            BatchId = "2cd869e1-a1e2-4a7d-94bb-1f60fddec9fe",
                            Filename = "Daily 23-04-22.zip",
                            FileDescription = "Daily 23-04-22.zip",
                            FileExtension = ".zip",
                            FileSizeInKB = FileHelper.FormatSize(346040),
                            MimeType = "application/gzip",
                            Links = null,
                            AllFilesZipSize = 346040
                        }
                    }
                }
            };
            expected = expected.OrderByDescending(x => x.Year).ThenByDescending(x => x.WeekNumber).ToList();

            foreach (var item in expected)
            {
                item.DailyFilesData = item.DailyFilesData.OrderBy(x => Convert.ToDateTime(x.DataDate)).ToList();
            }

            List<ShowDailyFilesResponseModel> result = NMHelper.GetDailyShowFilesResponse(searchResult);
            Assert.Multiple(() =>
            {
                for (int i = 0; i < result.Count; i++)
                {
                    Assert.AreEqual(expected[i].Year, result[i].Year);
                    Assert.AreEqual(expected[i].YearWeek, result[i].YearWeek);
                    Assert.AreEqual(expected[i].WeekNumber, result[i].WeekNumber);

                    for (int j = 0; j < expected[i].DailyFilesData.Count; j++)
                    {
                        Assert.AreEqual(expected[i].DailyFilesData[j].BatchId, result[i].DailyFilesData[j].BatchId);
                        Assert.AreEqual(expected[i].DailyFilesData[j].DataDate, result[i].DailyFilesData[j].DataDate);
                        Assert.AreEqual(expected[i].DailyFilesData[j].Filename, result[i].DailyFilesData[j].Filename);
                        Assert.AreEqual(expected[i].DailyFilesData[j].FileDescription, result[i].DailyFilesData[j].FileDescription);
                        Assert.AreEqual(expected[i].DailyFilesData[j].FileExtension, result[i].DailyFilesData[j].FileExtension);
                        Assert.AreEqual(expected[i].DailyFilesData[j].FileSizeInKB, result[i].DailyFilesData[j].FileSizeInKB);
                        Assert.AreEqual(expected[i].DailyFilesData[j].MimeType, result[i].DailyFilesData[j].MimeType);
                        Assert.AreEqual(expected[i].DailyFilesData[j].Links, result[i].DailyFilesData[j].Links);
                        Assert.AreEqual(expected[i].DailyFilesData[j].AllFilesZipSize, result[i].DailyFilesData[j].AllFilesZipSize);
                    }
                }
            });
        }

        [Test]
        public void WhenGetFormattedDateIsCalled_ThenShouldReturnDateInExpectedFormat()
        {
            const string strDate = "2022-05-26";
            string actualDate = NMHelper.GetFormattedDate(strDate);
            const string expectedDate = "26-05-22";
            Assert.AreEqual(expectedDate, actualDate);
        }

        [Test]
        public void WhenGetFormattedDateIsCalled_ThenShouldReturnEmptyString()
        {
            const string strDate = "";
            string actualDate = NMHelper.GetFormattedDate(strDate);
            const string expectedDate = "";
            Assert.AreEqual(expectedDate, actualDate);
        }

        [Test]
        public void  WhenValidateParametersForDownloadSingleFileIsCalled_ThenShouldThrowException()
        {
            List<KeyValuePair<string, string>> parameters = new()
            {
                new KeyValuePair<string, string>("BatchId", null),
                new KeyValuePair<string, string>("FileName", "test.txt"),
                new KeyValuePair<string, string>("MimeType", "application/pdf")
            };

            ILogger<NMHelperTest> fakeLogger = A.Fake<ILogger<NMHelperTest>>();
            
            Assert.Throws<ArgumentNullException>(() =>NMHelper.ValidateParametersForDownloadSingleFile(parameters, string.Empty, fakeLogger));
        }

        [Test]
        public void WhenValidateParametersForDownloadSingleFileIsCalled_ThenShouldNotThrowException()
        {
            List<KeyValuePair<string, string>> parameters = new()
            {
                new KeyValuePair<string, string>("BatchId", "Affsd-asd-asda"),
                new KeyValuePair<string, string>("FileName", "test.txt"),
                new KeyValuePair<string, string>("MimeType", "application/pdf")
            };

            ILogger<NMHelperTest> fakeLogger = A.Fake<ILogger<NMHelperTest>>();

            Assert.DoesNotThrow(() => NMHelper.ValidateParametersForDownloadSingleFile(parameters, string.Empty, fakeLogger));
        }

        private static BatchSearchResponse SetSearchResultForWeekly()
        {
            BatchSearchResponse searchResult = new()
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

            return searchResult;
        }

        private static BatchSearchResponse SetSearchResultForDaily()
        {
            BatchSearchResponse searchResult = new()
            {

                Count = 2,
                Links = null,
                Total = 0,
                Entries = new List<BatchDetails>() {
                        new BatchDetails() {
                            BatchId = "2cd869e1-a1e2-4a7d-94bb-1f60fddec9fe",
                            AllFilesZipSize=346040,
                            Attributes = new List<BatchDetailsAttributes>()
                            {
                                new BatchDetailsAttributes("Data Date","2022-04-22"),
                                new BatchDetailsAttributes("Frequency","Daily"),
                                new BatchDetailsAttributes("Product Type","Notices to Mariners"),
                                new BatchDetailsAttributes("Week Number","17"),
                                new BatchDetailsAttributes("Year","2022"),
                                new BatchDetailsAttributes("Year / Week","2022 / 17"),

                            },
                            BusinessUnit = "TEST",
                            BatchPublishedDate = DateTime.Now,
                            ExpiryDate = DateTime.Now,
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
                            BatchId = "68970ffc-4820-47eb-be76-aaa3209eb3b6",
                            AllFilesZipSize=299170,
                            Attributes = new List<BatchDetailsAttributes>()
                            {
                                new BatchDetailsAttributes("Data Date","2022-04-21"),
                                new BatchDetailsAttributes("Frequency","Daily"),
                                new BatchDetailsAttributes("Product Type","Notices to Mariners"),
                                new BatchDetailsAttributes("Week Number","17"),
                                new BatchDetailsAttributes("Year","2022"),
                                new BatchDetailsAttributes("Year / Week","2022 / 17"),

                            },
                            BusinessUnit = "TEST",
                            BatchPublishedDate = DateTime.Now,
                            ExpiryDate = DateTime.Now,
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

                        },

                        new BatchDetails() {
                            BatchId = "2cd869e1-a1e2-4a7d-94bb-1f60fddec9fe",
                            AllFilesZipSize=346040,
                            Attributes = new List<BatchDetailsAttributes>()
                            {
                                new BatchDetailsAttributes("Data Date","2022-04-22"),
                                new BatchDetailsAttributes("Frequency","Daily"),
                                new BatchDetailsAttributes("Product Type","Notices to Mariners"),
                                new BatchDetailsAttributes("Week Number","17"),
                                new BatchDetailsAttributes("Year","2022"),
                                new BatchDetailsAttributes("Year / Week","2022 / 17"),

                            },
                            BusinessUnit = "TEST",
                            BatchPublishedDate = DateTime.Now,
                            ExpiryDate = DateTime.Now,
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
                            BatchId = "68970ffc-4820-47eb-be76-aaa3209eb3b6",
                            AllFilesZipSize=299170,
                            Attributes = new List<BatchDetailsAttributes>()
                            {
                                new BatchDetailsAttributes("Data Date","2022-04-24"),
                                new BatchDetailsAttributes("Frequency","Daily"),
                                new BatchDetailsAttributes("Product Type","Notices to Mariners"),
                                new BatchDetailsAttributes("Week Number","18"),
                                new BatchDetailsAttributes("Year","2022"),
                                new BatchDetailsAttributes("Year / Week","2022 / 18"),

                            },
                            BusinessUnit = "TEST",
                            BatchPublishedDate = DateTime.Now,
                            ExpiryDate = DateTime.Now,
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

                        },
                        new BatchDetails() {
                            BatchId = "2cd869e1-a1e2-4a7d-94bb-1f60fddec9fe",
                            AllFilesZipSize=346040,
                            Attributes = new List<BatchDetailsAttributes>()
                            {
                                new BatchDetailsAttributes("Data Date","2022-04-23"),
                                new BatchDetailsAttributes("Frequency","Daily"),
                                new BatchDetailsAttributes("Product Type","Notices to Mariners"),
                                new BatchDetailsAttributes("Week Number","18"),
                                new BatchDetailsAttributes("Year","2022"),
                                new BatchDetailsAttributes("Year / Week","2022 / 18"),

                            },
                            BusinessUnit = "TEST",
                            BatchPublishedDate = DateTime.Now,
                            ExpiryDate = DateTime.Now,
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

                        }
                    }
            };

            return searchResult;
        }

    }
}
