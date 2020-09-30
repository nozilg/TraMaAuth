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
            InitRoles();
            InitWorkerFunctions();
            InitWorkers();
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

        private static void InitRoles()
        {
            string name = "Administrator";
            string description = "Administrator";
            int accessLevel = 1;
            if (!Role.Exists(name))
            {
                try
                {
                    Role.Create(name, description, accessLevel);
                }
                catch (Exception ex)
                {
                    throw new Exception(ExceptionBuilder.ComposeMessage(
                        MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex,
                        new string[] { name, description, accessLevel.ToString() }));
                }
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

        private static void InitWorkers()
        {
            string loginName = "Administrator";
            string name = "Administrator";
            string password = Worker.EncodePassword("Administrator");
            string badgeCode = "Administrator";
            string code = "0000";
            string roleName = "Administrator";
            if (!Worker.Exists(loginName))
            {
                try
                {
                    Worker.Create(name, loginName, password, badgeCode, code, roleName);
                }
                catch (Exception ex)
                {
                    throw new Exception(ExceptionBuilder.ComposeMessage(
                        MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex,
                        new string[] { name, loginName, password, badgeCode, code, roleName }));
                }
            }
        }
    }
}
