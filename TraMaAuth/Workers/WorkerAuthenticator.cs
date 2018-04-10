using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cizeta.TraMaAuth
{
    public class WorkerAuthenticator
    {
        public Worker CurrentWorker;
        public string ExceptionMessage;

        private AuthenticationMode AuthenticationMode;
        private string CryptoKey;

        #region Constructors

        public WorkerAuthenticator() : this(AuthenticationMode.Any, string.Empty)
        {

        }

        public WorkerAuthenticator(AuthenticationMode authMode) : this(authMode, string.Empty)
        {

        }

        public WorkerAuthenticator(AuthenticationMode authMode, string cryptoKey)
        {
            this.AuthenticationMode = authMode;
            this.CryptoKey = cryptoKey;
            this.CurrentWorker = new Worker();
            this.ExceptionMessage = string.Empty;
        }

        public WorkerAuthenticator(AuthenticationMode authMode, string cryptoKey, string connectionString)
        {
            this.AuthenticationMode = authMode;
            this.CryptoKey = cryptoKey;
            this.CurrentWorker = new Worker();
            this.ExceptionMessage = string.Empty;
            Properties.Settings.Default["TraMaConnectionString"] = connectionString;
        }

        #endregion

        #region Public methods

        public WorkerLoginResult Login(string workerLoginName, string workerPassword, string workerBadgeCode, string stationName)
        {
            WorkerLoginResult ret = WorkerLoginResult.Failed;
            try
            {
                switch (AuthenticationMode)
                {
                    case AuthenticationMode.Any:
                        ret = Login(workerLoginName, workerPassword, stationName);
                        if (ret != WorkerLoginResult.Ok)
                        {
                            ret = Login(workerBadgeCode, stationName);
                        }
                        break;
                    case AuthenticationMode.UserPassword:
                        ret = Login(workerLoginName, workerPassword, stationName);
                        break;
                    case AuthenticationMode.BadgeCode:
                        ret = Login(workerBadgeCode, stationName);
                        break;
                }
            }
            catch (Exception ex)
            {
                ExceptionMessage = ex.Message;
                ret = WorkerLoginResult.Failed;
            }
            return ret;
        }

        #endregion

        #region Private methods

        private WorkerLoginResult Login(string workerLoginName, string workerPassword, string stationName)
        {
            WorkerLoginResult ret = WorkerLoginResult.Failed;
            try
            {
                if (string.IsNullOrEmpty(workerLoginName))
                {
                    ret = WorkerLoginResult.Failed;
                }
                else
                {
                    CurrentWorker.LoadFromDbByLoginName(workerLoginName);
                    PasswordManager pm = new PasswordManager(CryptoKey);
                    if (pm.CheckPassword(CurrentWorker.Password, workerPassword))
                    {
                        if (CurrentWorker.IsEnabledOnStation(stationName))
                        {
                            ret = WorkerLoginResult.Ok;                            
                        }
                        else
                        {
                            ret = WorkerLoginResult.NotEnabled;
                        }
                    }
                    else
                    {
                        ret = WorkerLoginResult.Failed;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionMessage = ex.Message;
                ret = WorkerLoginResult.Failed;
            }
            return ret;
        }

        private WorkerLoginResult Login(string workerBadgeCode, string stationName)
        {
            WorkerLoginResult ret = WorkerLoginResult.Failed;
            try
            {
                if (string.IsNullOrEmpty(workerBadgeCode))
                {
                    ret = WorkerLoginResult.Failed;
                }
                else
                {
                    CurrentWorker.LoadFromDbByBadgeCode(workerBadgeCode);
                    if ((CurrentWorker.IsValid))
                    {
                        if (CurrentWorker.IsEnabledOnStation(stationName))
                        {
                            ret = WorkerLoginResult.Ok;
                        }
                        else
                        {
                            ret = WorkerLoginResult.NotEnabled;
                        }
                    }
                    else
                    {
                        ret = WorkerLoginResult.Failed;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionMessage = ex.Message;
                ret = WorkerLoginResult.Failed;
            }
            return ret;
        }

        #endregion

        private string GetConnectionString()
        {
            ConfigurationHandler ch = new ConfigurationHandler();
            return ch.GetConnectionString();
        }

        //public WorkerLoginResult Login(string loginName, string password, string badgeCode, string stationName)
        //{
        //    WorkerLoginResult ret = WorkerLoginResult.Failed;
        //    return (ret);
        //}

        //public WorkerLoginResult LoginByBadgeCode(string workerBadgeCode, string stationName)
        //{
        //    WorkerLoginResult ret;
        //    try
        //    {
        //        CurrentWorker = GetWorker(workerBadgeCode);
        //        if (CurrentWorker != null)
        //        {
        //            CurrentWorker.StationsAccess = GetStationsAccess(CurrentWorker.LoginName);
        //            if (CurrentWorker.IsEnabledOnStation(stationName))
        //            {
        //                ret = WorkerLoginResult.Ok;
        //            }
        //            else
        //            {
        //                ret = WorkerLoginResult.NotEnabled;
        //            }
        //        }
        //        else
        //        {
        //            ret = WorkerLoginResult.Failed;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionMessage = ex.Message;
        //        ret = WorkerLoginResult.Failed;
        //    }
        //    return (ret);
        //}

        //private Worker GetWorker(string workerBadgeCode)
        //{
        //    Worker w = new Worker();
        //    try
        //    {
        //        using (SqlConnection connection = new SqlConnection(ConnectionString))
        //        {
        //            connection.Open();
        //            SqlCommand command = new SqlCommand();
        //            command.Connection = connection;
        //            command.CommandType = CommandType.StoredProcedure;
        //            command.CommandText = "GetWorkerByBadgeCodeForLAS";
        //            SqlParameter sqlParamBadgeCode = new SqlParameter();
        //            sqlParamBadgeCode = command.Parameters.Add("@BadgeCode", SqlDbType.NVarChar);
        //            sqlParamBadgeCode.Value = workerBadgeCode;
        //            SqlDataReader reader = command.ExecuteReader();
        //            while (reader.Read())
        //            {
        //                w = new Worker(
        //                    (int)reader["Id"],
        //                    reader["Name"].ToString(),
        //                    reader["LoginName"].ToString(),
        //                    reader["BadgeCode"].ToString(),
        //                    reader["RoleName"].ToString(),
        //                    reader["Code"].ToString(),
        //                    (int)reader["AccessLevel"]);
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //    return w;
        //}

        //private Dictionary<string, bool> GetStationsAccess(string workerLoginName)
        //{
        //    Dictionary<string, bool> d = new Dictionary<string, bool>();
        //    try
        //    {
        //        using (SqlConnection connection = new SqlConnection(ConnectionString))
        //        {
        //            connection.Open();
        //            SqlCommand command = new SqlCommand();
        //            command.Connection = connection;
        //            command.CommandType = CommandType.StoredProcedure;
        //            command.CommandText = "GetWorkerStationsByLoginNameForLAS";
        //            SqlParameter sqlParamBadgeCode = new SqlParameter();
        //            sqlParamBadgeCode = command.Parameters.Add("@LoginName", SqlDbType.NVarChar);
        //            sqlParamBadgeCode.Value = workerLoginName;
        //            SqlDataReader reader = command.ExecuteReader();
        //            while (reader.Read())
        //            {
        //                d.Add(
        //                    reader["StationName"].ToString(),
        //                    (bool)reader["Enabled"]);
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //    return (d);
        //}

    }

}
