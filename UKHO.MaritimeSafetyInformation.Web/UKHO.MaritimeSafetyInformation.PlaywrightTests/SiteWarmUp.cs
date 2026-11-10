namespace UKHO.MaritimeSafetyInformation.PlaywrightTests
{
    internal static class SiteWarmUp
    {
        private const int MaxAttempts = 3;
        private static readonly TimeSpan RequestTimeout = TimeSpan.FromSeconds(60);
        private static readonly TimeSpan RetryDelay = TimeSpan.FromSeconds(10);

        public static async Task WarmUpAsync(params string[] urls)
        {
            using var handler = new HttpClientHandler { AllowAutoRedirect = false };
            using var client = new HttpClient(handler) { Timeout = RequestTimeout };

            foreach (var url in urls.Where(u => !string.IsNullOrWhiteSpace(u)))
            {
                await WarmUpUrlAsync(client, url);
            }
        }

        private static async Task WarmUpUrlAsync(HttpClient client, string url)
        {
            for (var attempt = 1; attempt <= MaxAttempts; attempt++)
            {
                try
                {
                    using var response = await client.GetAsync(url);
                    var status = (int)response.StatusCode;

                    if (status is >= 200 and < 400)
                    {
                        TestContext.Progress.WriteLine($"Warm-up of {url} succeeded on attempt {attempt} with status {status}.");
                        return;
                    }

                    if (status is >= 400 and < 500)
                    {
                        Assert.Fail($"Warm-up request to {url} returned {status}. Check the 'url' configuration.");
                    }

                    TestContext.Progress.WriteLine($"Warm-up of {url} attempt {attempt} returned status {status}.");
                }
                catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
                {
                    TestContext.Progress.WriteLine($"Warm-up of {url} attempt {attempt} failed: {ex.Message}");
                }

                if (attempt < MaxAttempts)
                {
                    await Task.Delay(RetryDelay);
                }
            }

            Assert.Fail($"Site {url} did not respond successfully after {MaxAttempts} warm-up attempts.");
        }
    }
}
