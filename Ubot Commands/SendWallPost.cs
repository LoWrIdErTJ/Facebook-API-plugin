using System;
using System.Collections.Generic;
using UBotPlugin;

namespace FB_API_Plugin.Ubot_Commands
{
    internal class SendWallPost : IUBotCommand
    {
        private readonly List<UBotParameterDefinition> _parameters = new List<UBotParameterDefinition>();

        public SendWallPost()
        {
            _parameters.Add(new UBotParameterDefinition("ProfileId", UBotType.String));
            _parameters.Add(new UBotParameterDefinition("Message", UBotType.String));
        }

        public string Token { get; private set; }
        public string Error { get; set; }

        public string Category
        {
            get { return "FB API Commands"; }
        }

        public string CommandName
        {
            get { return "fb api put wall post"; }
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


                //Post Wall Post 
                if (inicontainer.Attachement.Count == 0)
                {
                    facebook.PutWallPost(parameters["Message"], null, parameters["ProfileId"]);
                }
                else
                {
                    facebook.PutWallPost(parameters["Message"], inicontainer.Attachement, parameters["ProfileId"]);
                }
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
    }
}