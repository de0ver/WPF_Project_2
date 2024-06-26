﻿using System;
using System.Net;
using System.Net.Http;
using RestSharp;
using System.Collections.Generic;

namespace Global
{
    public class Globals
    {
        public static string proxyURL = "http://192.168.0.100:3128";                //internet (college)
        public string proxyUsername = "2is4";                                       //internet (college)
        public string proxyPassword = "edu-351";                                    //internet (college)
        public string globalURL = "http://api.govorovma.ru/api-cafe/login";         //internet (college)
        public static string localProxyURL = "http://192.168.0.17:3128";            //lan
        public string localURL = "http://cncaevh-m2.wsr.ru/api-cafe/login";         //lan
        public string localURL2 = "http://jbgfrra-m2.wsr.ru/api-cafe/login";        //lan
        public string localURL3 = "http://backend/api-cafe/login";        //lan
        public string UserURL = "http://backend/api-cafe/user";           //lan
        public string WorkShiftURL = "http://backend/api-cafe/work-shift";//lan
        public string OrderURL = "http://backend/api-cafe/order";         //lan
        public static string userToken { get; set; }

        public static WebProxy webProxy = new WebProxy
        {
            //Address = new Uri(localProxyURL)
        };

        public static HttpClientHandler httpClientHandler = new HttpClientHandler
        {
            //Proxy = webProxy
        };

        public RestClient client = new RestClient(httpClientHandler);

        public class JSONUser
        {
            public List<User> user { get; set; }

            public class User
            {
                public int id { get; set; }
                public string name { get; set; }
                public string login { get; set; }
                public string status { get; set; }
                public string group { get; set; }
            }
        }

        public class HTTPMessageError
        {
            public Error error { get; set; }
            public class Error
            {
                public int code { get; set; }
                public string message { get; set; }
                public Errors errors { get; set; }

                public class Errors
                {
                    public List<string> name { get; set; }
                    public List<string> surname { get; set; }
                    public List<string> patronymic { get; set; }
                    public List<string> login { get; set; }
                    public List<string> password { get; set; }
                    public List<string> role_id { get; set; }
                }
            }
        }

        public class HTTPMessageCreated //created, updated, deleted, fired
        {
            public MessageData data { get; set; }

            public class MessageData
            {
                public int id { get; set; }
                public string status { get; set; }
            }
        }

        public class HTTPMessageOK
        {
            public Data data { get; set; }
            public class Data
            {
                public string user_token { get; set; }
                public int role_id { get; set; }
                public string name { get; set; }
                public string surname { get; set; }
                public string patronymic { get; set; }
                public string login { get; set; }
                public int user_id { get; set; }
            }
        }

        public class HTTPMessageShifts
        {
            public class MessageCreateWorkShift
            {
                public string start { get; set; }
                public string end { get; set; }
                public string updated_at { get; set; }
                public string created_at { get; set; }
                public int id { get; set; }
            }

            public class MessageShifts
            {
                public int id { get; set; }
                public string start { get; set; }
                public string end { get; set; }
                public int active { get; set; }
                public string created_at { get; set; }
                public string updated_at { get; set; }
            }

            public class MessageWorkers
            {
                public int id { get; set; }
                public string name { get; set; }
                public string login { get; set; }
                public string status { get; set; }
                public Pivot pivot { get; set; }
                public class Pivot
                {
                    public int work_shift_id { get; set; }
                    public int user_id { get; set; }
                }
            }

            public class MessageOrders
            {
                public int id { get; set; }
                public string table { get; set; }
                public string shift_workers { get; set; }
                public string created_at { get; set; }
                public string status { get; set; }
                public int price { get; set; }
            }
        }
    }
}