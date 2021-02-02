using System;
using System.Reflection;

namespace Cizeta.TraMaAuth
{
    public class User
    {

        #region Public

        public int Id;
        public string Name;
        public string LoginName;
        public string Password;
        public string BadgeCode;
        public UserRole Role;
        public int AutoLogoutTime;
        public string EmailAddress;
        public bool MessengerEnabled;
        public DateTime LoginDate;
        public DateTime LogoutDate;
        public bool IsLogged;

        #endregion

        #region Properties

        public bool IsValid
        {
            get { return Id > 0; }
        }

        public string LoggedName
        {
            get { return IsLogged ? Name : string.Empty; }
        }

        #endregion

        #region Events

        public event LoginDoneEventHandler LoginDone;
        public delegate void LoginDoneEventHandler(string loginName);

        public event LogoutDoneEventHandler LogoutDone;
        public delegate void LogoutDoneEventHandler(string loginName);

        #endregion

        #region Constructors

        public User() : this(0, string.Empty, string.Empty, string.Empty, UserRole.Viewer, 0, string.Empty, false) { }

        public User(int id, string emailAddress, bool messengerEnabled) :
            this(id, string.Empty, string.Empty, string.Empty, UserRole.Viewer, 0, emailAddress, messengerEnabled)
        { }

        public User(int id, string name, string loginName, string badgeCode, UserRole role,
            int autoLogoutTime, string emailAddress, bool messengerEnabled)
        {
            Id = id;
            Name = name;
            LoginName = loginName;
            Password = string.Empty;
            BadgeCode = badgeCode;
            Role = role;
            AutoLogoutTime = autoLogoutTime;
            EmailAddress = emailAddress;
            MessengerEnabled = messengerEnabled;
            IsLogged = false;
        }

        public User(string loginName)
        {
            if (!string.IsNullOrEmpty(loginName))
                LoadFromDbByLoginName(loginName);
        }

        #endregion

        #region Internal methods

        internal void LoadFromDbByLoginName(string userLoginName)
        {
            UsersDataSetTableAdapters.GetUserByLoginNameTableAdapter da = new UsersDataSetTableAdapters.GetUserByLoginNameTableAdapter();
            UsersDataSet.GetUserByLoginNameDataTable dt = da.GetData(userLoginName);
            if (dt.Rows.Count == 1)
            {
                foreach (UsersDataSet.GetUserByLoginNameRow dtr in dt)
                {
                    {
                        Id = dtr.Id;
                        Name = dtr.Name;
                        LoginName = dtr.LoginName;
                        Password = dtr.Password;
                        BadgeCode = dtr.BadgeCode;
                        AutoLogoutTime = dtr.AutoLogoutTime;
                        try
                        {
                            LoginDate = dtr.LastLoginDate;
                        }
                        catch (Exception)
                        {
                            LoginDate = DateTime.MinValue;
                        }
                        try
                        {
                            LogoutDate = dtr.LastLogoutDate;
                        }
                        catch (Exception)
                        {
                            LogoutDate = DateTime.MinValue;
                        }
                        Role = (UserRole)Enum.Parse(typeof(UserRole), dtr.RoleName);
                        IsLogged = false;
                    }
                }
            }
            else
            {
                throw new Exception(string.Format("UserNotFound"));
            }
        }

        internal void LoadFromDbByBadgeCode(string userBadgeCode)
        {
            UsersDataSetTableAdapters.GetUserByBadgeCodeTableAdapter da = new UsersDataSetTableAdapters.GetUserByBadgeCodeTableAdapter();
            UsersDataSet.GetUserByBadgeCodeDataTable dt = da.GetData(userBadgeCode);
            if (dt.Rows.Count == 1)
            {
                foreach (UsersDataSet.GetUserByBadgeCodeRow dtr in dt)
                {
                    {
                        Id = dtr.Id;
                        Name = dtr.Name;
                        LoginName = dtr.LoginName;
                        Password = dtr.Password;
                        BadgeCode = dtr.BadgeCode;
                        AutoLogoutTime = dtr.AutoLogoutTime;
                        try
                        {
                            LoginDate = dtr.LastLoginDate;
                        }
                        catch (Exception)
                        {
                            LoginDate = DateTime.MinValue;
                        }
                        try
                        {
                            LogoutDate = dtr.LastLogoutDate;
                        }
                        catch (Exception)
                        {
                            LogoutDate = DateTime.MinValue;
                        }
                        Role = (UserRole)Enum.Parse(typeof(UserRole), dtr.RoleName);
                        IsLogged = false;
                    }
                }
            }
            else
            {
                throw new Exception(string.Format("UserNotFound"));
            }
        }

        #endregion

        #region Public methods

        public bool IsAtLeast(UserRole userRole)
        {
            return Role <= userRole;
        }

        public bool IsHigherThan(UserRole userRole)
        {
            return Role < userRole;
        }

        public bool IsLowerThan(UserRole userRole)
        {
            return Role > userRole;
        }

        public bool CheckPassword(string password)
        {
            return EncodePassword(password) == Password;
        }

        public UserLoginResult Login()
        {
            LoginDate = DateTime.Now;
            IsLogged = true;
            UpdateLoginDate(Id, LoginDate);
            LoginDone?.Invoke(LoginName);
            return UserLoginResult.Failed;
        }

        public void Logout()
        {
            LogoutDate = DateTime.Now;
            IsLogged = false;
            UpdateLogoutDate(Id, LogoutDate);
            LogoutDone?.Invoke(LoginName);
        }

        #endregion

        #region Public static methods

        public static string EncodePassword(string password)
        {
            return new UserAuthenticator().EncodePassword(password);
        }

        public static string DecodePassword(string password)
        {
            return new UserAuthenticator().DecodePassword(password);
        }

        #endregion

        #region Private methods

        #endregion

        #region Static database functions

        public static bool Exists(string loginName)
        {
            bool ret = false;
            try
            {
                using (UsersDataSetTableAdapters.QueriesTableAdapter q = new UsersDataSetTableAdapters.QueriesTableAdapter())
                {
                    ret = q.UserExists(loginName) ?? false;
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

        public static int Create(string name, string loginName, string password, string badgeCode, string roleName, int autoLogoutTime, string emailAddress, bool messengerEnabled)
        {
            int ret = 0;
            try
            {
                using (UsersDataSetTableAdapters.QueriesTableAdapter q = new UsersDataSetTableAdapters.QueriesTableAdapter())
                {
                    q.CreateUser(name, loginName, password, badgeCode, roleName, autoLogoutTime, emailAddress, messengerEnabled);
                    ret = q.GetLastId("Users") ?? 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ExceptionBuilder.ComposeMessage(
                    MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex,
                    new string[] { name, loginName, password, badgeCode, roleName,
                        autoLogoutTime.ToString(), emailAddress, messengerEnabled.ToString() }));
            }
            return ret;
        }

        public static void Merge(int id, string name, string loginName, string password, string badgeCode, string roleName, int autoLogoutTime, string emailAddress, bool messengerEnabled)
        {
            try
            {
                using (UsersDataSetTableAdapters.QueriesTableAdapter q = new UsersDataSetTableAdapters.QueriesTableAdapter())
                {
                    q.MergeUser(id, name, loginName, password, badgeCode, roleName, autoLogoutTime, emailAddress, messengerEnabled);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ExceptionBuilder.ComposeMessage(
                    MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex,
                    new string[] { id.ToString(), name, loginName, password, badgeCode, roleName,
                        autoLogoutTime.ToString(), emailAddress, messengerEnabled.ToString() }));
            }
        }

        public static void MergeByWorker(int id, string name, string loginName, string password, string badgeCode, string roleName)
        {
            try
            {
                using (UsersDataSetTableAdapters.QueriesTableAdapter q = new UsersDataSetTableAdapters.QueriesTableAdapter())
                {
                    q.MergeUserByWorker(id, name, loginName, password, badgeCode, roleName);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ExceptionBuilder.ComposeMessage(
                    MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex,
                    new string[] { id.ToString(), name, loginName, password, badgeCode, roleName }));
            }
        }

        public static void Delete(string loginName)
        {
            try
            {
                using (UsersDataSetTableAdapters.QueriesTableAdapter q = new UsersDataSetTableAdapters.QueriesTableAdapter())
                {
                    q.DeleteUser(loginName);
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
            int ret;
            try
            {
                using (UsersDataSetTableAdapters.QueriesTableAdapter q = new UsersDataSetTableAdapters.QueriesTableAdapter())
                {
                    ret = q.GetUserId(loginName) ?? 0;
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
                using (UsersDataSetTableAdapters.QueriesTableAdapter q = new UsersDataSetTableAdapters.QueriesTableAdapter())
                {
                    ret = q.GetUserLoginName(id);
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
                using (UsersDataSetTableAdapters.QueriesTableAdapter q = new UsersDataSetTableAdapters.QueriesTableAdapter())
                {
                    ret = q.GetUserName(id);
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

        public static void UpdatePassword(string loginName, string password)
        {
            try
            {
                using (UsersDataSetTableAdapters.QueriesTableAdapter q = new UsersDataSetTableAdapters.QueriesTableAdapter())
                {
                    q.UpdateUserPassword(loginName, password);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ExceptionBuilder.ComposeMessage(
                    MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex,
                    new string[] { loginName }));
            }
        }

        public static void UpdateLoginDate(int id, DateTime loginDate)
        {
            try
            {
                using (UsersDataSetTableAdapters.QueriesTableAdapter q = new UsersDataSetTableAdapters.QueriesTableAdapter())
                {
                    q.UpdateUserLoginDate(id, loginDate);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ExceptionBuilder.ComposeMessage(
                    MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex,
                    new string[] { id.ToString(), loginDate.ToString() }));
            }
        }

        public static void UpdateLogoutDate(int id, DateTime logoutDate)
        {
            try
            {
                using (UsersDataSetTableAdapters.QueriesTableAdapter q = new UsersDataSetTableAdapters.QueriesTableAdapter())
                {
                    q.UpdateUserLogoutDate(id, logoutDate);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ExceptionBuilder.ComposeMessage(
                    MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex,
                    new string[] { id.ToString(), logoutDate.ToString() }));
            }
        }

        #endregion

    }
}
