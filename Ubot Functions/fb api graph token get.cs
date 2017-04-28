using System.Collections.Generic;
using FB_API_Plugin.Webdriver.PhantomJS;
using UBotPlugin;

namespace FB_API_Plugin.Ubot_Functions
{
    internal class FbApiGraphTokenGet : IUBotFunction
    {
        // List to hold the parameters we define for our command.
        private readonly List<UBotParameterDefinition> _parameters = new List<UBotParameterDefinition>();
        private PhantomJSDriverService _driverService;
        private PhantomJSDriver _phantom;
        private string _returnValue;


        public FbApiGraphTokenGet()
        {
            _parameters.Add(new UBotParameterDefinition("fb email", UBotType.String));
            _parameters.Add(new UBotParameterDefinition("fb password", UBotType.String));
            _parameters.Add(new UBotParameterDefinition("user agent", UBotType.String));
            _parameters.Add(new UBotParameterDefinition("Proxy", UBotType.String));
        }

        public bool IsContainer => false;

        public string Category => "FB API Functions";

        public string FunctionName => "$fb api http graph token";

        public void Execute(IUBotStudio ubotStudio, Dictionary<string, string> parameters)
        {
            try
            {
                var fbemail = parameters["fb email"];
                var fbpassword = parameters["fb password"];
                //var useragent = parameters["user agent"];
                var proxy = parameters["Proxy"];


                _driverService = PhantomJSDriverService.CreateDefaultService();
                _driverService.HideCommandPromptWindow = true;


                if (proxy != string.Empty)
                {
                    var pproxy = new Webdriver.Proxy();
                    if (proxy.Contains("@"))
                    {
                        var sproxy = proxy.Split('@');
                        var cred = sproxy[0].Split(':');
                        pproxy.HttpProxy = string.Format(sproxy[1]);

                        _driverService.ProxyType = "http";
                        _driverService.Proxy = pproxy.HttpProxy;
                        _driverService.ProxyAuthentication = cred[0] + ":" + cred[1];
                    }
                    else
                    {
                        pproxy.HttpProxy = string.Format(proxy);

                        _driverService.ProxyType = "http";
                        _driverService.Proxy = pproxy.HttpProxy;
                    }
                }


                _phantom = new PhantomJSDriver(_driverService);


                _phantom.Navigate().GoToUrl("http://www.facebook.com");
                var element1 = _phantom.FindElementByXPath("//input[@name=\"email\"]");
                element1.SendKeys(fbemail);


                element1 = _phantom.FindElementByXPath("//input[@name=\"pass\"]");
                element1.SendKeys(fbpassword);

                element1 = _phantom.FindElementByXPath("//label[@id=\"loginbutton\"]/input");
                element1.Click();


                _phantom.Navigate().GoToUrl("https://developers.facebook.com/tools/explorer/");

                element1 = _phantom.FindElementByXPath("//input[contains(@placeholder,\"Access Token\")]");

                _returnValue = element1.GetAttribute("value");
            }
            finally
            {
                _phantom.Quit();
            }
        }

        public IEnumerable<UBotParameterDefinition> ParameterDefinitions => _parameters;

        public object ReturnValue => _returnValue;

        public UBotType ReturnValueType => UBotType.String;

        public UBotVersion UBotVersion => UBotVersion.Standard;
    }
}