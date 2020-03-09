using Cizeta.TraMaAuth.DataSets;
using System;

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
        public DateTime LoginDate;
        public DateTime LogoutDate;
        public bool IsLogged;

        #endregion

        #region Properties

        public bool IsValid
        {
            get { return (Id == 0 ? false : true); }
        }

        #endregion

        #region Constructors

        internal User() : this(0, string.Empty, string.Empty, string.Empty, UserRole.Viewer, 0) { }

        internal User(int id, string name, string loginName, string badgeCode, UserRole role, int autoLogoutTime)
        {
            Id = id;
            Name = name;
            LoginName = loginName;
            Password = string.Empty;
            BadgeCode = badgeCode;
            Role = role;
            AutoLogoutTime = autoLogoutTime;
            IsLogged = false;
        }

        #endregion

        #region Internal methods

        internal void LoadFromDbByLoginName(string userLoginName)
        {
            DataSets.UsersDataSetTableAdapters.GetUserByLoginNameTableAdapter da = new DataSets.UsersDataSetTableAdapters.GetUserByLoginNameTableAdapter();
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
            DataSets.UsersDataSetTableAdapters.GetUserByBadgeCodeTableAdapter da = new DataSets.UsersDataSetTableAdapters.GetUserByBadgeCodeTableAdapter();
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
            return (Role <= userRole ? true : false);
        }

        public bool IsHigherThan(UserRole userRole)
        {
            return (Role < userRole ? true : false);
        }

        public bool IsLowerThan(UserRole userRole)
        {
            return (Role > userRole ? true : false);
        }

        #endregion

    }
}
