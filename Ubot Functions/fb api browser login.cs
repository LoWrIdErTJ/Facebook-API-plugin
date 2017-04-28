using System;
using System.Collections.Generic;
using UBotPlugin;

namespace FB_API_Plugin.Ubot_Functions
{
    internal class fb_api_browser_login : IUBotFunction
    {
        private object _lockit = new object();
        // List to hold the parameters we define for our command.
        private readonly List<UBotParameterDefinition> _parameters = new List<UBotParameterDefinition>();
        private string _returnValue;

        public fb_api_browser_login()
        {
            _parameters.Add(new UBotParameterDefinition("Client ID", UBotType.String));
            _parameters.Add(new UBotParameterDefinition("Redirect Url", UBotType.String));
            _parameters.Add(new UBotParameterDefinition("scope", UBotType.String));
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
            get { return "$fb api login"; }
        }

        public void Execute(IUBotStudio ubotStudio, Dictionary<string, string> parameters)
        {
            var Client_Id = parameters["Client ID"];
            var redirectUrl = parameters["Redirect Url"];
            var scope = parameters["scope"];
            var UrlPopUp = "https://www.facebook.com/dialog/oauth?response_type=token&display=popup&client_id=" +
                           Client_Id + "&redirect_uri=" + Uri.EscapeDataString(redirectUrl) + "&scope=" +
                           Uri.EscapeDataString(scope);
            
            try
            {
                var bs = new Browser();
                bs.Url = UrlPopUp;
                bs.RedirectUrl = redirectUrl;
                bs.ShowDialog();
                _returnValue = bs.GetToken;
            }

            catch
            {
                _returnValue = "";
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