﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using FB_API_Plugin.Ubot_Commands;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UBotPlugin;

namespace FB_API_Plugin.Ubot_Functions
{
    internal class GetPagesDetails : IUBotFunction
    {
        private object _lockit = new object();
        // List to hold the parameters we define for our command.
        private readonly List<UBotParameterDefinition> _parameters = new List<UBotParameterDefinition>();
        private List<string> _returnValue = new List<string>();

        public GetPagesDetails()
        {
            _parameters.Add(new UBotParameterDefinition("parent page", UBotType.String));
            var SSLParameter = new UBotParameterDefinition("List", UBotType.String);
            SSLParameter.Options = new[]
            {
                "admins", "albums", "blocked", "checkins", "conversations", "events", "feed", "global_brand_children",
                "insights", "links", "locations", "milestones", "notes", "notes", "picture", "photos", "photos/uploaded",
                "posts", "promotable_posts", "questions", "ratings", "settings", "statuses", "tabs", "tagged", "videos"
            };
            _parameters.Add(SSLParameter);
        }

        public bool IsContainer
        {
            // This command does not have any nested commands inside of it, so it is not a container command.
            get { return false; }
        }

        public string Category
        {
            // This is what category our command is categorized as. 
            // If you choose something not already in the toolbox list, a new category will be created.
            get { return "FB API Functions"; }
        }

        public string FunctionName
        {
            // The name of our node in UBot Studio.
            // You need to use a $ sign in front of function names.
            get { return "$fb api page details search"; }
        }

        public void Execute(IUBotStudio ubotStudio, Dictionary<string, string> parameters)
        {
            var inicontainer = (FBcontainer) ubotStudio.ContainerParent;
            var token = inicontainer.Token;

            try
            {
                var facebook = new FacebookGraphApi(token.Trim());
                facebook.ProxyStr = inicontainer.Proxy;
                facebook.FBversion = inicontainer.Version;
                //JArray data = new JArray();
                var ScrapedData = new List<string>();
                var dict = new Dictionary<string, string>();
                dict.Add("fields", "id");
                dict.Add("limit", "100");
                var ObjectFb = facebook.GetConnections(parameters["parent page"], parameters["List"], dict);

                var Jpath = JsonPathParser(facebook.JsonText, "$..next");

                foreach (var group in ObjectFb["data"])
                {
                    ScrapedData.Add(group["id"].ToString());
                }

                var fbNext = "";
                if (Jpath.Length != 0)
                {
                    fbNext = Jpath[0];
                }

                while (fbNext != string.Empty)
                {
                    var wc = new WebClient();
                    var reply = wc.DownloadString(fbNext.Replace(@"\", ""));
                    var jo = JObject.Parse(reply);
                    foreach (var group in jo["data"])
                    {
                        ScrapedData.Add(group["id"].ToString());
                    }

                    var JpathW = JsonPathParser(reply, "$..next");
                    fbNext = "";
                    if (JpathW.Length != 0)
                    {
                        fbNext = JpathW[0];
                    }
                }

                _returnValue = ScrapedData;
            }
            catch (Exception exp)
            {
                if (exp.GetType() == typeof (FacebookGraphApiException))
                {
                    inicontainer.Error = (exp as FacebookGraphApiException).Message;
                }
                else
                {
                    inicontainer.Error = exp.Message;
                }
            }
        }

        public IEnumerable<UBotParameterDefinition> ParameterDefinitions
        {
            // We reference our parameter list we defined above here.
            get { return _parameters; }
        }

        public object ReturnValue
        {
            // We return our variable _returnValue as the result of the function.
            get { return _returnValue; }
        }

        public UBotType ReturnValueType
        {
            // Our result is a list, so the return value type is UBotList.
            get { return UBotType.UBotList; }
        }

        public UBotVersion UBotVersion
        {
            // This is the lowest version of UBot Studio that this function can run in.
            get { return UBotVersion.Standard; }
        }

        private string[] JsonPathParser(string jsonInput, string ExprJson)
        {
            var json = JObject.Parse(jsonInput);
            var context = new JsonPathContext {ValueSystem = new JsonNetValueSystem()};
            var values = context.SelectNodes(json, ExprJson).Select(node => node.Value);
            var JsonOut = JsonConvert.SerializeObject(values);
            var Arr = JArray.Parse(JsonOut);
            var items = Arr.Select(jv => (string) jv).ToArray();

            return items;
        }
    }
}