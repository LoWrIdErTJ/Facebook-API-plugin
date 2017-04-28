using System.Collections.Generic;
using FB_API_Plugin.Ubot_Commands;
using UBotPlugin;

namespace FB_API_Plugin.Ubot_Functions
{
    internal class FBError : IUBotFunction
    {
        private object _lockit = new object();
        // List to hold the parameters we define for our command.
        private readonly List<UBotParameterDefinition> _parameters = new List<UBotParameterDefinition>();
        private string _returnValue;

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
            get { return "$fb error"; }
        }

        public void Execute(IUBotStudio ubotStudio, Dictionary<string, string> parameters)
        {
            var inicontainer = (FBcontainer) ubotStudio.ContainerParent;
            _returnValue = inicontainer.Error;
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