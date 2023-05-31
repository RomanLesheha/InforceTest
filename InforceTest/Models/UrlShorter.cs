using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace InforceTest.Models
{
    public static class UrlShorter
    {
        public static async Task<string> MakeTinyUrlByTinyUrl(string url)
        {
            try
            {
                if (url.Length <= 30)
                {
                    return url;
                }

                if (!url.ToLower().StartsWith("http") && !url.ToLower().StartsWith("ftp"))
                {
                    url = "http://" + url;
                }

                var request = WebRequest.Create("http://tinyurl.com/api-create.php?url=" + url);
                var res = await request.GetResponseAsync();

                string text;
                using (var reader = new StreamReader(res.GetResponseStream()))
                {
                    text = await reader.ReadToEndAsync();
                }

                return text;
            }
            catch (Exception)
            {
                return url;
            }
        }
    }
}
