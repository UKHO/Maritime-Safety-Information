using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UKHO.FileShareClient.Models;
using UKHO.MaritimeSafetyInformation.Common.Helpers;
using UKHO.MaritimeSafetyInformation.Common.Models.NoticesToMariners;

namespace UKHO.MaritimeSafetyInformation.Common.UnitTests.Helpers
{
    public class NMHelperTest
    {
        [Test]
        public void WhenNMHelperCallsListFilesResponseForPublicUser_ThenConversionIsCorrect()
        {
            BatchSearchResponse searchResult = SetSearchResultForWeeklyForPublicUser();

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
                    Links = null,
                    IsDistributorUser = false,
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
                    Links = null,
                    IsDistributorUser = false
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
                    Links = null,
                    IsDistributorUser = false
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
                    Links = null,
                    IsDistributorUser = false
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
        public void WhenNMHelperCallsListFilesResponseForDistributorUser_ThenConversionIsCorrect()
        {
            BatchSearchResponse searchResult = SetSearchResultForWeeklyForDistributorUser();

            List<ShowFilesResponseModel> expected = new()
            {
                new ShowFilesResponseModel()
                {
                    BatchId = "1",
                    Filename = "aaa.xml",
                    FileDescription = "aaa",
                    FileExtension = ".xml",
                    FileSize = 4545,
                    FileSizeinKB = "4 KB",
                    MimeType = "XML",
                    Links = null,
                    IsDistributorUser = true,
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
                    Links = null,
                    IsDistributorUser = true
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
                    Links = null,
                    IsDistributorUser = false
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
                    Links = null,
                    IsDistributorUser = false
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
                    Assert.AreEqual(expected[i].IsDistributorUser, result[i].IsDistributorUser);
                }
            });
        }

        [Test]
        public void WhenNMHelperCallsListFilesResponseForPublicUserWithDuplicateData_ThenReturnLatestData()
        {
            BatchSearchResponse searchResult = SetSearchResultDuplicateDataForWeeklyForPublicUser();

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
        public void WhenNMHelperCallsListFilesResponseForDistributorUserWithDuplicateData_ThenReturnLatestData()
        {
            BatchSearchResponse searchResult = SetSearchResultDuplicateDataWeeklyForDistributorUser();

            List<ShowFilesResponseModel> expected = new()
            {
                new ShowFilesResponseModel()
                {
                    BatchId = "2",
                    Filename = "ccc.xml",
                    FileDescription = "ccc",
                    FileExtension = ".xml",
                    FileSize = 1232,
                    FileSizeinKB = "1 KB",
                    MimeType = "XML",
                    Links = null,
                    IsDistributorUser = true
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
                    Links = null,
                    IsDistributorUser = true
                },
                new ShowFilesResponseModel()
                {
                    BatchId = "3",
                    Filename = "yyy.pdf",
                    FileDescription = "yyy",
                    FileExtension = ".pdf",
                    FileSize = 4545,
                    FileSizeinKB = "4 KB",
                    MimeType = "PDF",
                    Links = null,
                    IsDistributorUser = false
                },
                new ShowFilesResponseModel()
                {
                    BatchId = "3",
                    Filename = "xxx.pdf",
                    FileDescription = "xxx",
                    FileExtension = ".pdf",
                    FileSize = 8998,
                    FileSizeinKB = "9 KB",
                    MimeType = "PDF",
                    Links = null,
                    IsDistributorUser = false
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
                    Assert.AreEqual(expected[i].IsDistributorUser, result[i].IsDistributorUser);
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
                    YearWeek = "2022/17",
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
                    YearWeek = "2022/18",
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
        public void WhenGetDailyShowFilesResponseIsCalledWithDuplicateData_ThenLatestDistinctIsReturned()
        {
            BatchSearchResponse searchResult = SetSearchResultDuplicateDataForDaily();

            List<ShowDailyFilesResponseModel> expected = new()
            {
                new ShowDailyFilesResponseModel
                {
                    YearWeek = "2022/07",
                    WeekNumber = "07",
                    Year = "2022",
                    DailyFilesData = new()
                    {
                        new DailyFilesDataModel()
                        {
                            DataDate = "2022-02-15",
                            BatchId = "8cd563e1-a8e4-4a7d-84bb-3f60fddec0fe",
                            Filename = "Daily 15-02-22.zip",
                            FileDescription = "Daily 15-02-22.zip",
                            FileExtension = ".zip",
                            FileSizeInKB = FileHelper.FormatSize(400040),
                            MimeType = "application/gzip",
                            Links = null,
                            AllFilesZipSize = 400040
                        },
                        new DailyFilesDataModel()
                        {
                            DataDate = "2022-02-16",
                            BatchId = "9cd969e6-a1e1-7a7d-44bb-6f60fddec9fe",
                            Filename = "Daily 16-02-22.zip",
                            FileDescription = "Daily 16-02-22.zip",
                            FileExtension = ".zip",
                            FileSizeInKB = FileHelper.FormatSize(300040),
                            MimeType = "application/gzip",
                            Links = null,
                            AllFilesZipSize = 300040
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
        public void WhenValidateParametersForDownloadSingleFileIsCalled_ThenShouldThrowException()
        {
            List<KeyValuePair<string, string>> parameters = new()
            {
                new KeyValuePair<string, string>("BatchId", null),
                new KeyValuePair<string, string>("FileName", "test.txt"),
                new KeyValuePair<string, string>("MimeType", "application/pdf")
            };

            ILogger<NMHelperTest> fakeLogger = A.Fake<ILogger<NMHelperTest>>();

            Assert.Throws<ArgumentNullException>(() => NMHelper.ValidateParametersForDownloadSingleFile(parameters, string.Empty, fakeLogger));
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

        [Test]
        public async Task WhenGetFileBytesFromStreamIsCalled_ThenShouldReturnByte()
        {
            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes("test stream"));
            byte[] result = await NMHelper.GetFileBytesFromStream(stream);
            Assert.IsInstanceOf(typeof(byte[]), result);
        }

        private static BatchSearchResponse SetSearchResultForWeeklyForPublicUser()
        {
            BatchSearchResponse searchResult = new()
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

            List<ShowFilesResponseModel> result = NMHelper.GetShowFilesResponseModel(searchResult.Entries);

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
        public void WhenListFilesResponseLeisureIsCalled_ThenCheckIfConversionIsCorrect()
        {
            BatchSearchResponse searchResult = SetSearchResultForLeisure();

            List<ShowFilesResponseModel> expected = new()
            {
                new ShowFilesResponseModel()
                {
                    BatchId = "e22bf7c7-4c1c-424a-8aa2-8594ce98e233",
                    Filename = "SC5622 Ireland - West Coast.pdf",
                    FileDescription = "SC5622 Ireland - West Coast",
                    FileExtension = ".pdf",
                    FileSize = 636436,
                    FileSizeinKB = "622 KB",
                    MimeType = "application/pdf",
                    Links = null
                },
                new ShowFilesResponseModel()
                {
                    BatchId = "2cd869e1-a1e2-4a7d-94bb-1f60fddec9fe",
                    Filename = "SC5623 Ireland - South West Coast.pdf",
                    FileDescription = "SC5623 Ireland - South West Coast",
                    FileExtension = ".pdf",
                    FileSize = 636436,
                    FileSizeinKB = "622 KB",
                    MimeType = "application/pdf",
                    Links = null
                Count = 2,
                Links = null,
                Total = 0,
                Entries = new List<BatchDetails>() {
                    new BatchDetails() {
                        BatchId = "1",
                        Attributes = new List<BatchDetailsAttributes> { new BatchDetailsAttributes() {  Key = "Data Date" , Value =  "2022-06-20" },
                                                                                new BatchDetailsAttributes() { Key = "Frequency" , Value =  "Weekly" },
                                                                                new BatchDetailsAttributes() { Key = "Product Type" , Value = "NMTest" },
                                                                                new BatchDetailsAttributes() { Key = "Week Number", Value = "25" },
                                                                                new BatchDetailsAttributes() { Key = "Year", Value =  "2022"  },
                                                                                new BatchDetailsAttributes() { Key = "YEAR/WEEK", Value =  "2022 / 25"  } },
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
                        Attributes = new List<BatchDetailsAttributes> {new BatchDetailsAttributes() {  Key = "Data Date" , Value =  "2022-06-20" },
                                                                                new BatchDetailsAttributes() { Key = "Frequency" , Value =  "Weekly" },
                                                                                new BatchDetailsAttributes() { Key = "Product Type" , Value = "NMTest" },
                                                                                new BatchDetailsAttributes() { Key = "Week Number", Value = "25" },
                                                                                new BatchDetailsAttributes() { Key = "Year", Value =  "2022"  },
                                                                                new BatchDetailsAttributes() { Key = "YEAR/WEEK", Value =  "2022 / 25"  } },
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

        private static BatchSearchResponse SetSearchResultForWeeklyForDistributorUser()
        {
            BatchSearchResponse searchResult = new()
            {
                new ShowFilesResponseModel()
                {
                    BatchId = "e22bf7c7-4c1c-424a-8aa2-8594ce98e233",
                    Filename = "SC5622 Ireland - West Coast.pdf",
                    FileDescription = "SC5622 Ireland - West Coast",
                    FileExtension = ".pdf",
                    FileSize = 636436,
                    FileSizeinKB = "622 KB",
                    MimeType = "application/pdf",
                    Links = null
                },
                new ShowFilesResponseModel()
                {
                    BatchId = "2cd869e1-a1e2-4a7d-94bb-1f60fddec9fe",
                    Filename = "SC5623 Ireland - South West Coast.pdf",
                    FileDescription = "SC5623 Ireland - South West Coast",
                    FileExtension = ".pdf",
                    FileSize = 636436,
                    FileSizeinKB = "622 KB",
                    MimeType = "application/pdf",
                    Links = null
                }
            };

            List<ShowFilesResponseModel> result = NMHelper.ListFilesResponseLeisure(searchResult);

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
        public void WhenCallsListFilesResponseCumulative_ThenConversionIsCorrect()
        {
            BatchSearchResponse searchResult = SetSearchResultForCumulative();

            List<ShowFilesResponseModel> expected = new()
            {
                new ShowFilesResponseModel()
                {

                    BatchId = "2",
                    Filename = "NP234(B) 2022.pdf",
                    FileDescription = "NP234(B) 2022",
                    FileExtension = ".pdf",
                    FileSize = 1232,
                    FileSizeinKB = "1 KB",
                    MimeType = "PDF",
                    Links = null
                },
                new ShowFilesResponseModel()
                {
                    BatchId = "1",
                    Filename = "NP234(A) 2022.pdf",
                    FileDescription = "NP234(A) 2022",
                    FileExtension = ".pdf",
                    FileSize = 1232,
                    FileSizeinKB = "1 KB",
                    MimeType = "PDF",
                    Links = null
                },
                new ShowFilesResponseModel()
                {
                    BatchId = "4",
                    Filename = "NP234(B) 2021.pdf",
                    FileDescription = "NP234(B) 2021",
                    FileExtension = ".pdf",
                    FileSize = 1232,
                    FileSizeinKB = "1 KB",
                    MimeType = "PDF",
                    Links = null
                },
                new ShowFilesResponseModel()
                {
                    BatchId = "3",
                    Filename = "NP234(A) 2021.pdf",
                    FileDescription = "NP234(A) 2021",
                    FileExtension = ".pdf",
                    FileSize = 1232,
                    FileSizeinKB = "1 KB",
                    MimeType = "PDF",
                    Links = null
                }
            };

            List<ShowFilesResponseModel> result = NMHelper.ListFilesResponseCumulative(searchResult.Entries);

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
        public void WhenCallsListFilesResponseCumulativeWithDuplicateData_ThenConversionIsCorrect()
        {
            BatchSearchResponse searchResult = SetSearchResultForDuplicateCumulative();

            List<ShowFilesResponseModel> expected = new()
            {
                new ShowFilesResponseModel()
                {
                    BatchId = "2",
                    Filename = "NP234(B) 2022.pdf",
                    FileDescription = "NP234(B) 2022",
                    FileExtension = ".pdf",
                    FileSize = 1232,
                    FileSizeinKB = "1 KB",
                    MimeType = "PDF",
                    Links = null
                },
                new ShowFilesResponseModel()
                {
                    BatchId = "1",
                    Filename = "NP234(A) 2022.pdf",
                    FileDescription = "NP234(A) 2022",
                    FileExtension = ".pdf",
                    FileSize = 1232,
                    FileSizeinKB = "1 KB",
                    MimeType = "PDF",
                    Links = null
                },
                new ShowFilesResponseModel()
                {
                    BatchId = "4",
                    Filename = "NP234(B) 2021.pdf",
                    FileDescription = "NP234(B) 2021",
                    FileExtension = ".pdf",
                    FileSize = 1232,
                    FileSizeinKB = "1 KB",
                    MimeType = "PDF",
                    Links = null
                },
                new ShowFilesResponseModel()
                {
                    BatchId = "3",
                    Filename = "NP234(A) 2021.pdf",
                    FileDescription = "NP234(A) 2021",
                    FileExtension = ".pdf",
                    FileSize = 1232,
                    FileSizeinKB = "1 KB",
                    MimeType = "PDF",
                    Links = null
                Count = 2,
                Links = null,
                Total = 0,
                Entries = new List<BatchDetails>() {
                    new BatchDetails() {
                        BatchId = "1",
                        Attributes = new List<BatchDetailsAttributes> {new BatchDetailsAttributes() {  Key = "Content" , Value =  "tracings" },
                                                                            new BatchDetailsAttributes() {  Key = "Data Date" , Value =  "2022-06-20" },
                                                                            new BatchDetailsAttributes() { Key = "Frequency" , Value =  "Weekly" },
                                                                            new BatchDetailsAttributes() { Key = "Product Type" , Value = "NMTest" },
                                                                            new BatchDetailsAttributes() { Key = "Week Number", Value = "25" },
                                                                            new BatchDetailsAttributes() { Key = "Year", Value =  "2022"  },
                                                                            new BatchDetailsAttributes() { Key = "YEAR/WEEK", Value =  "2022 / 25"  } },
                        Files = new List<BatchDetailsFiles>() {
                            new BatchDetailsFiles () {
                                Filename = "aaa.xml",
                                FileSize=4545,
                                MimeType = "XML",
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
                        Attributes = new List<BatchDetailsAttributes> {new BatchDetailsAttributes() {  Key = "Data Date" , Value =  "2022-06-20" },
                                                                                new BatchDetailsAttributes() { Key = "Frequency" , Value =  "Weekly" },
                                                                                new BatchDetailsAttributes() { Key = "Product Type" , Value = "NMTest" },
                                                                                new BatchDetailsAttributes() { Key = "Week Number", Value = "25" },
                                                                                new BatchDetailsAttributes() { Key = "Year", Value =  "2022"  },
                                                                                new BatchDetailsAttributes() { Key = "YEAR/WEEK", Value =  "2022 / 25"  } },
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

            List<ShowFilesResponseModel> result = NMHelper.ListFilesResponseCumulative(searchResult.Entries);

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
        public void WhenGetShowAnnualFilesResponseIsCalled_ThenDataShouldBeOrderByFileName()
        {
            BatchSearchResponse searchResult = SetSearchResultForAnnual();

            List<ShowFilesResponseModel> expected = new()
            {
                new ShowFilesResponseModel()
                {

                    BatchId = "6",
                    Filename = "00 NP234(A) 2022.pdf",
                    FileDescription = "NP234(A) 2022",
                    FileExtension = ".pdf",
                    FileSize = 1232,
                    FileSizeinKB = "1 KB",
                    MimeType = "PDF",
                    Links = null,
                    Hash = "---"
                },
                new ShowFilesResponseModel()
                {
                    BatchId = "4",
                    Filename = "01 NP234(A) 2022.pdf",
                    FileDescription = "NP234(A) 2022",
                    FileExtension = ".pdf",
                    FileSize = 1232,
                    FileSizeinKB = "1 KB",
                    MimeType = "PDF",
                    Links = null,
                    Hash = "1"
                },
                new ShowFilesResponseModel()
                {
                    BatchId = "5",
                    Filename = "02 NP234(A) 2022.pdf",
                    FileDescription = "NP234(A) 2022",
                    FileExtension = ".pdf",
                    FileSize = 1232,
                    FileSizeinKB = "1 KB",
                    MimeType = "PDF",
                    Links = null,
                    Hash = "2"
                },
                new ShowFilesResponseModel()
                {
                    BatchId = "3",
                    Filename = "26 NP234(A) 2022.pdf",
                    FileDescription = "NP234(A) 2022",
                    FileExtension = ".pdf",
                    FileSize = 1232,
                    FileSizeinKB = "1 KB",
                    MimeType = "PDF",
                    Links = null,
                    Hash = "26"
                },
                new ShowFilesResponseModel()
                {
                    BatchId = "2",
                    Filename = "27 NP234(A) 2022.pdf",
                    FileDescription = "NP234(A) 2022",
                    FileExtension = ".pdf",
                    FileSize = 1232,
                    FileSizeinKB = "1 KB",
                    MimeType = "PDF",
                    Links = null,
                    Hash = "---"
                },
                new ShowFilesResponseModel()
                {
                    BatchId = "1",
                    Filename = "28 NP234(A) 2022.pdf",
                    FileDescription = "NP234(A) 2022",
                    FileExtension = ".pdf",
                    FileSize = 1232,
                    FileSizeinKB = "1 KB",
                    MimeType = "PDF",
                    Links = null,
                    Hash = "---"
                }
            };

            List<ShowFilesResponseModel> result = NMHelper.GetShowAnnualFilesResponse(searchResult.Entries);

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
                    Assert.AreEqual(expected[i].Hash, result[i].Hash);
                }
            });
        }

        [TestCase("01 NP234(A) 2022.pdf", "filename", ExpectedResult = "NP234(A) 2022", Description = "When GetShowAnnualFilesResponse Is Called With Filename Then Should Return Proper Filename")]
        [TestCase("", "filename", ExpectedResult = "", Description = "When GetShowAnnualFilesResponse Is Called Empty Filename Then Should Return  Empty")]
        [TestCase("01 NP234(A) 2022.pdf", "section", ExpectedResult = "1", Description = "When GetShowAnnualFilesResponse Is Called With Filename Then Should Return Expected Section")]
        [TestCase("00 NP234(A) 2022.pdf", "section", ExpectedResult = "---", Description = "When GetShowAnnualFilesResponse Is Called With Filename Then Should Return Expected Section")]
        [TestCase("26 NP234(A) 2022.pdf", "section", ExpectedResult = "26", Description = "When GetShowAnnualFilesResponse Is Called With Filename Then Should Return Expected Section")]
        [TestCase("27 NP234(A) 2022.pdf", "section", ExpectedResult = "---", Description = "When GetShowAnnualFilesResponse Is Called With Filename Then Should Return Expected Section")]
        [TestCase("28 NP234(A) 2022.pdf", "section", ExpectedResult = "---", Description = "When GetShowAnnualFilesResponse Is Called With Filename Then Should Return Expected Section")]
        [TestCase("", "section", ExpectedResult = "", Description = "When GetShowAnnualFilesResponse Is Called Without Filename Then Should Return Empty Section")]
        [TestCase("01 NP234(A) 2022.pdf", "", ExpectedResult = "", Description = "When GetShowAnnualFilesResponse Is Called With Filename But Empty type Then Should Return Empty")]
        [TestCase("", "", ExpectedResult = "", Description = "When GetShowAnnualFilesResponse Is Called With Empty Filename And type Then Should Return Empty")]
        public string WhenGetAnnualFileNameAndSection_ThenShouldReturnExpectedResult(string fileName, string type)
        {
            return NMHelper.GetAnnualFileNameAndSection(fileName, type);
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
                        Attributes = new List<BatchDetailsAttributes> { new BatchDetailsAttributes() {  Key = "Data Date" , Value =  "2022-04-08" },
                                                                                new BatchDetailsAttributes() { Key = "Frequency" , Value =  "Weekly" },
                                                                                new BatchDetailsAttributes() { Key = "Product Type" , Value = "NMTest" },
                                                                                new BatchDetailsAttributes() { Key = "Week Number", Value = "14" },
                                                                                new BatchDetailsAttributes() { Key = "Year", Value =  "2022"  },
                                                                                new BatchDetailsAttributes() { Key = "YEAR/WEEK", Value =  "2022 / 14"  } },
                        BatchPublishedDate = DateTime.Now,
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
                        Attributes = new List<BatchDetailsAttributes> { new BatchDetailsAttributes() {  Key = "Data Date" , Value =  "2022-04-08" },
                                                                                new BatchDetailsAttributes() { Key = "Frequency" , Value =  "Weekly" },
                                                                                new BatchDetailsAttributes() { Key = "Product Type" , Value = "NMTest" },
                                                                                new BatchDetailsAttributes() { Key = "Week Number", Value = "14" },
                                                                                new BatchDetailsAttributes() { Key = "Year", Value =  "2022"  },
                                                                                new BatchDetailsAttributes() { Key = "YEAR/WEEK", Value =  "2022 / 14"  } },
                        BatchPublishedDate= DateTime.Now.AddMinutes(-10),
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

        private static BatchSearchResponse SetSearchResultDuplicateDataWeeklyForDistributorUser()
        {
            BatchSearchResponse searchResult = new()
            {
                Count = 4,
                Links = null,
                Total = 0,
                Entries = new List<BatchDetails>() {
                    new BatchDetails() {
                        BatchId = "1",
                        Attributes = new List<BatchDetailsAttributes>{new BatchDetailsAttributes() {  Key = "Content" , Value =  "tracings" },
                                                                      new BatchDetailsAttributes() {  Key = "Data Date" , Value =  "2022-04-08" },
                                                                                new BatchDetailsAttributes() { Key = "Frequency" , Value =  "Weekly" },
                                                                                new BatchDetailsAttributes() { Key = "Product Type" , Value = "NMTest" },
                                                                                new BatchDetailsAttributes() { Key = "Week Number", Value = "14" },
                                                                                new BatchDetailsAttributes() { Key = "Year", Value =  "2022"  },
                                                                                new BatchDetailsAttributes() { Key = "YEAR/WEEK", Value =  "2022 / 14"  } },
                        BatchPublishedDate= DateTime.Now.AddMinutes(-10),                        
                        Files = new List<BatchDetailsFiles>() {
                            new BatchDetailsFiles () {
                                Filename = "aaa.pdf",
                                FileSize=1232,
                                MimeType = "PDF",
                                Links = null
                            },
                            new BatchDetailsFiles () {
                                Filename = "bbb.txt",
                                FileSize=1232,
                                MimeType = "TEXT",
                                Links = null
                            }
                        }
                    },
                    new BatchDetails() {
                        BatchId = "2",
                        Attributes = new List<BatchDetailsAttributes> {new BatchDetailsAttributes() {  Key = "Content" , Value =  "tracings" },
                                                                                new BatchDetailsAttributes() {  Key = "Data Date" , Value =  "2022-04-08" },
                                                                                new BatchDetailsAttributes() { Key = "Frequency" , Value =  "Weekly" },
                                                                                new BatchDetailsAttributes() { Key = "Product Type" , Value = "NMTest" },
                                                                                new BatchDetailsAttributes() { Key = "Week Number", Value = "14" },
                                                                                new BatchDetailsAttributes() { Key = "Year", Value =  "2022"  },
                                                                                new BatchDetailsAttributes() { Key = "YEAR/WEEK", Value =  "2022 / 14"  } },
                        BatchPublishedDate = DateTime.Now,
                        Files = new List<BatchDetailsFiles>() {
                            new BatchDetailsFiles () {
                                Filename = "ccc.xml",
                                FileSize=1232,
                                MimeType = "XML",
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
                        BatchId = "3",
                        Attributes = new List<BatchDetailsAttributes> { new BatchDetailsAttributes() {  Key = "Data Date" , Value =  "2022-04-08" },
                                                                                new BatchDetailsAttributes() { Key = "Frequency" , Value =  "Weekly" },
                                                                                new BatchDetailsAttributes() { Key = "Product Type" , Value = "NMTest" },
                                                                                new BatchDetailsAttributes() { Key = "Week Number", Value = "14" },
                                                                                new BatchDetailsAttributes() { Key = "Year", Value =  "2022"  },
                                                                                new BatchDetailsAttributes() { Key = "YEAR/WEEK", Value =  "2022 / 14"  } },
                        BatchPublishedDate = DateTime.Now,
                        Files = new List<BatchDetailsFiles>() {
                            new BatchDetailsFiles () {
                                Filename = "yyy.pdf",
                                FileSize = 4545,
                                MimeType = "PDF",
                                Links = null
                            },
                            new BatchDetailsFiles () {
                                Filename = "xxx.pdf",
                                FileSize = 8998,
                                MimeType = "PDF",
                                Links = null
                            }
                        }

                    },
                    new BatchDetails() {
                        BatchId = "4",
                        Attributes = new List<BatchDetailsAttributes> { new BatchDetailsAttributes() {  Key = "Data Date" , Value =  "2022-04-08" },
                                                                                new BatchDetailsAttributes() { Key = "Frequency" , Value =  "Weekly" },
                                                                                new BatchDetailsAttributes() { Key = "Product Type" , Value = "NMTest" },
                                                                                new BatchDetailsAttributes() { Key = "Week Number", Value = "14" },
                                                                                new BatchDetailsAttributes() { Key = "Year", Value =  "2022"  },
                                                                                new BatchDetailsAttributes() { Key = "YEAR/WEEK", Value =  "2022 / 14"  } },
                        BatchPublishedDate= DateTime.Now.AddMinutes(-10),
                        Files = new List<BatchDetailsFiles>() {
                            new BatchDetailsFiles () {
                                Filename = "mmm.pdf",
                                FileSize = 5445,
                                MimeType = "PDF",
                                Links = null
                            },
                            new BatchDetailsFiles () {
                                Filename = "nnn.pdf",
                                FileSize = 8998,
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

        private static BatchSearchResponse SetSearchResultDuplicateDataForDaily()
        {
            BatchSearchResponse searchResult = new()
            {
                Count = 1,
                Links = null,
                Total = 0,
                Entries = new List<BatchDetails>() {
                        new BatchDetails() {
                            BatchId = "75920ffc-4820-47eb-be76-aaa3209eb3b6",
                            AllFilesZipSize=299170,
                            Attributes = new List<BatchDetailsAttributes>()
                            {
                                new BatchDetailsAttributes("Data Date","2022-02-15"),
                                new BatchDetailsAttributes("Frequency","Daily"),
                                new BatchDetailsAttributes("Product Type","Notices to Mariners"),
                                new BatchDetailsAttributes("Week Number","07"),
                                new BatchDetailsAttributes("Year","2022"),
                                new BatchDetailsAttributes("Year / Week","2022 / 07"),

                            },
                            BusinessUnit = "TEST",
                            BatchPublishedDate = DateTime.Now.AddMinutes(-10),
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
                            BatchId = "1cd579e1-a1e2-4a7d-94bb-1f90fddec7fe",
                            AllFilesZipSize=60040,
                            Attributes = new List<BatchDetailsAttributes>()
                            {
                                new BatchDetailsAttributes("Data Date","2022-02-15"),
                                new BatchDetailsAttributes("Frequency","Daily"),
                                new BatchDetailsAttributes("Product Type","Notices to Mariners"),
                                new BatchDetailsAttributes("Week Number","07"),
                                new BatchDetailsAttributes("Year","2022"),
                                new BatchDetailsAttributes("Year / Week","2022 / 07"),

                            },
                            BusinessUnit = "TEST",
                            BatchPublishedDate = DateTime.Now.AddMinutes(-20),
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
                            BatchId = "8cd563e1-a8e4-4a7d-84bb-3f60fddec0fe",
                            AllFilesZipSize=400040,
                            Attributes = new List<BatchDetailsAttributes>()
                            {
                                new BatchDetailsAttributes("Data Date","2022-02-15"),
                                new BatchDetailsAttributes("Frequency","Daily"),
                                new BatchDetailsAttributes("Product Type","Notices to Mariners"),
                                new BatchDetailsAttributes("Week Number","07"),
                                new BatchDetailsAttributes("Year","2022"),
                                new BatchDetailsAttributes("Year / Week","2022 / 07"),

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
                            BatchId = "9cd969e6-a1e1-7a7d-44bb-6f60fddec9fe",
                            AllFilesZipSize=300040,
                            Attributes = new List<BatchDetailsAttributes>()
                            {
                                new BatchDetailsAttributes("Data Date","2022-02-16"),
                                new BatchDetailsAttributes("Frequency","Daily"),
                                new BatchDetailsAttributes("Product Type","Notices to Mariners"),
                                new BatchDetailsAttributes("Week Number","07"),
                                new BatchDetailsAttributes("Year","2022"),
                                new BatchDetailsAttributes("Year / Week","2022 / 07"),

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

                        }
                }
            };

            return searchResult;
        }

        private static BatchSearchResponse SetSearchResultForCumulative()
        {
            BatchSearchResponse searchResult = new()
            {
                Count = 4,
                Links = null,
                Total = 0,
                Entries = new List<BatchDetails>() {
                        new BatchDetails() {
                            BatchId = "1",
                              Attributes = new List<BatchDetailsAttributes>()
                            {
                                new BatchDetailsAttributes("Data Date","2022-01-22"),
                                new BatchDetailsAttributes("Frequency","Cumulative"),
                                new BatchDetailsAttributes("Product Type","Notices to Mariners"),
                                new BatchDetailsAttributes("Year","2022"),

                            },
                            BatchPublishedDate = DateTime.Now,
                            Files = new List<BatchDetailsFiles>() {
                                new BatchDetailsFiles () {
                                    Filename = "NP234(A) 2022.pdf",
                                    FileSize=1232,
                                    MimeType = "PDF",
                                    Links = null
                                }
                            }
                        },
                           new BatchDetails() {
                            BatchId = "2",
                             Attributes = new List<BatchDetailsAttributes>()
                            {
                                new BatchDetailsAttributes("Data Date","2022-06-21"),
                                new BatchDetailsAttributes("Frequency","Cumulative"),
                                new BatchDetailsAttributes("Product Type","Notices to Mariners"),
                                new BatchDetailsAttributes("Year","2022"),

                            },
                            BatchPublishedDate = DateTime.Now,
                            Files = new List<BatchDetailsFiles>() {
                                new BatchDetailsFiles () {
                                    Filename = "NP234(B) 2022.pdf",
                                    FileSize=1232,
                                    MimeType = "PDF",
                                    Links = null
                                }
                            }
                        },
                        new BatchDetails() {
                            BatchId = "3",
                            Attributes = new List<BatchDetailsAttributes>()
                            {
                                new BatchDetailsAttributes("Data Date","2021-01-20"),
                                new BatchDetailsAttributes("Frequency","Cumulative"),
                                new BatchDetailsAttributes("Product Type","Notices to Mariners"),
                                new BatchDetailsAttributes("Year","2021"),

                            },
                            BatchPublishedDate = DateTime.Now,
                            Files = new List<BatchDetailsFiles>() {
                                new BatchDetailsFiles () {
                                    Filename = "NP234(A) 2021.pdf",
                                    FileSize=1232,
                                    MimeType = "PDF",
                                    Links = null
                                }
                            }
                        },
                        new BatchDetails() {
                            BatchId = "4",
                            Attributes = new List<BatchDetailsAttributes>()
                            {
                                new BatchDetailsAttributes("Data Date","2021-06-19"),
                                new BatchDetailsAttributes("Frequency","Cumulative"),
                                new BatchDetailsAttributes("Product Type","Notices to Mariners"),
                                new BatchDetailsAttributes("Year","2021"),

                            },
                            BatchPublishedDate = DateTime.Now,
                            Files = new List<BatchDetailsFiles>() {
                                new BatchDetailsFiles () {
                                    Filename = "NP234(B) 2021.pdf",
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

        private static BatchSearchResponse SetSearchResultForDuplicateCumulative()
        {
            BatchSearchResponse searchResult = new()
            {
                Count = 6,
                Links = null,
                Total = 0,
                Entries = new List<BatchDetails>() {
                        new BatchDetails() {
                            BatchId = "1",
                              Attributes = new List<BatchDetailsAttributes>()
                            {
                                new BatchDetailsAttributes("Data Date","2022-01-22"),
                                new BatchDetailsAttributes("Frequency","Cumulative"),
                                new BatchDetailsAttributes("Product Type","Notices to Mariners"),
                                new BatchDetailsAttributes("Year","2022"),

                            },
                            BatchPublishedDate = DateTime.Now,
                            Files = new List<BatchDetailsFiles>() {
                                new BatchDetailsFiles () {
                                    Filename = "NP234(A) 2022.pdf",
                                    FileSize=1232,
                                    MimeType = "PDF",
                                    Links = null
                                }
                            }
                        },
                           new BatchDetails() {
                            BatchId = "2",
                             Attributes = new List<BatchDetailsAttributes>()
                            {
                                new BatchDetailsAttributes("Data Date","2022-06-21"),
                                new BatchDetailsAttributes("Frequency","Cumulative"),
                                new BatchDetailsAttributes("Product Type","Notices to Mariners"),
                                new BatchDetailsAttributes("Year","2022"),
                            },
                            BatchPublishedDate = DateTime.Now,
                            Files = new List<BatchDetailsFiles>() {
                                new BatchDetailsFiles () {
                                    Filename = "NP234(B) 2022.pdf",
                                    FileSize=1232,
                                    MimeType = "PDF",
                                    Links = null
                                }
                            }
                        },
                            new BatchDetails() {
                            BatchId = "3",
                            Attributes = new List<BatchDetailsAttributes>()
                            {
                                new BatchDetailsAttributes("Data Date","2021-01-20"),
                                new BatchDetailsAttributes("Frequency","Cumulative"),
                                new BatchDetailsAttributes("Product Type","Notices to Mariners"),
                                new BatchDetailsAttributes("Year","2021"),
                            },
                            BatchPublishedDate = DateTime.Now.AddMinutes(-10),
                            Files = new List<BatchDetailsFiles>() {
                                new BatchDetailsFiles () {
                                    Filename = "NP234(A) 2021.pdf",
                                    FileSize=1232,
                                    MimeType = "PDF",
                                    Links = null
                                }
                            }
                        },
                        new BatchDetails() {
                            BatchId = "3",
                            Attributes = new List<BatchDetailsAttributes>()
                            {
                                new BatchDetailsAttributes("Data Date","2021-01-20"),
                                new BatchDetailsAttributes("Frequency","Cumulative"),
                                new BatchDetailsAttributes("Product Type","Notices to Mariners"),
                                new BatchDetailsAttributes("Year","2021"),
                            },
                            BatchPublishedDate = DateTime.Now,
                            Files = new List<BatchDetailsFiles>() {
                                new BatchDetailsFiles () {
                                    Filename = "NP234(A) 2021.pdf",
                                    FileSize=1232,
                                    MimeType = "PDF",
                                    Links = null
                                }
                            }
                        },
                        new BatchDetails() {
                            BatchId = "4",
                            Attributes = new List<BatchDetailsAttributes>()
                            {
                                new BatchDetailsAttributes("Data Date","2021-06-19"),
                                new BatchDetailsAttributes("Frequency","Cumulative"),
                                new BatchDetailsAttributes("Product Type","Notices to Mariners"),
                                new BatchDetailsAttributes("Year","2021"),
                            },
                            BatchPublishedDate = DateTime.Now,
                            Files = new List<BatchDetailsFiles>() {
                                new BatchDetailsFiles () {
                                    Filename = "NP234(B) 2021.pdf",
                                    FileSize=1232,
                                    MimeType = "PDF",
                                    Links = null
                                }
                            }
                        },
                         new BatchDetails() {
                            BatchId = "4",
                            Attributes = new List<BatchDetailsAttributes>()
                            {
                                new BatchDetailsAttributes("Data Date","2021-06-19"),
                                new BatchDetailsAttributes("Frequency","Cumulative"),
                                new BatchDetailsAttributes("Product Type","Notices to Mariners"),
                                new BatchDetailsAttributes("Year","2021"),
                            },
                            BatchPublishedDate = DateTime.Now.AddMinutes(-10),
                            Files = new List<BatchDetailsFiles>() {
                                new BatchDetailsFiles () {
                                    Filename = "NP234(B) 2021.pdf",
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

        private static BatchSearchResponse SetSearchResultForLeisure()
        {
            BatchSearchResponse searchResult = new()
            {
                Count = 2,
                Links = null,
                Total = 2,
                Entries = new List<BatchDetails>() {
                    new BatchDetails() {
                        BatchId = "2cd869e1-a1e2-4a7d-94bb-1f60fddec9fe",
                        AllFilesZipSize=346040,
                        Attributes = new List<BatchDetailsAttributes>()
                        {
                            new BatchDetailsAttributes("Chart","SC5623"),
                            new BatchDetailsAttributes("Data Date","2022-04-22"),
                            new BatchDetailsAttributes("Frequency","leisure"),
                            new BatchDetailsAttributes("Product Type","Notices to Mariners"),
                            new BatchDetailsAttributes("Year","2022")
                        },
                        BusinessUnit = "TEST",
                        BatchPublishedDate =Convert.ToDateTime("05-07-2022 13:25:35"),
                        ExpiryDate = DateTime.Now,
                        Files = new List<BatchDetailsFiles>() {
                            new BatchDetailsFiles () {
                                Filename = "SC5623 Ireland - South West Coast.pdf",
                                FileSize=636436,
                                MimeType = "application/pdf",
                                Links = null
                            }
                        }

                    },
                    new BatchDetails() {
                        BatchId = "e22bf7c7-4c1c-424a-8aa2-8594ce98e233",
                        AllFilesZipSize=346040,
                        Attributes = new List<BatchDetailsAttributes>()
                        {
                            new BatchDetailsAttributes("Chart","SC5622"),
                            new BatchDetailsAttributes("Data Date","2022-04-22"),
                            new BatchDetailsAttributes("Frequency","leisure"),
                            new BatchDetailsAttributes("Product Type","Notices to Mariners"),
                            new BatchDetailsAttributes("Year","2022")
                        },
                        BusinessUnit = "TEST",
                        BatchPublishedDate =Convert.ToDateTime("05-07-2022 14:25:35"),
                        ExpiryDate = DateTime.Now,
                        Files = new List<BatchDetailsFiles>() {
                            new BatchDetailsFiles () {
                                Filename = "SC5622 Ireland - West Coast.pdf",
                                FileSize=636436,
                                MimeType = "application/pdf",
                                Links = null
                            }
                        }

                    }
                }
            };

            return searchResult;
        }

        private static BatchSearchResponse SetSearchResultForDuplicateLeisure()
        {
            BatchSearchResponse searchResult = new()
            {
                Count = 3,
                Links = null,
                Total = 3,
                Entries = new List<BatchDetails>() {
                        new BatchDetails() {
                            BatchId = "2cd869e1-a1e2-4a7d-94bb-1f60fddec9fe",
                            AllFilesZipSize=346040,
                            Attributes = new List<BatchDetailsAttributes>()
                            {
                                new BatchDetailsAttributes("Chart","SC5623"),
                                new BatchDetailsAttributes("Data Date","2022-04-22"),
                                new BatchDetailsAttributes("Frequency","leisure"),
                                new BatchDetailsAttributes("Product Type","Notices to Mariners"),
                                new BatchDetailsAttributes("Year","2022")
                            },
                            BusinessUnit = "TEST",
                            BatchPublishedDate =Convert.ToDateTime("05-07-2022 13:25:35"),
                            ExpiryDate = DateTime.Now,
                            Files = new List<BatchDetailsFiles>() {
                                new BatchDetailsFiles () {
                                    Filename = "SC5623 Ireland - South West Coast.pdf",
                                    FileSize=636436,
                                    MimeType = "application/pdf",
                                    Links = null
                                }
                            }

                        },
                        new BatchDetails() {
                            BatchId = "e22bf7c7-4c1c-424a-8aa2-8594ce98e233",
                            AllFilesZipSize=346040,
                            Attributes = new List<BatchDetailsAttributes>()
                            {
                                new BatchDetailsAttributes("Chart","SC5622"),
                                new BatchDetailsAttributes("Data Date","2022-04-22"),
                                new BatchDetailsAttributes("Frequency","leisure"),
                                new BatchDetailsAttributes("Product Type","Notices to Mariners"),
                                new BatchDetailsAttributes("Year","2022")
                            },
                            BusinessUnit = "TEST",
                            BatchPublishedDate =Convert.ToDateTime("05-07-2022 14:25:35"),
                            ExpiryDate = DateTime.Now,
                            Files = new List<BatchDetailsFiles>() {
                                new BatchDetailsFiles () {
                                    Filename = "SC5622 Ireland - West Coast.pdf",
                                    FileSize=636436,
                                    MimeType = "application/pdf",
                                    Links = null
                                }
                            }

                        },
                        new BatchDetails() {
                            BatchId = "2cd869e1-a1e2-4a7d-94bb-1f60fddec9fe",
                            AllFilesZipSize=346040,
                            Attributes = new List<BatchDetailsAttributes>()
                            {
                                new BatchDetailsAttributes("Chart","SC5623"),
                                new BatchDetailsAttributes("Data Date","2022-04-22"),
                                new BatchDetailsAttributes("Frequency","leisure"),
                                new BatchDetailsAttributes("Product Type","Notices to Mariners"),
                                new BatchDetailsAttributes("Year","2022")
                            },
                            BusinessUnit = "TEST",
                            BatchPublishedDate =Convert.ToDateTime("06-07-2022 13:25:35"),
                            ExpiryDate = DateTime.Now,
                            Files = new List<BatchDetailsFiles>() {
                                new BatchDetailsFiles () {
                                    Filename = "SC5623 Ireland - South West Coast.pdf",
                                    FileSize=636436,
                                    MimeType = "application/pdf",
                                    Links = null
                                }
                            }

                        },
                    }
            };

            return searchResult;
        }

        private static BatchSearchResponse SetSearchResultForAnnual()
        {
            BatchSearchResponse searchResult = new()
            {
                Count = 15,
                Links = null,
                Total = 15,
                Entries = new List<BatchDetails>() {
                        new BatchDetails() {
                            BatchId = "1",
                              Attributes = new List<BatchDetailsAttributes>()
                            {
                                new BatchDetailsAttributes("Data Date","2022-01-22"),
                                new BatchDetailsAttributes("Frequency","Annual"),
                                new BatchDetailsAttributes("Product Type","Notices to Mariners"),
                                new BatchDetailsAttributes("Year","2022"),

                            },
                            BatchPublishedDate = DateTime.Now,
                            Files = new List<BatchDetailsFiles>() {
                                new BatchDetailsFiles () {
                                    Filename = "28 NP234(A) 2022.pdf",
                                    FileSize=1232,
                                    MimeType = "PDF",
                                    Links = null
                                }
                            }
                        },
                           new BatchDetails() {
                            BatchId = "2",
                             Attributes = new List<BatchDetailsAttributes>()
                            {
                                new BatchDetailsAttributes("Data Date","2022-06-21"),
                                new BatchDetailsAttributes("Frequency","Annual"),
                                new BatchDetailsAttributes("Product Type","Notices to Mariners"),
                                new BatchDetailsAttributes("Year","2022"),

                            },
                            BatchPublishedDate = DateTime.Now,
                            Files = new List<BatchDetailsFiles>() {
                                new BatchDetailsFiles () {
                                    Filename = "27 NP234(A) 2022.pdf",
                                    FileSize=1232,
                                    MimeType = "PDF",
                                    Links = null
                                }
                            }
                        },
                        new BatchDetails() {
                            BatchId = "3",
                            Attributes = new List<BatchDetailsAttributes>()
                            {
                                new BatchDetailsAttributes("Data Date","2021-01-20"),
                                new BatchDetailsAttributes("Frequency","Annual"),
                                new BatchDetailsAttributes("Product Type","Notices to Mariners"),
                                new BatchDetailsAttributes("Year","2021"),

                            },
                            BatchPublishedDate = DateTime.Now,
                            Files = new List<BatchDetailsFiles>() {
                                new BatchDetailsFiles () {
                                    Filename = "26 NP234(A) 2022.pdf",
                                    FileSize=1232,
                                    MimeType = "PDF",
                                    Links = null
                                }
                            }
                        },
                        new BatchDetails() {
                            BatchId = "4",
                            Attributes = new List<BatchDetailsAttributes>()
                            {
                                new BatchDetailsAttributes("Data Date","2021-06-19"),
                                new BatchDetailsAttributes("Frequency","Annual"),
                                new BatchDetailsAttributes("Product Type","Notices to Mariners"),
                                new BatchDetailsAttributes("Year","2021"),

                            },
                            BatchPublishedDate = DateTime.Now,
                            Files = new List<BatchDetailsFiles>() {
                                new BatchDetailsFiles () {
                                    Filename = "01 NP234(A) 2022.pdf",
                                    FileSize=1232,
                                    MimeType = "PDF",
                                    Links = null
                                }
                            }
                        },
                        new BatchDetails() {
                            BatchId = "5",
                              Attributes = new List<BatchDetailsAttributes>()
                            {
                                new BatchDetailsAttributes("Data Date","2022-01-18"),
                                new BatchDetailsAttributes("Frequency","Annual"),
                                new BatchDetailsAttributes("Product Type","Notices to Mariners"),
                                new BatchDetailsAttributes("Year","2022"),

                            },
                            BatchPublishedDate = DateTime.Now,
                            Files = new List<BatchDetailsFiles>() {
                                new BatchDetailsFiles () {
                                    Filename = "02 NP234(A) 2022.pdf",
                                    FileSize=1232,
                                    MimeType = "PDF",
                                    Links = null
                                }
                            }
                        },
                        new BatchDetails() {
                            BatchId = "6",
                              Attributes = new List<BatchDetailsAttributes>()
                            {
                                new BatchDetailsAttributes("Data Date","2022-01-17"),
                                new BatchDetailsAttributes("Frequency","Annual"),
                                new BatchDetailsAttributes("Product Type","Notices to Mariners"),
                                new BatchDetailsAttributes("Year","2022"),

                            },
                            BatchPublishedDate = DateTime.Now,
                            Files = new List<BatchDetailsFiles>() {
                                new BatchDetailsFiles () {
                                    Filename = "00 NP234(A) 2022.pdf",
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
