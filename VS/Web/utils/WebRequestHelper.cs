using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
public class WebRequestHelper
{
    public object GetPostResponse(string url)
    {
        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = client
                .PostAsync(url, new StringContent(""))
                .Result;
            response.EnsureSuccessStatusCode();
            object result = response.Content.ReadAsAsync<object>().Result;
            return result;
        }
    }
}
