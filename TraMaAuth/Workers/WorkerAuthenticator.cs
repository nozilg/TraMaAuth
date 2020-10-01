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

        private readonly AuthenticationMode AuthenticationMode;
        private readonly string CryptoKey;

        #endregion

        #region Constructors

        public WorkerAuthenticator() : this(AuthenticationMode.Any) { }

        public WorkerAuthenticator(AuthenticationMode authenticationMode) : this(authenticationMode, Properties.Settings.Default.TraMaConnectionString) { }

        public WorkerAuthenticator(AuthenticationMode authenticationMode, string connectionString)
        {
            AuthenticationMode = authenticationMode;
            CryptoKey = "sfsoerkgalap";
            CurrentWorker = new Worker();
            ExceptionMessage = string.Empty;
            Properties.Settings.Default["TraMaConnectionString"] = connectionString;
        }

        #endregion

        #region Public methods

        public WorkerLoginResult Login(string loginName, string password, string badgeCode, string stationName)
        {
            return Login(loginName, password, badgeCode, stationName, WorkerFunction.None);
        }

        public WorkerLoginResult Login(string loginName, string password, string badgeCode, string stationName, WorkerFunction workerFunction)
        {
            WorkerLoginResult ret = WorkerLoginResult.Failed;
            try
            {
                switch (AuthenticationMode)
                {
                    case AuthenticationMode.Any:
                        ret = Login(loginName, password, stationName, workerFunction);
                        if (ret != WorkerLoginResult.Ok)
                            ret = Login(badgeCode, stationName, workerFunction);
                        break;
                    case AuthenticationMode.UserPassword:
                        ret = Login(loginName, password, stationName, workerFunction);
                        break;
                    case AuthenticationMode.BadgeCode:
                        ret = Login(badgeCode, stationName, workerFunction);
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

        #region Internal methods

        internal string EncodePassword(string password)
        {
            return new PasswordManager(CryptoKey).EncodePassword(password);
        }

        internal string DecodePassword(string password)
        {
            return new PasswordManager(CryptoKey).DecodePassword(password);
        }

        #endregion

        #region Private methods

        private WorkerLoginResult Login(string workerLoginName, string workerPassword, string stationName, WorkerFunction workerFunction)
        {
            WorkerLoginResult ret;
            if (!string.IsNullOrEmpty(workerLoginName))
                try
                {
                    CurrentWorker.LoadFromDbByLoginName(workerLoginName);
                    PasswordManager pm = new PasswordManager(CryptoKey);
                    ret = pm.CheckPassword(CurrentWorker.Password, workerPassword) ? WorkerLoginResult.Ok : WorkerLoginResult.NotEnabled;
                    if (ret == WorkerLoginResult.Ok)
                        ret = CurrentWorker.IsEnabledOnStation(stationName) ? WorkerLoginResult.Ok : WorkerLoginResult.NotEnabled;
                    if (ret == WorkerLoginResult.Ok)
                        if (workerFunction != WorkerFunction.None)
                            ret = CurrentWorker.HasPermissionTo(workerFunction) ? WorkerLoginResult.Ok : WorkerLoginResult.NotEnabled;
                }
                catch (Exception ex)
                {
                    ExceptionMessage = ex.Message;
                    ret = WorkerLoginResult.Failed;
                }
            else
                ret = WorkerLoginResult.Failed;
            return ret;
        }

        private WorkerLoginResult Login(string workerBadgeCode, string stationName, WorkerFunction workerFunction)
        {
            WorkerLoginResult ret;
            if (!string.IsNullOrEmpty(workerBadgeCode))
                try
                {
                    CurrentWorker.LoadFromDbByBadgeCode(workerBadgeCode);
                    ret = CurrentWorker.IsValid ? WorkerLoginResult.Ok : WorkerLoginResult.Failed;
                    if (ret == WorkerLoginResult.Ok)
                        ret = CurrentWorker.IsEnabledOnStation(stationName) ? WorkerLoginResult.Ok : WorkerLoginResult.NotEnabled;
                    if (ret == WorkerLoginResult.Ok)
                        if (workerFunction != WorkerFunction.None)
                            ret = CurrentWorker.HasPermissionTo(workerFunction) ? WorkerLoginResult.Ok : WorkerLoginResult.NotEnabled;
                }
                catch (Exception ex)
                {
                    ExceptionMessage = ex.Message;
                    ret = WorkerLoginResult.Failed;
                }
            else
                ret = WorkerLoginResult.Failed;
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
