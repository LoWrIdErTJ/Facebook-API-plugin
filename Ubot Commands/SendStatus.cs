using System;
using System.Collections.Generic;
using UBotPlugin;

namespace FB_API_Plugin.Ubot_Commands
{
    internal class SendStatus : IUBotCommand
    {
        private readonly List<UBotParameterDefinition> _parameters = new List<UBotParameterDefinition>();

        public SendStatus()
        {
            _parameters.Add(new UBotParameterDefinition("Parent", UBotType.String));
            _parameters.Add(new UBotParameterDefinition("Status", UBotType.String));
        }

        public string Token { get; private set; }
        public string Error { get; set; }

        public string Category
        {
            get { return "FB API Commands"; }
        }

        public string CommandName
        {
            get { return "fb api status"; }
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
                //post to my wall
                var data = new Dictionary<string, string>();
                data.Add("message", parameters["Status"]);
                var putobject = facebook.PutObject(parameters["Parent"], "feed", data);

                //Post Wall Post 
                //facebook.PutWallPost(parameters["Status"], null, parameters["Parent"]);
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