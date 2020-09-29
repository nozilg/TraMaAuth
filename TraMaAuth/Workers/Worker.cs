using System;
using System.Collections.Generic;
using System.Reflection;

namespace Cizeta.TraMaAuth
{
    public class Worker
    {
        public int Id;
        public string Name;
        public string LoginName;
        public string Password;
        public string BadgeCode;
        public string Code;
        public Role Role;
        public Dictionary<string, bool> Access;

        public event LoginDoneEventHandler LoginDone;
        public delegate void LoginDoneEventHandler(Worker worker);

        public event LogoutDoneEventHandler LogoutDone;
        public delegate void LogoutDoneEventHandler(Worker worker);

        public Worker() : this(0, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty) { }

        public Worker(int id, string name, string loginName, string badgeCode, string code, string roleName)
        {
            Id = id;
            Name = name;
            LoginName = loginName;
            Password = string.Empty;
            BadgeCode = badgeCode;
            Code = code;
            try
            {
                Role = new Role(roleName);
            }
            catch (Exception)
            {
                Role = new Role();
            }
            Access = new Dictionary<string, bool>();
        }

        public void LoadFromDbByLoginName(string workerLoginName)
        {
            WorkersDataSet.GetWorkerByLoginNameDataTable dt;
            WorkersDataSetTableAdapters.GetWorkerByLoginNameTableAdapter da =
                new WorkersDataSetTableAdapters.GetWorkerByLoginNameTableAdapter();
            dt = da.GetData(workerLoginName);
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
                LoadStationsConfigFromDb(workerLoginName);
            }
            //else
            //    throw new Exception("WorkerNotFound");
            //throw new Exception(MainLang.GetMsgText("Errors", "WorkerNotFound"));
        }

        public void LoadFromDbByBadgeCode(string workerBadgeCode)
        {
            WorkersDataSet.GetWorkerByBadgeCodeDataTable dt;
            WorkersDataSetTableAdapters.GetWorkerByBadgeCodeTableAdapter da =
                new WorkersDataSetTableAdapters.GetWorkerByBadgeCodeTableAdapter();
            dt = da.GetData(workerBadgeCode);
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
                }
                LoadStationsConfigFromDb(LoginName);
            }
            //else
            //    throw new Exception("WorkerNotFound");
            //throw new Exception(MainLang.GetMsgText("Errors", "WorkerNotFound"));
        }

        public void LoadStationsConfigFromDb(string workerLoginName)
        {
            WorkersDataSet.GetStationsConfigForWorkerByLoginNameDataTable dt;
            WorkersDataSetTableAdapters.GetStationsConfigForWorkerByLoginNameTableAdapter da =
                new WorkersDataSetTableAdapters.GetStationsConfigForWorkerByLoginNameTableAdapter();
            dt = da.GetData(workerLoginName);
            Access.Clear();
            if (dt.Rows.Count > 0)
            {
                foreach (WorkersDataSet.GetStationsConfigForWorkerByLoginNameRow dtr in dt)
                    Access.Add(dtr.StationName, dtr.Enabled);
            }
        }

        public bool HasPermissionTo(WorkerFunction workerFunction)
        {
            return GetFunctionPermission(LoginName, workerFunction);
        }

        public void Login(string stationName, DateTime loginDate)
        {
            try
            {
                UpdateLoginDataOnStation(stationName, loginDate);
                LoginDone?.Invoke(this);
            }
            catch (Exception)
            {

            }
        }

        public void Logout()
        {
            LogoutDone?.Invoke(this);
        }

        public void Logout(string stationName, DateTime logoutDate)
        {
            try
            {
                UpdateLogoutDataOnStation(stationName, logoutDate);
                LogoutDone?.Invoke(this);
            }
            catch (Exception)
            {

            }
        }

        private void UpdateLoginDataOnStation(string stationName, DateTime loginDate)
        {
        }

        private void UpdateLogoutDataOnStation(string stationName, DateTime logoutDate)
        {
        }

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

            #endregion

        }
    }
}
