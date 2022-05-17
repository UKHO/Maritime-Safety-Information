<Query Kind="Program">
  <Reference>&lt;RuntimeDirectory&gt;\netstandard.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Net.Http.dll</Reference>
  <NuGetReference>Microsoft.Identity.Client</NuGetReference>
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <NuGetReference>UKHO.FileShareAdminClient</NuGetReference>
  <NuGetReference>UKHO.WeekNumberUtils</NuGetReference>
  <Namespace>Microsoft.Identity.Client</Namespace>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Net.Http.Headers</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>UKHO.FileShareAdminClient</Namespace>
  <Namespace>UKHO.FileShareClient</Namespace>
  <Namespace>System.Globalization</Namespace>
  <Namespace>UKHO.WeekNumberUtils</Namespace>
  <Namespace>UKHO.FileShareAdminClient.Models</Namespace>
  <Namespace>UKHO.FileShareClient.Models</Namespace>
</Query>

const string BUSINESS_UNIT = "MaritimeSafetyInformation";
const string PRODUCT_TYPE_ATTRIBUTE_VALUE = "Notices to Mariners";

private readonly Func<DateTime, DateTime> WEEKLY_NMS_RETENTION_PERIOD = d => d.AddDays(800);
private readonly Func<DateTime, DateTime> DAILY_NMS_RETENTION_PERIOD = d => d.AddDays(15);

private readonly bool REPLACE_ALL_DATA_IN_BUSINESS_UNIT = false;

private async Task Main()
{
	var configLive = new
	{
		BaseAddress = @"https://files.admiralty.co.uk/",
		ClientId = "30cc156d-765f-43ea-8c84-a97d5b8ce852",
		TenantId = "9134ca48-663d-4a05-968a-31a42f0aed3e",
		AuthUrl = "https://login.microsoftonline.com/9134ca48-663d-4a05-968a-31a42f0aed3e/oauth2/v2.0/authorize"
	};
	var configQA = new
	{
		BaseAddress = @"https://fss-qa-webapp.azurewebsites.net",
		ClientId = "805be024-a208-40fb-ab6f-399c2647d334",
		TenantId = "9134ca48-663d-4a05-968a-31a42f0aed3e",
		AuthUrl = "https://login.microsoftonline.com/9134ca48-663d-4a05-968a-31a42f0aed3e/oauth2/v2.0/authorize"
	};
	var configDev = new
	{
		BaseAddress = @"https://fss-dev-webapp.azurewebsites.net/",
		ClientId = "3ec9403b-8299-4a90-8ecc-07ee6642109f",
		TenantId = "9134ca48-663d-4a05-968a-31a42f0aed3e",
		AuthUrl = "https://login.microsoftonline.com/9134ca48-663d-4a05-968a-31a42f0aed3e/oauth2/v2.0/authorize"
	};

	var config = configQA;

	var fssClient = new FileShareApiAdminClient(new UserAgentClientFactory(), config.BaseAddress,
		new AuthTokenProvider(config.ClientId, config.TenantId, config.AuthUrl));

	if (REPLACE_ALL_DATA_IN_BUSINESS_UNIT)
	{
		await ExpireAllDataInBusinessUnit(fssClient, BUSINESS_UNIT);
	}

	await ProcessDailies(fssClient, @"\\hadsprddom04.business.ukho.gov.uk\DFS\Prod_Prod\nms\FILES\NMOutput\Dailies");
	await ProcessWeeklies(fssClient, @"\\hadsprddom04.business.ukho.gov.uk\DFS\Bus_CorpData\_DATA EXCHANGE\NM Weekly");
}

private async Task ExpireAllDataInBusinessUnit(FileShareApiAdminClient fssClient, string businessUnit)
{
	var query = $"businessUnit eq '{businessUnit}' and $batch(Product Type) eq '{PRODUCT_TYPE_ATTRIBUTE_VALUE}'";
	var existingData = await fssClient.Search(query);

	while (existingData.Count > 0)
	{
		foreach (var existingBatch in existingData.Entries)
		{
			await fssClient.SetExpiryDateAsync(existingBatch.BatchId, new BatchExpiryModel() { ExpiryDate = DateTime.Now });
		}

		existingData = await fssClient.Search(query);
	}
}

// Define other methods and classes here

/// <summary>Weeklies folder, probably in the Data Exchange.  Expected to contain sub folders: yyyy/WK xx-yy/Public/  and yyyy/WK xx-yy/Agents</summary>
private async Task ProcessWeeklies(IFileShareApiAdminClient fssClient, string weekliesFolder)
{
	var weekliesDirectories = new DirectoryInfo(weekliesFolder);

	foreach (var yearDirectory in weekliesDirectories.EnumerateDirectories())
	{
		foreach (var weekDirectory in yearDirectory.EnumerateDirectories())
		{
			var year = int.Parse(yearDirectory.Name);
			var weekNameMatch = Regex.Match(weekDirectory.Name, @"^WK (\d{1,2})-(\d\d)$", RegexOptions.IgnoreCase);

			if (!weekNameMatch.Success)
			{
				throw new ArgumentException($"Odd week name: {weekDirectory.Name}");
			}

			var weekAsInt = int.Parse(weekNameMatch.Groups[1].Value);
			var weekNumber = new WeekNumber(year, weekAsInt);

			var attributes = new Dictionary<string, string>()
			{
				{"Product Type", PRODUCT_TYPE_ATTRIBUTE_VALUE},
				{"Year",         $"{weekNumber.Year}"},
				{"Week Number",  $"{weekNumber.Week}"},
				{"Year / Week",  $"{weekNumber.Year} / {weekNumber.Week}"},
				{"Frequency",    "Weekly"}
			};

			await AddPdfsFromPublicFolderToFss(WEEKLY_NMS_RETENTION_PERIOD(weekNumber.Date), weekDirectory, fssClient, attributes);
		}
	}
}

/// <summary>Dailies folder, Expected to contain sub folders: yyyy/Weeks x to y/Week ww/Published/Public/  and yyyy/Weeks x to y/Week ww/Published/Agents</summary>
private async Task ProcessDailies(IFileShareApiAdminClient fssClient, string dailiesFolder)
{
	var dailiesFolderDirectory = new DirectoryInfo(dailiesFolder);

	foreach (var yearDirectory in dailiesFolderDirectory.EnumerateDirectories())
	{
		foreach (var weekDirectory in yearDirectory.EnumerateDirectories().SelectMany(d => d.EnumerateDirectories()).OrderBy(d => d.CreationTime))
		{
			var year = int.Parse(yearDirectory.Name);
			var weekNameMatch = Regex.Match(weekDirectory.Name, @"^Week (\d\d)$", RegexOptions.IgnoreCase);

			if (!weekNameMatch.Success)
			{
				$"Odd week name: {weekDirectory.FullName}".Dump();
				continue;
			}

			var weekAsInt = int.Parse(weekNameMatch.Groups[1].Value);
			var weekNumber = new WeekNumber(year, weekAsInt);

			if (DAILY_NMS_RETENTION_PERIOD(weekNumber.Date).AddDays(7) < DateTime.UtcNow)
			{
				$"The whole week of data is expired, no point uploading from {weekDirectory.FullName}".Dump();
				continue;
			}

			var publishedDataDirectory = weekDirectory.EnumerateDirectories().SingleOrDefault(d => d.Name == "Published");

			if (publishedDataDirectory == null)
			{
				weekDirectory.FullName.Dump("Could not find Published directory in");
			}
			else
			{
				publishedDataDirectory.FullName.Dump("Loading daily Published data from");

				foreach (var dayDataDirectory in publishedDataDirectory.EnumerateDirectories().OrderBy(d => d.CreationTime))
				{
					var dayDataFolderNameMatch = Regex.Match(dayDataDirectory.Name, @"^(\d{4})(\d{2})(\d{2})$", RegexOptions.IgnoreCase);

					if (!dayDataFolderNameMatch.Success)
					{
						$"Odd Day's Data Folder name: {dayDataDirectory.Name}".Dump();
						continue;
					}

					var dataDateAttribute = $"{dayDataFolderNameMatch.Groups[1].Value}-{dayDataFolderNameMatch.Groups[2].Value}-{dayDataFolderNameMatch.Groups[3].Value}";
					var dayDate = DateTime.ParseExact(dataDateAttribute, "yyyy-MM-dd", CultureInfo.InvariantCulture);
					var attributes = new Dictionary<string, string>()
					{
						{"Product Type", PRODUCT_TYPE_ATTRIBUTE_VALUE},
						{"Year",         $"{weekNumber.Year}",
						{"Week Number",  $"{weekNumber.Week}"},
						{"Year / Week",  $"{weekNumber.Year} / {weekNumber.Week}"},
						{"Frequency",    "Daily"},
						{"Data Date",    dataDateAttribute}
					};

					await AddPDFsFromPublicFolderToFSS(DAILY_NMS_RETENTION_PERIOD(dayDate), dayDataDirectory, fssClient, attributes);
				}
			}
		}
	}
}

private async Task AddPDFsFromPublicFolderToFSS(DateTime expiryDate, DirectoryInfo dayDataDirectory, IFileShareApiAdminClient fssClient, Dictionary<string, string> attributes)
{
	if (expiryDate < DateTime.UtcNow)
	{
		$"Data from {dayDataDirectory.FullName} as already expired: {expiryDate}. No point uploading".Dump();
		return;
	}

	var publicDataDirectory = dayDataDirectory.EnumerateDirectories().SingleOrDefault(d => d.Name == "Public");

	if (publicDataDirectory == null)
	{
		dayDataDirectory.FullName.Dump("Could not find Public directory in");
	}
	else
	{
		var publicNMsFiles = publicDataDirectory.EnumerateFileSystemInfos("*.pdf", SearchOption.AllDirectories);
		
		if (publicNMsFiles.Any())
		{
			var batchModel = new BatchModel()
			{
				Acl = new Acl
				{
					ReadGroups = new List<string>() { "public" }
				},
				BusinessUnit = BUSINESS_UNIT,
				ExpiryDate = expiryDate,
				Attributes = attributes.ToList()
			};

			var existingBatches = await searchForExistingBatches(fssClient, batchModel);

			if (existingBatches.Any())
			{
				$"Existing batches already exist for {publicDataDirectory.FullName}".Dump();
			}
			else
			{
				var batch = await fssClient.CreateBatchAsync(batchModel);

				foreach (var publicNMsFile in publicNMsFiles)
				{
					await fssClient.AddFileToBatch(batch, File.OpenRead(publicNMsFile.FullName), publicNMsFile.Name, "application/pdf");
				}

				await fssClient.CommitBatch(batch);
				//await fssClient.RollBackBatchAsync(batch);// undo the changes for now!
			}
		}
		else
		{
			$"No PDFs found in {publicDataDirectory}".Dump();
		}
	}
}

private async Task<IEnumerable<BatchDetails>> searchForExistingBatches(IFileShareApiAdminClient fssClient, BatchModel batchModel)
{
	var query = $"businessUnit eq '{BUSINESS_UNIT}' and ({string.Join(" and ", batchModel.Attributes.Select(a => $"($batch({a.Key}) eq '{a.Value}')"))})";
	var result = await fssClient.Search(query);

	return result.Entries;
}

public class UserAgentClientFactory : IHttpClientFactory
{
	public HttpClient CreateClient(string name)
	{
		var client = new HttpClient();

		client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("RockyLinqPadScriptsForMSIUpload", "1.0.0.0"));
		client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(nameof(FileShareApiAdminClient),
				typeof(FileShareApiAdminClient)
					.Assembly
					.GetCustomAttributes<AssemblyFileVersionAttribute>()
					.Single()
					.Version));

		return client;
	}
}

public class AuthTokenProvider : IAuthTokenProvider
{
	private readonly string clientId;
	private readonly string tenantId;
	private readonly string authUrl;

	private AuthenticationResult? lastAuthResult = null;

	public AuthTokenProvider(string clientId, string tenantId, string authUrl)
	{
		this.clientId = clientId;
		this.tenantId = tenantId;
		this.authUrl = authUrl;
	}

	public async Task<string> GenerateToken(string clientId, string tenantId, string microsoftOnlineLoginUrl)
	{
		if (lastAuthResult == null || lastAuthResult.ExpiresOn.UtcDateTime.AddMinutes(-5) < DateTimeOffset.UtcNow.UtcDateTime)
		{
			string authority = $"{microsoftOnlineLoginUrl}{tenantId}";
			string[] scopes = new string[] { $"{clientId}/.default" };

			IPublicClientApplication debugapp = PublicClientApplicationBuilder
												  .Create(clientId)
												  .WithAuthority(authority)
												  .WithRedirectUri("http://localhost")
												  .Build();

			var token = (await debugapp.AcquireTokenInteractive(scopes).ExecuteAsync());
			lastAuthResult = token;
		}
		return lastAuthResult.AccessToken;
	}

	public Task<string> GetToken()
	{
		return GenerateToken(clientId, tenantId, authUrl);
	}
}