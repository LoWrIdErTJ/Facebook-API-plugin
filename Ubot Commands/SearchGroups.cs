using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UBotPlugin;

namespace FB_API_Plugin.Ubot_Commands
{
    internal class SearchGroups : IUBotCommand
    {
        private readonly List<UBotParameterDefinition> _parameters = new List<UBotParameterDefinition>();

        public SearchGroups()
        {
            _parameters.Add(new UBotParameterDefinition("keyword", UBotType.String));
            _parameters.Add(new UBotParameterDefinition("Data", UBotType.UBotTable));
        }

        public string Category
        {
            get { return "FB API Commands"; }
        }

        public string CommandName
        {
            get { return "fb api search groups"; }
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
                dict.Add("q", parameters["keyword"]);
                dict.Add("type", "group");
                dict.Add("limit", "100");
                var ObjectFb = facebook.GetConnections("", "search", dict);
                //_returnValue = facebook.JsonText;
                var Jpath = JsonPathParser(facebook.JsonText, "$..next");

                foreach (var group in ObjectFb["data"])
                {
                    ScrapedData.Add(group["name"] + "|" + group["id"]);
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
                        ScrapedData.Add(group["name"] + "|" + group["id"]);
                    }

                    var JpathW = JsonPathParser(reply, "$..next");
                    fbNext = "";
                    if (JpathW.Length != 0)
                    {
                        fbNext = JpathW[0];
                    }
                }


                var data = new string[ScrapedData.Count, 2];

                for (var i = 0; i < ScrapedData.Count; i++)
                {
                    var LocD = ScrapedData[i].Split('|');
                    data[i, 0] = LocD[0];
                    data[i, 1] = LocD[1];
                }

                ubotStudio.SetTable(parameters["Data"], data);
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

        public bool IsContainer
        {
            get { return false; }
        }

        public IEnumerable<UBotParameterDefinition> ParameterDefinitions
        {
            get { return _parameters; }
        }

        public UBotVersion UBotVersion
        {
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