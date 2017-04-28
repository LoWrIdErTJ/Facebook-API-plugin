using System;
using System.Collections.Generic;
using System.Linq;
using UBotPlugin;

namespace FB_API_Plugin.Ubot_Commands
{
    internal class FbFriendsDetails : IUBotCommand
    {
        private readonly List<UBotParameterDefinition> _parameters = new List<UBotParameterDefinition>();

        public FbFriendsDetails()
        {
            _parameters.Add(new UBotParameterDefinition("Parent", UBotType.String));
            _parameters.Add(new UBotParameterDefinition("Details", UBotType.UBotTable));
        }

        public string Token { get; private set; }
        public string Error { get; set; }

        public string Category
        {
            get { return "FB API Commands"; }
        }

        public string CommandName
        {
            get { return "fb api get friends"; }
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
                //get my friends info
                var friends = facebook.GetConnections(parameters["Parent"], "friends", null);

                var table = new string[friends["data"].Count(), 2];

                var i = 0;
                foreach (var friend in friends["data"])
                {
                    table[i, 0] = friend["id"].ToString();
                    table[i, 1] = friend["name"].ToString();


                    i++;
                }
                ubotStudio.SetTable(parameters["Details"], table);
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