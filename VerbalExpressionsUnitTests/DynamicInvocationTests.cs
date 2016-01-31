using CSharpVerbalExpressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace VerbalExpressionsUnitTests
{
    [TestFixture]
    class DynamicInvocationTests
    {
        private Dictionary<string, string> returnTypeAlias = new Dictionary<string, string>{
            {"string", "String"}
        };

        [Test]
        [TestCaseSource("DynamicInvocationTestCases")]
        public void TestWithDynamicInvocation(DynamicInvocationTest test)
        {
            string output;
            if (!test.output.TryGetValue("csharp", out output))
            {
                if (!test.output.TryGetValue("default", out output))
                {
                    throw new InvalidOperationException("Cannot invoke dynamic test if no output is present!");
                }
            }

            VerbalExpressions regex = new VerbalExpressions();
            Type regexType = regex.GetType();
            object returnValue = null;
            List<DynamicInvocationMethodCall> testCallStack = new List<DynamicInvocationMethodCall>();

            if (test.pattern.Count > 0)
            {
                testCallStack.AddRange(test.pattern);
            }
            testCallStack.AddRange(test.callStack);

            foreach (DynamicInvocationMethodCall testCall in testCallStack)
            {
                MethodInfo method = regexType.GetMethod(testCall.method);
                List<object> methodArguments = new List<object>();
                bool usingDefaultArguments = true;
                if (testCall.method == "getRegex")
                {
                    method = regexType.GetMethod("ToString");
                }
                else if (method == null)
                {
                    method = regexType.GetMethod(Char.ToUpper(testCall.method[0]) + testCall.method.Substring(1));
                }
                Assert.NotNull(method);
                if (testCall.arguments.Length > 0)
                {
                    methodArguments.AddRange(testCall.arguments);
                    while (methodArguments.Count < method.GetParameters().Length)
                    {
                        methodArguments.Add(Type.Missing);
                    }
                    usingDefaultArguments = false;
                }
                else
                {
                    methodArguments = (method.GetParameters().Select(info => Type.Missing)).ToList();
                }
                bool isParams = (
                    method.GetParameters().Length == 1 &&
                    method.GetParameters()[0].GetCustomAttributes(typeof(ParamArrayAttribute), false).Length > 0
                );
                try
                {
                    returnValue = method.Invoke(regex, isParams ? new object[] { methodArguments.ToArray() } : methodArguments.ToArray());
                }
                catch (Exception ex)
                {
                    throw new TargetParameterCountException("Could not invoke regex." + testCall.method + "()" + (usingDefaultArguments ? " using default arguments" : ""), ex);
                }
                if (testCall.returnType == "sameInstance")
                {
                    Assert.IsInstanceOf<VerbalExpressions>(returnValue);
                    Assert.AreSame(regex, returnValue);
                }
                else if (returnTypeAlias.ContainsKey(testCall.returnType))
                {
                    Assert.AreEqual(returnTypeAlias[testCall.returnType], returnValue.GetType().Name);
                }
                else
                {
                    Assert.AreEqual(testCall.returnType, returnValue.GetType().Name);
                }
            }
            Assert.AreEqual(output, returnValue);
        }

        public static DynamicInvocationTest[] DynamicInvocationTestCases()
        {
            return new DynamicInvocationTest[] {
                new DynamicInvocationTest
                {
                    name = "getRegex",
                    description = "Test getRegex",
                    output = new Dictionary<string, string> {
                        {"default", "/^[0-9a-zA-Z]+/m"},
                        {"csharp", "^[0-9a-zA-Z]+"}
                    },
                    callStack = new List<DynamicInvocationMethodCall> {
                       new DynamicInvocationMethodCall{
                          method = "startOfLine"
                        },
                       new DynamicInvocationMethodCall {
                          method = "range",
                          arguments = new object[] {0, 9, "a", "z", "A", "Z" }
                        },
                        new DynamicInvocationMethodCall{
                          method = "multiple",
                          arguments = new object[] {""}
                        },
                        new DynamicInvocationMethodCall{
                          method = "getRegex",
                          returnType = "string"
                        }
                    }
                }
            };
        }
    }
}
