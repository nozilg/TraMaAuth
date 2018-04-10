using System;

namespace Cizeta.TraMaAuth
{
    public class WorkerAuthenticator
    {

        #region Public members

        public Worker CurrentWorker;
        public string ExceptionMessage;

        #endregion

        #region Private members

        private AuthenticationMode AuthenticationMode;
        private string CryptoKey;

        #endregion

        #region Constructors

        public WorkerAuthenticator() : this(AuthenticationMode.Any) { }

        public WorkerAuthenticator(AuthenticationMode authMode) : this(authMode, Properties.Settings.Default.TraMaConnectionString) { }

        public WorkerAuthenticator(AuthenticationMode authMode, string connectionString)
        {
            this.AuthenticationMode = authMode;
            this.CryptoKey = "sfsoerkgalap";
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
                if (ret == WorkerLoginResult.Ok)
                {
                    CurrentWorker.StationsLogin[stationName] = true;
                }
            }
            catch (Exception ex)
            {
                ExceptionMessage = ex.Message;
                ret = WorkerLoginResult.Failed;
            }
            return ret;
        }

        public void Logout(string stationName)
        {
            CurrentWorker.StationsLogin[stationName] = false;
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

        private string GetConnectionString()
        {
            ConfigurationHandler ch = new ConfigurationHandler();
            return ch.GetConnectionString();
        }

        #endregion

    }

}
