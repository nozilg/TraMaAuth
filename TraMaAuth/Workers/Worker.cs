using Cizeta.TraMaAuth.DataSets;
using System;
using System.Collections.Generic;

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
        public string RoleName;
        public string Code;
        public int AccessLevel;
        public Dictionary<string, bool> StationsAccess;
        public Dictionary<string, bool> StationsLogin;

        #endregion

        #region Properties

        public bool IsValid
        {
            get { return (Id == 0 ? false : true); }
        }

        #endregion

        #region Constructors

        internal Worker() : this(0, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, 0) { }

        internal Worker(
            int id,
            string name,
            string loginName,
            string badgeCode,
            string roleName,
            string code,
            int accessLevel)
        {
            Id = id;
            Name = name;
            LoginName = loginName;
            Password = string.Empty;
            BadgeCode = badgeCode;
            RoleName = roleName;
            Code = code;
            AccessLevel = accessLevel;
            StationsAccess = new Dictionary<string, bool>();
            StationsLogin = new Dictionary<string, bool>();
        }

        #endregion

        #region Internal methods

        internal void LoadFromDbByLoginName(string workerLoginName)
        {
            WorkersDataSet.GetWorkerByLoginNameDataTable dt;
            DataSets.WorkersDataSetTableAdapters.GetWorkerByLoginNameTableAdapter da = 
                new DataSets.WorkersDataSetTableAdapters.GetWorkerByLoginNameTableAdapter();
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
                    RoleName = dtr.RoleName;
                    Code = dtr.Code;
                    AccessLevel = dtr.AccessLevel;
                }
                LoadStationsConfigFromDb(LoginName);
            }
            else
            {
                throw new Exception(string.Format("WorkerNotFound"));
            }
        }

        internal void LoadFromDbByBadgeCode(string workerBadgeCode)
        {
            WorkersDataSet.GetWorkerByBadgeCodeDataTable dt;
            DataSets.WorkersDataSetTableAdapters.GetWorkerByBadgeCodeTableAdapter da = 
                new DataSets.WorkersDataSetTableAdapters.GetWorkerByBadgeCodeTableAdapter();
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
                    RoleName = dtr.RoleName;
                    Code = dtr.Code;
                    AccessLevel = dtr.AccessLevel;
                }
                LoadStationsConfigFromDb(LoginName);
            }
            else
            {
                throw new Exception(string.Format("WorkerNotFound"));
            }
        }

        internal void LoadStationsConfigFromDb(string workerLoginName)
        {
            WorkersDataSet.GetStationsConfigForWorkerByLoginNameDataTable dt;
            DataSets.WorkersDataSetTableAdapters.GetStationsConfigForWorkerByLoginNameTableAdapter da = 
                new DataSets.WorkersDataSetTableAdapters.GetStationsConfigForWorkerByLoginNameTableAdapter();
            dt = da.GetData(workerLoginName);
            StationsAccess.Clear();
            StationsLogin.Clear();
            if (dt.Rows.Count > 0)
            {
                foreach (WorkersDataSet.GetStationsConfigForWorkerByLoginNameRow dtr in dt)
                {
                    StationsAccess.Add(dtr.StationName, dtr.Enabled);
                    StationsLogin.Add(dtr.StationName, false);
                }
            }
        }

        #endregion

        #region Public methods

        public bool IsEnabledOnStation(string stationName)
        {
            return (StationsAccess[stationName]);
        }

        public bool IsLoggedOnStation(string stationName)
        {
            return (StationsLogin[stationName]);
        }

        #endregion

    }
}
