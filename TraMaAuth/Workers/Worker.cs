using Cizeta.TraMaAuth.DataSets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            get { return (this.Id == 0 ? false : true); }
        }

        #endregion

        #region Constructors

        internal Worker() : this(0, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, 0) { }

        internal Worker(
            int workerId,
            string workerName,
            string workerLoginName,
            string workerBadgeCode,
            string workerRoleName,
            string workerCode,
            int workerAccessLevel)
        {
            this.Id = workerId;
            this.Name = workerName;
            this.LoginName = workerLoginName;
            this.Password = string.Empty;
            this.BadgeCode = workerBadgeCode;
            this.RoleName = workerRoleName;
            this.Code = workerCode;
            this.AccessLevel = workerAccessLevel;
            this.StationsAccess = new Dictionary<string, bool>();
            this.StationsLogin = new Dictionary<string, bool>();
        }

        #endregion

        #region Internal methods

        internal void LoadFromDbByLoginName(string workerLoginName)
        {
            WorkersDataSet.GetWorkerByLoginNameDataTable dt;
            DataSets.WorkersDataSetTableAdapters.GetWorkerByLoginNameTableAdapter da = new DataSets.WorkersDataSetTableAdapters.GetWorkerByLoginNameTableAdapter();
            dt = da.GetData(workerLoginName);
            if (dt.Rows.Count == 1)
            {
                foreach (WorkersDataSet.GetWorkerByLoginNameRow dtr in dt)
                {
                    this.Id = dtr.Id;
                    this.Name = dtr.Name;
                    this.LoginName = dtr.LoginName;
                    this.Password = dtr.Password;
                    this.BadgeCode = dtr.BadgeCode;
                    this.Code = dtr.Code;
                    this.RoleName = dtr.RoleName;
                }
                LoadStationsConfigFromDb(this.LoginName);
            }
            else
            {
                throw new Exception(string.Format("WorkerNotFound"));
            }
        }

        internal void LoadFromDbByBadgeCode(string workerBadgeCode)
        {
            WorkersDataSet.GetWorkerByBadgeCodeDataTable dt;
            DataSets.WorkersDataSetTableAdapters.GetWorkerByBadgeCodeTableAdapter da = new DataSets.WorkersDataSetTableAdapters.GetWorkerByBadgeCodeTableAdapter();
            dt = da.GetData(workerBadgeCode);
            if (dt.Rows.Count == 1)
            {
                foreach (WorkersDataSet.GetWorkerByBadgeCodeRow dtr in dt)
                {
                    this.Id = dtr.Id;
                    this.Name = dtr.Name;
                    this.LoginName = dtr.LoginName;
                    this.Password = dtr.Password;
                    this.BadgeCode = dtr.BadgeCode;
                    this.Code = dtr.Code;
                    this.RoleName = dtr.RoleName;
                }
                LoadStationsConfigFromDb(this.LoginName);
            }
            else
            {
                throw new Exception(string.Format("WorkerNotFound"));
            }
        }

        internal void LoadStationsConfigFromDb(string workerLoginName)
        {
            WorkersDataSet.GetStationsConfigForWorkerByLoginNameDataTable dt = default(WorkersDataSet.GetStationsConfigForWorkerByLoginNameDataTable);
            DataSets.WorkersDataSetTableAdapters.GetStationsConfigForWorkerByLoginNameTableAdapter da = new DataSets.WorkersDataSetTableAdapters.GetStationsConfigForWorkerByLoginNameTableAdapter();
            dt = da.GetData(workerLoginName);
            StationsAccess.Clear();
            if (dt.Rows.Count > 0)
            {
                foreach (WorkersDataSet.GetStationsConfigForWorkerByLoginNameRow dtr in dt)
                {
                    StationsAccess.Add(dtr.StationName, dtr.Enabled);
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
