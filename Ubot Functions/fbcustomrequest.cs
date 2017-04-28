using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using FB_API_Plugin.Ubot_Commands;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UBotPlugin;

namespace FB_API_Plugin.Ubot_Functions
{
    internal class FBcustomRequest : IUBotFunction
    {
        private object _lockit = new object();
        // List to hold the parameters we define for our command.
        private readonly List<UBotParameterDefinition> _parameters = new List<UBotParameterDefinition>();
        private List<string> _returnValue = new List<string>();

        public FBcustomRequest()
        {
            _parameters.Add(new UBotParameterDefinition("ObjectID", UBotType.String));
            _parameters.Add(new UBotParameterDefinition("Method", UBotType.String));
            _parameters.Add(new UBotParameterDefinition("Parameters", UBotType.String));
            _parameters.Add(new UBotParameterDefinition("Parameter To Scrape", UBotType.String));
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
            get { return "$fb api custom request"; }
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


                var ScrapedData = new List<string>();
                var dict = new Dictionary<string, string>();

                var ArgsArray = parameters["Parameters"].Split('&');
                foreach (var arg in ArgsArray)
                {
                    var ArgsKeys = arg.Split('=');
                    dict.Add(ArgsKeys[0], ArgsKeys[1]);
                }

                var ObjectFb = facebook.GetConnections(parameters["ObjectID"], parameters["Method"], dict);

                var Jpath = JsonPathParser(facebook.JsonText, "$..next");

                foreach (var group in ObjectFb["data"])
                {
                    ScrapedData.Add(group[parameters["Parameter To Scrape"]].ToString());
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
                        ScrapedData.Add(group[parameters["Parameter To Scrape"]].ToString());
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
            var items = Arr.Select(jv => jv.ToString()).ToArray();

            return items;
        }
    }
}