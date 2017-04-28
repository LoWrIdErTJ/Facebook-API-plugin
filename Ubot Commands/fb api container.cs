using System.Collections.Generic;
using UBotPlugin;

namespace FB_API_Plugin.Ubot_Commands
{
    internal class FBcontainer : IUBotCommand
    {
        private readonly List<UBotParameterDefinition> _parameters = new List<UBotParameterDefinition>();

        public FBcontainer()
        {
            _parameters.Add(new UBotParameterDefinition("Token", UBotType.String));
        }

        public string Token { get; private set; }
        public string Error { get; set; }
        public string Version { get; set; }
        public Dictionary<string, string> Attachement { get; set; }

        public string Proxy { get; set; }

        public string Category
        {
            get { return "FB API Commands"; }
        }

        public string CommandName
        {
            get { return "fb api container"; }
        }

        public void Execute(IUBotStudio ubotStudio, Dictionary<string, string> parameters)
        {
            Attachement = new Dictionary<string, string>();
            Error = "";
            Proxy = "";
            Version = "none";
            Token = parameters["Token"];
            ubotStudio.RunContainerCommands();
        }

        public bool IsContainer
        {
            get { return true; }
        }

        public IEnumerable<UBotParameterDefinition> ParameterDefinitions
        {
            get { return _parameters; }
        }

        public UBotVersion UBotVersion
        {
            get { return UBotVersion.Standard; }
        }
    }
}