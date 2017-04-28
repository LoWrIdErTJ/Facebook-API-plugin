using System;
using System.Collections.Generic;
using FB_API_Plugin.Ubot_Commands;
using UBotPlugin;

namespace FB_API_Plugin.Ubot_Functions
{
    internal class GetObject : IUBotFunction
    {
        private object _lockit = new object();
        // List to hold the parameters we define for our command.
        private readonly List<UBotParameterDefinition> _parameters = new List<UBotParameterDefinition>();
        private string _returnValue = "";

        public GetObject()
        {
            _parameters.Add(new UBotParameterDefinition("Object ID", UBotType.String));
            _parameters.Add(new UBotParameterDefinition("args", UBotType.String));
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
            get { return "$fb api get object"; }
        }

        public void Execute(IUBotStudio ubotStudio, Dictionary<string, string> parameters)
        {
            var inicontainer = (FBcontainer) ubotStudio.ContainerParent;
            var token = inicontainer.Token;
            var dict = new Dictionary<string, string>();
            try
            {
                var facebook = new FacebookGraphApi(token.Trim());
                facebook.ProxyStr = inicontainer.Proxy;
                facebook.FBversion = inicontainer.Version;

                if (parameters["args"] == string.Empty)
                {
                    dict = null;
                }
                else
                {
                    var argsItems = parameters["args"].Split('&');
                    foreach (var item in argsItems)
                    {
                        var args = item.Split('=');
                        dict.Add(args[0], args[1]);
                    }
                }

                var ObjectFb = facebook.GetObject(parameters["Object ID"], dict);
                _returnValue = facebook.JsonText;
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
            get { return UBotType.String; }
        }

        public UBotVersion UBotVersion
        {
            // This is the lowest version of UBot Studio that this function can run in.
            get { return UBotVersion.Standard; }
        }
    }
}