using System;
using System.Collections.Generic;
using System.Reflection;

namespace Cizeta.TraMaAuth
{
    public class Worker
    {
        #region Public

        public int Id;
        public string Name;
        public string LoginName;
        public string Password;
        public string BadgeCode;
        public string Code;
        public Role Role;
        public Dictionary<string, bool> Access;

        #endregion

        #region Properties

        public bool IsValid
        {
            get { return Id > 0; }
        }

        #endregion

        #region Constructors

        public Worker() : this(0, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty) { }

        public Worker(int id, string name, string loginName, string badgeCode, string code, string roleName)
        {
            Id = id;
            Name = name;
            LoginName = loginName;
            Password = string.Empty;
            BadgeCode = badgeCode;
            Code = code;
            if (Role.Exists(roleName))
                Role = new Role(roleName);
            else
                Role = new Role();
            Access = new Dictionary<string, bool>();
        }

        public Worker(string loginName)
        {
            if (!string.IsNullOrEmpty(loginName))
                LoadFromDbByLoginName(loginName);
        }

        #endregion

        #region Internal methods

        internal void LoadFromDbByLoginName(string loginName)
        {
            WorkersDataSet.GetWorkerByLoginNameDataTable dt;
            WorkersDataSetTableAdapters.GetWorkerByLoginNameTableAdapter da =
                new WorkersDataSetTableAdapters.GetWorkerByLoginNameTableAdapter();
            dt = da.GetData(loginName);
            if (dt.Rows.Count == 1)
            {
                foreach (WorkersDataSet.GetWorkerByLoginNameRow dtr in dt)
                {
                    Id = dtr.Id;
                    Name = dtr.Name;
                    LoginName = dtr.LoginName;
                    Password = dtr.Password;
                    BadgeCode = dtr.BadgeCode;
                    Code = dtr.Code;
                    Role = new Role(dtr.RoleName);
                    Access = new Dictionary<string, bool>();
                }
                LoadStationsConfigFromDb(loginName);
            }
            //else
            //    throw new Exception("WorkerNotFound");
            //throw new Exception(MainLang.GetMsgText("Errors", "WorkerNotFound"));
        }

        internal void LoadFromDbByBadgeCode(string badgeCode)
        {
            WorkersDataSet.GetWorkerByBadgeCodeDataTable dt;
            WorkersDataSetTableAdapters.GetWorkerByBadgeCodeTableAdapter da =
                new WorkersDataSetTableAdapters.GetWorkerByBadgeCodeTableAdapter();
            dt = da.GetData(badgeCode);
            if (dt.Rows.Count == 1)
            {
                foreach (WorkersDataSet.GetWorkerByBadgeCodeRow dtr in dt)
                {
                    Id = dtr.Id;
                    Name = dtr.Name;
                    LoginName = dtr.LoginName;
                    Password = dtr.Password;
                    BadgeCode = dtr.BadgeCode;
                    Code = dtr.Code;
                    Role = new Role(dtr.RoleName);
                    Access = new Dictionary<string, bool>();
                }
                LoadStationsConfigFromDb(LoginName);
            }
            //else
            //    throw new Exception("WorkerNotFound");
            //throw new Exception(MainLang.GetMsgText("Errors", "WorkerNotFound"));
        }

        internal void LoadStationsConfigFromDb(string loginName)
        {
            WorkersDataSet.GetStationsConfigForWorkerByLoginNameDataTable dt;
            WorkersDataSetTableAdapters.GetStationsConfigForWorkerByLoginNameTableAdapter da =
                new WorkersDataSetTableAdapters.GetStationsConfigForWorkerByLoginNameTableAdapter();
            dt = da.GetData(loginName);
            Access.Clear();
            if (dt.Rows.Count > 0)
            {
                foreach (WorkersDataSet.GetStationsConfigForWorkerByLoginNameRow dtr in dt)
                    Access.Add(dtr.StationName, dtr.Enabled);
            }
        }

        #endregion

        #region Public methods

        public bool IsEnabledOnStation(string stationName)
        {
            return Access[stationName];
        }

        public bool HasPermissionTo(WorkerFunction workerFunction)
        {
            return GetFunctionPermission(LoginName, workerFunction);
        }

        public bool CheckPassword(string password)
        {
            return EncodePassword(password) == Password;
        }

        #region Login by badge code

        public WorkerLoginResult Login(string stationName, string badgeCode)
        {
            return Login(stationName, badgeCode, WorkerFunction.None);
        }

        public WorkerLoginResult Login(string stationName, string badgeCode, WorkerFunction workerFunction)
        {
            return Login(stationName, badgeCode, workerFunction, Properties.Settings.Default.TraMaConnectionString);
        }

        public WorkerLoginResult Login(string stationName, string badgeCode, WorkerFunction workerFunction, string connectionString)
        {
            LoadFromDbByBadgeCode(badgeCode);
            WorkerAuthenticator wa = new WorkerAuthenticator(AuthenticationMode.BadgeCode, connectionString);
            WorkerLoginResult ret = wa.Login(string.Empty, string.Empty, badgeCode, stationName, workerFunction);
            if (ret == WorkerLoginResult.Ok)
            {
                LoadFromDbByBadgeCode(badgeCode);
                UpdateLoginDateOnStation(Id, stationName, DateTime.Now);
            }
            return ret;
        }

        #endregion

        #region Login by username/password

        public WorkerLoginResult Login(string stationName, string loginName, string password)
        {
            return Login(stationName, loginName, password, WorkerFunction.None);
        }

        public WorkerLoginResult Login(string stationName, string loginName, string password, WorkerFunction workerFunction)
        {
            return Login(stationName, loginName, password, workerFunction, Properties.Settings.Default.TraMaConnectionString);
        }

        public WorkerLoginResult Login(string stationName, string loginName, string password, WorkerFunction workerFunction, string connectionString)
        {
            LoadFromDbByLoginName(loginName);
            WorkerAuthenticator wa = new WorkerAuthenticator(AuthenticationMode.UserPassword, connectionString);
            WorkerLoginResult ret = wa.Login(loginName, password, string.Empty, stationName, workerFunction);
            if (ret == WorkerLoginResult.Ok)
                UpdateLoginDateOnStation(Id, stationName, DateTime.Now);
            return ret;
        }

        #endregion

        public void Logout(string stationName)
        {
            UpdateLogoutDateOnStation(Id, stationName, DateTime.Now);
            Clear();
        }

        #endregion

        #region Public static methods

        public static string EncodePassword(string password)
        {
            return new WorkerAuthenticator().EncodePassword(password);
        }

        public static string DecodePassword(string password)
        {
            return new WorkerAuthenticator().DecodePassword(password);
        }

        #endregion

        #region Private methods

        private void Clear()
        {
            Id = 0;
            Name = string.Empty;
            LoginName = string.Empty;
            Password = string.Empty;
            BadgeCode = string.Empty;
            Code = string.Empty;
            Role = new Role();
            Access = new Dictionary<string, bool>();
        }

        #endregion

        #region Static database functions

        public static bool Exists(string loginName)
        {
            bool ret = false;
            try
            {
                using (WorkersDataSetTableAdapters.QueriesTableAdapter q = new WorkersDataSetTableAdapters.QueriesTableAdapter())
                {
                    ret = q.WorkerExists(loginName) ?? false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ExceptionBuilder.ComposeMessage(
                    MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex,
                    new string[] { loginName }));
            }
            return ret;
        }

        public static bool ExistsByCode(string code)
        {
            bool ret = false;
            try
            {
                using (WorkersDataSetTableAdapters.QueriesTableAdapter q = new WorkersDataSetTableAdapters.QueriesTableAdapter())
                {
                    ret = q.WorkerExistsByCode(code) ?? false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ExceptionBuilder.ComposeMessage(
                    MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex,
                    new string[] { code }));
            }
            return ret;
        }

        public static int Create(string name, string loginName, string password, string badgeCode, string code, string roleName)
        {
            int ret = 0;
            try
            {
                using (WorkersDataSetTableAdapters.QueriesTableAdapter q = new WorkersDataSetTableAdapters.QueriesTableAdapter())
                {
                    q.CreateWorker(name, loginName, password, badgeCode, code, roleName);
                    ret = q.GetLastId("Workers") ?? 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ExceptionBuilder.ComposeMessage(
                    MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex,
                    new string[] { name, loginName, password, badgeCode, code, roleName }));
            }
            return ret;
        }

        public static void Merge(int id, string name, string loginName, string password, string badgeCode, string code, string roleName, string userRoleName)
        {
            try
            {
                using (WorkersDataSetTableAdapters.QueriesTableAdapter q = new WorkersDataSetTableAdapters.QueriesTableAdapter())
                {
                    q.MergeWorker(id, name, loginName, password, badgeCode, code, roleName);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ExceptionBuilder.ComposeMessage(
                    MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex,
                    new string[] { name, loginName, password, badgeCode, code, roleName }));
            }
        }

        public static void Delete(string loginName)
        {
            try
            {
                using (WorkersDataSetTableAdapters.QueriesTableAdapter q = new WorkersDataSetTableAdapters.QueriesTableAdapter())
                {
                    q.DeleteWorker(loginName);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ExceptionBuilder.ComposeMessage(
                    MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex,
                    new string[] { loginName }));
            }
        }

        public static int GetId(string loginName)
        {
            int ret = 0;
            try
            {
                using (WorkersDataSetTableAdapters.QueriesTableAdapter q = new WorkersDataSetTableAdapters.QueriesTableAdapter())
                {
                    ret = q.GetWorkerId(loginName) ?? 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ExceptionBuilder.ComposeMessage(
                    MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex,
                    new string[] { loginName }));
            }
            return ret;
        }

        public static string GetLoginName(int id)
        {
            string ret = string.Empty;
            try
            {
                using (WorkersDataSetTableAdapters.QueriesTableAdapter q = new WorkersDataSetTableAdapters.QueriesTableAdapter())
                {
                    ret = q.GetWorkerLoginName(id);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ExceptionBuilder.ComposeMessage(
                    MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex,
                    new string[] { id.ToString() }));
            }
            return ret;
        }

        public static string GetName(int id)
        {
            string ret = string.Empty;
            try
            {
                using (WorkersDataSetTableAdapters.QueriesTableAdapter q = new WorkersDataSetTableAdapters.QueriesTableAdapter())
                {
                    ret = q.GetWorkerName(id);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ExceptionBuilder.ComposeMessage(
                    MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex,
                    new string[] { id.ToString() }));
            }
            return ret;
        }

        public static bool GetFunctionPermission(string loginName, WorkerFunction workerFunction)
        {
            bool ret = false;
            try
            {
                using (WorkersDataSetTableAdapters.QueriesTableAdapter q = new WorkersDataSetTableAdapters.QueriesTableAdapter())
                {
                    ret = q.GetWorkerPermission(loginName, workerFunction.ToString()) ?? false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ExceptionBuilder.ComposeMessage(
                    MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex,
                    new string[] { loginName, workerFunction.ToString() }));
            }
            return ret;
        }

        public static void UpdatePassword(string loginName, string password)
        {
            try
            {
                using (WorkersDataSetTableAdapters.QueriesTableAdapter q = new WorkersDataSetTableAdapters.QueriesTableAdapter())
                {
                    q.UpdateWorkerPassword(loginName, password);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ExceptionBuilder.ComposeMessage(
                    MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex,
                    new string[] { loginName }));
            }
        }

        public static void UpdateLoginDateOnStation(int id, string stationName, DateTime loginDate)
        {
            try
            {
                using (WorkersDataSetTableAdapters.QueriesTableAdapter q = new WorkersDataSetTableAdapters.QueriesTableAdapter())
                {
                    q.UpdateStationWorkerLoginDate(id, stationName, loginDate);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ExceptionBuilder.ComposeMessage(
                    MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex,
                    new string[] { id.ToString(), stationName, loginDate.ToString() }));
            }
        }

        public static void UpdateLogoutDateOnStation(int id, string stationName, DateTime logoutDate)
        {
            try
            {
                using (WorkersDataSetTableAdapters.QueriesTableAdapter q = new WorkersDataSetTableAdapters.QueriesTableAdapter())
                {
                    q.UpdateStationWorkerLogoutDate(id, stationName, logoutDate);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ExceptionBuilder.ComposeMessage(
                    MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex,
                    new string[] { id.ToString(), stationName, logoutDate.ToString() }));
            }
        }

        #endregion
    }
}
