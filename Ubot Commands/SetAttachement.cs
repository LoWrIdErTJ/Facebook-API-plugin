using System;
using System.Collections.Generic;
using UBotPlugin;

namespace FB_API_Plugin.Ubot_Commands
{
    internal class SetAttachement : IUBotCommand
    {
        private readonly List<UBotParameterDefinition> _parameters = new List<UBotParameterDefinition>();

        public SetAttachement()
        {
            var SSLParameter = new UBotParameterDefinition("Attachement Name", UBotType.String);
            SSLParameter.Options = new[] {"name", "link", "caption", "description", "picture"};
            _parameters.Add(SSLParameter);
            _parameters.Add(new UBotParameterDefinition("Value", UBotType.String));
        }

        public string Token { get; private set; }
        public string Error { get; set; }

        public string Category
        {
            get { return "FB API Commands"; }
        }

        public string CommandName
        {
            get { return "fb api message attachement"; }
        }

        public void Execute(IUBotStudio ubotStudio, Dictionary<string, string> parameters)
        {
            var inicontainer = (FBcontainer) ubotStudio.ContainerParent;
            var AttName = parameters["Attachement Name"];
            //string token = inicontainer.Token;
            try
            {
                switch (AttName)
                {
                    case "name":
                        inicontainer.Attachement.Add("name", parameters["Value"]);
                        break;

                    case "link":
                        inicontainer.Attachement.Add("link", parameters["Value"]);
                        break;

                    case "caption":
                        inicontainer.Attachement.Add("caption", parameters["Value"]);
                        break;

                    case "description":
                        inicontainer.Attachement.Add("description", parameters["Value"]);
                        break;

                    case "picture":
                        inicontainer.Attachement.Add("picture", parameters["Value"]);
                        break;

                    default:
                        break;
                }
            }
            catch (Exception exp)
            {
                inicontainer.Error = exp.Message;
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