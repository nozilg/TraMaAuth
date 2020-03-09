using System;
using System.Linq;

namespace Cizeta.TraMaAuth
{
    static class ExceptionBuilder
    {
        internal static string ComposeMessage(string className, string methodName, Exception ex)
        {
            return string.Format("[{0}.{1}] {2}", className, methodName, ex.Message);
        }

        internal static string ComposeMessage(string className, string methodName, Exception ex, params string[] parameters)
        {
            string parametersString = string.Empty;
            for (int i = 0; i < parameters.Count(); i++)
            {
                if (i == 0)
                    parametersString += string.Format("{0}", parameters[i]);
                else
                    parametersString += string.Format(", {0}", parameters[i]);
            }
            return string.Format("[{0}.{1}({2})] {3}", className, methodName, parametersString, ex.Message);
        }
    }
}
