using System;
using System.Net;
using System.Net.Http;
using RestSharp;

namespace Global
{
    public class Globals
    {
        public static string proxyURL = "http://192.168.0.100:3128";
        public string proxyUsername = "2is4";
        public string proxyPassword = "edu-351";
        public static string localProxyURL = "http://192.168.0.17:3128";
        public string globalUrl = "http://api.govorovma.ru/api-cafe/login";
        public string localUrl = "http://cncaevh-m2.wsr.ru/api-cafe/login";
        public string getUsersUrl = "http://cncaevh-m2.wsr.ru/api-cafe/user";
        public static string userToken { get; set; }

        public static WebProxy webProxy = new WebProxy
        {
            Address = new Uri(localProxyURL)
        };
        public static HttpClientHandler httpClientHandler = new HttpClientHandler
        {
            Proxy = webProxy
        };
        public RestClient client = new RestClient(httpClientHandler);
    }
}