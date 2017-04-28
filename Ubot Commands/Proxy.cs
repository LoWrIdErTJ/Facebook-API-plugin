using System.Collections.Generic;
using UBotPlugin;

namespace FB_API_Plugin.Ubot_Commands
{
    internal class Proxy : IUBotCommand
    {
        private readonly List<UBotParameterDefinition> _parameters = new List<UBotParameterDefinition>();

        public Proxy()
        {
            _parameters.Add(new UBotParameterDefinition("Proxy", UBotType.String));
        }

        public string Token { get; private set; }
        public string Error { get; set; }

        public string Category
        {
            get { return "FB API Commands"; }
        }

        public string CommandName
        {
            get { return "fb api proxy"; }
        }

        public void Execute(IUBotStudio ubotStudio, Dictionary<string, string> parameters)
        {
            var inicontainer = (FBcontainer) ubotStudio.ContainerParent;
            inicontainer.Proxy = parameters["Proxy"];
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
    }
}