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

        private AuthenticationMode AuthenticationMode;
        private string CryptoKey;

        #endregion

        #region Constructors

        public UserAuthenticator() : this(AuthenticationMode.Any) { }

        public UserAuthenticator(AuthenticationMode authenticationMode) : this(authenticationMode, Properties.Settings.Default.TraMaConnectionString) { }

        public UserAuthenticator(AuthenticationMode authenticationMode, string connectionString)
        {
            this.AuthenticationMode = authenticationMode;
            this.CryptoKey = "wpcuklseraox";
            this.CurrentUser = new User();
            this.ExceptionMessage = string.Empty;
            Properties.Settings.Default["TraMaConnectionString"] = connectionString;
        }

        #endregion

        #region Public methods

        public UserLoginResult AutoLogin(string userLoginName)
        {
            UserLoginResult ret;
            try
            {
                User u = new User();
                u.LoadFromDbByLoginName(userLoginName);
                PasswordManager pm = new PasswordManager(CryptoKey);
                ret = Login(u.LoginName, pm.DecodePassword(u.Password));
                if (ret == UserLoginResult.Ok)
                {
                    CurrentUser.IsLogged = true;
                }
            }
            catch (Exception ex)
            {
                ExceptionMessage = ex.Message;
                ret = UserLoginResult.Failed;
            }
            return ret;
        }

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
                        {
                            ret = Login(userBadgeCode);
                        }
                        break;
                    case AuthenticationMode.UserPassword:
                        ret = Login(userLoginName, userPassword);
                        break;
                    case AuthenticationMode.BadgeCode:
                        ret = Login(userBadgeCode);
                        break;
                }
                if (ret == UserLoginResult.Ok)
                {
                    CurrentUser.IsLogged = true;
                }
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

        #region Private methods

        private UserLoginResult Login(string userLoginName, string userPassword)
        {
            UserLoginResult ret;
            try
            {
                if (string.IsNullOrEmpty(userLoginName))
                {
                    ret = UserLoginResult.Failed;
                }
                else
                {
                    CurrentUser.LoadFromDbByLoginName(userLoginName);
                    PasswordManager pm = new PasswordManager(CryptoKey);
                    if (pm.CheckPassword(CurrentUser.Password, userPassword))
                    {
                        ret = UserLoginResult.Ok;
                    }
                    else
                    {
                        ret = UserLoginResult.Failed;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionMessage = ex.Message;
                ret = UserLoginResult.Failed;
            }
            return ret;
        }

        private UserLoginResult Login(string userBadgeCode)
        {
            UserLoginResult ret;
            try
            {
                if (string.IsNullOrEmpty(userBadgeCode))
                {
                    ret = UserLoginResult.Failed;
                }
                else
                {
                    CurrentUser.LoadFromDbByBadgeCode(userBadgeCode);
                    if ((CurrentUser.IsValid))
                    {
                        ret = UserLoginResult.Ok;
                    }
                    else
                    {
                        ret = UserLoginResult.Failed;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionMessage = ex.Message;
                ret = UserLoginResult.Failed;
            }
            return ret;
        }

        #endregion

    }
}
