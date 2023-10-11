using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace ErSoftDev.Common.Utilities
{
    public static class ApiHelper
    {
        public static async Task<HttpResponseMessage> SendHttpAsync<TRequest>(IHttpClientFactory client,
            string serviceUrl,
            TRequest request, HttpMethod httpMethod, CancellationToken cancellationToken
            , Dictionary<string, string>? headers = null, string contentType = "application/json")
        {

            var httpRequestMessage = new HttpRequestMessage(httpMethod, serviceUrl);
            if (request is not null)
                httpRequestMessage.Content = new HttpContentConvert(request);
            if (httpRequestMessage.Content is not null)
                httpRequestMessage.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);
            if (headers != null)
                foreach (var header in headers)
                    httpRequestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value);

            return await client.CreateClient().SendAsync(httpRequestMessage, cancellationToken);
        }

        public static async Task<StreamAndDeserializeResponse<T>> StreamAndDeserialize<T>(HttpResponseMessage response,
            CancellationToken cancellationToken)
        {
            var responseStream = await response.Content.ReadAsStringAsync(cancellationToken);
            var deserializeResponse =
                JsonConvert.DeserializeObject<T>(responseStream);

            return new StreamAndDeserializeResponse<T>(responseStream, deserializeResponse!);
        }
    }
    public class StreamAndDeserializeResponse<T>
    {
        public StreamAndDeserializeResponse(string stream, T deserialize)
        {
            Stream = stream;
            Deserialize = deserialize;
        }

        public string Stream { get; set; }
        public T Deserialize { get; set; }
    }
}
