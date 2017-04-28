using System;
using System.Collections.Generic;
using UBotPlugin;

namespace FB_API_Plugin.Ubot_Commands
{
    internal class Post_Comment : IUBotCommand
    {
        private readonly List<UBotParameterDefinition> _parameters = new List<UBotParameterDefinition>();

        public Post_Comment()
        {
            _parameters.Add(new UBotParameterDefinition("Object ID", UBotType.String));
            _parameters.Add(new UBotParameterDefinition("Comment", UBotType.String));
        }

        public string Token { get; private set; }
        public string Error { get; set; }

        public string Category
        {
            get { return "FB API Commands"; }
        }

        public string CommandName
        {
            get { return "fb api post comment"; }
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

                //post a comment
                var putobject = facebook.PutComment(parameters["Object ID"], parameters["Comment"]);
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