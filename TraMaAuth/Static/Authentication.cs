using System;
using System.Reflection;

namespace Cizeta.TraMaAuth
{
    public static class Authentication
    {
        public static void Init(string connectionString)
        {
            InitConnectionString(connectionString);
            InitAuthenticationModes();
            InitWorkerFunctions();
        }

        private static void InitConnectionString(string connectionString)
        {
            Properties.Settings.Default["TraMaConnectionString"] = connectionString;
        }

        private static void InitAuthenticationModes()
        {
            try
            {
                int id = 0;
                string name = string.Empty;
                string description = string.Empty;
                foreach (AuthenticationMode am in Enum.GetValues(typeof(AuthenticationMode)))
                {
                    try
                    {
                        id = (int)am;
                        name = am.ToString();
                        description = name;
                        using (WorkersDataSetTableAdapters.QueriesTableAdapter q = new WorkersDataSetTableAdapters.QueriesTableAdapter())
                        {
                            q.MergeAuthenticationMode(id, name, description);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ExceptionBuilder.ComposeMessage(
                            MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex,
                            new string[] { id.ToString(), name, description }));
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ExceptionBuilder.ComposeMessage(
                    MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex));
            }
        }

        private static void InitWorkerFunctions()
        {
            try
            {
                int id = 0;
                string name = string.Empty;
                string description = string.Empty;
                foreach (WorkerFunction value in Enum.GetValues(typeof(WorkerFunction)))
                {
                    try
                    {
                        id = (int)value;
                        name = value.ToString();
                        description = name;
                        using (WorkersDataSetTableAdapters.QueriesTableAdapter q = new WorkersDataSetTableAdapters.QueriesTableAdapter())
                        {
                            q.MergeWorkerFunction(id, name, description);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ExceptionBuilder.ComposeMessage(
                            MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex,
                            new string[] { id.ToString(), name, description }));
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ExceptionBuilder.ComposeMessage(
                    MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex));
            }
        }
    }
}
