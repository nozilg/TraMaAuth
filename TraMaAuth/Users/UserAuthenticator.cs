using System;

namespace Cizeta.TraMaAuth
{
    public class UserAuthenticator
    {

        #region Public members

        public User CurrentUser;
        public string ExceptionMessage;

        #endregion

        #region Private members

        private readonly AuthenticationMode AuthenticationMode;
        private readonly string CryptoKey;

        #endregion

        #region Constructors

        public UserAuthenticator() : this(AuthenticationMode.Any) { }

        public UserAuthenticator(AuthenticationMode authenticationMode) : this(authenticationMode, Properties.Settings.Default.TraMaConnectionString) { }

        public UserAuthenticator(AuthenticationMode authenticationMode, string connectionString)
        {
            AuthenticationMode = authenticationMode;
            CryptoKey = "wpcuklseraox";
            CurrentUser = new User();
            ExceptionMessage = string.Empty;
            Properties.Settings.Default["TraMaConnectionString"] = connectionString;
        }

        #endregion

        #region Public methods

        //public UserLoginResult AutoLogin(string userLoginName)
        //{
        //    UserLoginResult ret;
        //    try
        //    {
        //        User u = new User();
        //        u.LoadFromDbByLoginName(userLoginName);
        //        PasswordManager pm = new PasswordManager(CryptoKey);
        //        ret = Login(u.LoginName, pm.DecodePassword(u.Password));
        //        if (ret == UserLoginResult.Ok)
        //        {
        //            CurrentUser.IsLogged = true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionMessage = ex.Message;
        //        ret = UserLoginResult.Failed;
        //    }
        //    return ret;
        //}

        public UserLoginResult Login(string userLoginName, string userPassword, string userBadgeCode)
        {
            UserLoginResult ret = UserLoginResult.Failed;
            try
            {
                switch (AuthenticationMode)
                {
                    case AuthenticationMode.Any:
                        ret = Login(userLoginName, userPassword);
                        if (ret != UserLoginResult.Ok)
                            ret = Login(userBadgeCode);
                        break;
                    case AuthenticationMode.UserPassword:
                        ret = Login(userLoginName, userPassword);
                        break;
                    case AuthenticationMode.BadgeCode:
                        ret = Login(userBadgeCode);
                        break;
                }
                if (ret == UserLoginResult.Ok)
                    CurrentUser.IsLogged = true;
            }
            catch (Exception ex)
            {
                ExceptionMessage = ex.Message;
                ret = UserLoginResult.Failed;
            }
            return ret;
        }

        public void Logout()
        {
            CurrentUser.IsLogged = false;
        }

        #endregion

        #region Internal methoods

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

        private UserLoginResult Login(string loginName, string password)
        {
            UserLoginResult ret;
            if (!string.IsNullOrEmpty(loginName))
                try
                {
                    CurrentUser.LoadFromDbByLoginName(loginName);
                    PasswordManager pm = new PasswordManager(CryptoKey);
                    ret = pm.CheckPassword(CurrentUser.Password, password) ? UserLoginResult.Ok : UserLoginResult.Failed;
                }
                catch (Exception ex)
                {
                    ExceptionMessage = ex.Message;
                    ret = UserLoginResult.Failed;
                }
            else
                ret = UserLoginResult.Failed;
            return ret;
        }

        private UserLoginResult Login(string badgeCode)
        {
            UserLoginResult ret;
            if (!string.IsNullOrEmpty(badgeCode))
                try
                {
                    CurrentUser.LoadFromDbByBadgeCode(badgeCode);
                    ret = CurrentUser.IsValid ? UserLoginResult.Ok : UserLoginResult.Failed;
                }
                catch (Exception ex)
                {
                    ExceptionMessage = ex.Message;
                    ret = UserLoginResult.Failed;
                }
            else
                ret = UserLoginResult.Failed;
            return ret;
        }

        #endregion

    }
}
