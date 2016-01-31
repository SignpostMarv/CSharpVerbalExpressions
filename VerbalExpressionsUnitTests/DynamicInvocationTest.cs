using System.Collections.Generic;
namespace VerbalExpressionsUnitTests
{
    class DynamicInvocationTest
    {
        public string name;

        public string description;

        public Dictionary<string, string> output = new Dictionary<string, string>();

        public List<DynamicInvocationMethodCall> callStack = new List<DynamicInvocationMethodCall>();

        public List<DynamicInvocationMethodCall> pattern = new List<DynamicInvocationMethodCall>();
    }
}
