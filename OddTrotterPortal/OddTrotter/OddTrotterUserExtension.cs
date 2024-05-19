namespace OddTrotter
{
    using System;
    using System.Net.Http;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Threading.Tasks;

    using global::OddTrotter.GraphClient;

    public sealed class OddTrotterUserExtension
    {
        [JsonPropertyName("id")]
        public string Id { get; } = "microsoft.oddTrotter";

        [JsonRequired]
        [JsonPropertyName("data")]
        public string? Data { get; set; }
    }

    //// TODO productize this class
    public static class Utilities
    {
        public static async Task UpdateUserExtension(IGraphClient graphClient, OddTrotterUserExtension extensionData)
        {
            var serializedExtension = JsonSerializer.Serialize(extensionData);
            using (var stringContent = new StringContent(serializedExtension))
            {
                var patchUrl = new Uri($"/me/extensions/{extensionData.Id}", UriKind.Relative).ToRelativeUri();
                var postUrl = new Uri("/me/extensions", UriKind.Relative).ToRelativeUri();

                using (var httpResponse = await PatchUserExtension(graphClient, patchUrl, postUrl, stringContent).ConfigureAwait(false))
                {
                    httpResponse.EnsureSuccessStatusCode();
                }
            }
        }

        private static async Task<HttpResponseMessage> PatchUserExtension(IGraphClient graphClient, RelativeUri patchUrl, RelativeUri postUrl, HttpContent httpContent)
        {
            HttpResponseMessage? httpResponse = null;
            try
            {
                httpResponse = await graphClient.PatchAsync(patchUrl, httpContent).ConfigureAwait(false);
                if (httpResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    httpResponse.Dispose();
                    return await PostUserExtension(graphClient, patchUrl, postUrl, httpContent).ConfigureAwait(false);
                }

                return httpResponse;

            }
            catch
            {
                httpResponse?.Dispose();
                throw;
            }
        }

        private static async Task<HttpResponseMessage> PostUserExtension(IGraphClient graphClient, RelativeUri patchUrl, RelativeUri postUrl, HttpContent httpContent)
        {
            HttpResponseMessage? httpResponse = null;
            try
            {
                httpResponse = await graphClient.PostAsync(postUrl, httpContent).ConfigureAwait(false);
                if (httpResponse.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    httpResponse.Dispose();
                    return await PatchUserExtension(graphClient, patchUrl, postUrl, httpContent).ConfigureAwait(false);
                }

                return httpResponse;

            }
            catch
            {
                httpResponse?.Dispose();
                throw;
            }
        }
    }
}
