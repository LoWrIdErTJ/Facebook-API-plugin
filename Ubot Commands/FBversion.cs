using System.Collections.Generic;
using UBotPlugin;

namespace FB_API_Plugin.Ubot_Commands
{
    internal class FBVersion : IUBotCommand
    {
        private readonly List<UBotParameterDefinition> _parameters = new List<UBotParameterDefinition>();

        public FBVersion()
        {
            var SSLParameter = new UBotParameterDefinition("API version", UBotType.String);
            SSLParameter.Options = new[] {"none", "2.0", "1.0"};
            SSLParameter.DefaultValue = "none";
            _parameters.Add(SSLParameter);
        }

        public string Category
        {
            get { return "FB API Commands"; }
        }

        public string CommandName
        {
            get { return "fb api version"; }
        }

        public void Execute(IUBotStudio ubotStudio, Dictionary<string, string> parameters)
        {
            var inicontainer = (FBcontainer) ubotStudio.ContainerParent;

            if (parameters["API version"] == "2.0")
            {
                inicontainer.Version = "v2.0";
            }
            else if (parameters["API version"] == "1.0")
            {
                inicontainer.Version = "v1.0";
            }
            else
            {
                inicontainer.Version = "none";
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
    }
}