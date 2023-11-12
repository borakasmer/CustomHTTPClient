using Microsoft.Extensions.Options;
using System.ComponentModel;
using System.Net;
using System.Text;
using System.Threading;

namespace CustomHTTPClient
{
    public class CustomWebClient : WebClient
    {
        public CookieContainer Cookies { get; }
        public int Timeout { get; set; }
        public int Tries { get; set; }

        //Default Constructor
        public CustomWebClient() : this(100)
        {
        }
        public readonly IOptionsSnapshot<CustomWebClientConfig> _config;
        //1-) Get Property From Config Constructor
        public CustomWebClient(IOptionsSnapshot<CustomWebClientConfig> config) : this(100)
        {
            _config = config;
            Timeout = config.Value.TimeOut;
            Tries = config.Value.Tries;
            Cookies = new CookieContainer();
            this.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
            this.Headers.Add(HttpRequestHeader.ContentType, "application/json");
        }

        //2-) Get Property From Parameter Constructor
        public CustomWebClient(int timeOut)
        {
            Timeout = timeOut;
            Cookies = new CookieContainer();
            Encoding = Encoding.UTF8;
        }

        //Without CookieContainer
        //protected override WebRequest GetWebRequest(Uri uri)
        //{
        //    WebRequest w = base.GetWebRequest(uri);
        //    w.Timeout = Timeout * 1000;
        //    return w;
        //}

        //With CookieContainer
        protected override WebRequest GetWebRequest(Uri uri)
        {
            WebRequest w = base.GetWebRequest(uri);
            w.Timeout = Timeout * 1000;

            var request = w as HttpWebRequest;
            if (request != null)
            {
                request.CookieContainer = Cookies;
            }

            return request;
        }

        //Without CookieContainer
        //protected override WebResponse GetWebResponse(WebRequest request)
        //{
        //    WebRequest deepCopiedWebRequest = ObjectCopier.Clone<WebRequest>(request);
        //    try
        //    {
        //        return base.GetWebResponse(deepCopiedWebRequest);
        //    }
        //    catch (WebException ex)
        //    {
        //        if (ex.Status == WebExceptionStatus.Timeout || ex.Status == WebExceptionStatus.ConnectFailure)
        //            if (--Tries == 0)
        //                throw;

        //        return GetWebResponse(request);
        //    }
        //}

        //With CookieContainer
        protected override WebResponse GetWebResponse(WebRequest request)
        {
            try
            {
                WebResponse response = base.GetWebResponse(request);
                ReadCookies(response);
                return response;
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.Timeout || ex.Status == WebExceptionStatus.ConnectFailure || ex.Status==WebExceptionStatus.ProtocolError)
                {
                    if (--Tries == 0)
                        throw;

                    if (Tries == 1)
                    {
                        request.Timeout = 5000;
                    }

                    //New Patch For NonExist Urls
                    /*var htmlStr = GetResponseStr((HttpWebRequest)request).Result;
                    if (!string.IsNullOrEmpty(htmlStr))
                    {
                        return GetWebResponse(request);
                    }
                    throw;*/
                    //---------------------------------------------
                    
                    if (ex.Status == WebExceptionStatus.ProtocolError) throw;                    
                    return GetWebResponse(request);
                }
                else
                {
                    throw;
                }
            }
        }

        private void ReadCookies(WebResponse r)
        {
            var response = r as HttpWebResponse;
            if (response != null)
            {
                CookieCollection cookies = response.Cookies;
                Cookies.Add(cookies);
            }
        }

        //IS URL EXIST
        private async Task<string> GetResponseStr(HttpWebRequest request)
        {
            var final_response = string.Empty;
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                StreamReader stream = new StreamReader(response.GetResponseStream());
                final_response = stream.ReadToEnd();
            }
            catch (Exception ex)
            {
                //DO whatever necessary like log or sending email to notify you   
            }

            return final_response;
        }

    }
}
