using Cizeta.TraMaAuth.DataSets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            get { return (this.Id == 0 ? false : true); }
        }

        #endregion

        #region Constructors

        internal User() : this(0, string.Empty, string.Empty, string.Empty, UserRole.Viewer, 0)
        {

        }

        internal User(int userId, string userName, string userLoginName, string userBadgeCode, UserRole userRole, int userAutoLogoutTime)
        {
            this.Id = userId;
            this.Name = userName;
            this.LoginName = userLoginName;
            this.Password = string.Empty;
            this.BadgeCode = userBadgeCode;
            this.Role = userRole;
            this.AutoLogoutTime = userAutoLogoutTime;
            this.IsLogged = false;
        }

        #endregion

        #region Private methods

        internal void LoadFromDbByLoginName(string userLoginName)
        {
            UsersDataSet.GetUserByLoginNameDataTable dt = default(UsersDataSet.GetUserByLoginNameDataTable);
            DataSets.UsersDataSetTableAdapters.GetUserByLoginNameTableAdapter da = new DataSets.UsersDataSetTableAdapters.GetUserByLoginNameTableAdapter();
            dt = da.GetData(userLoginName);
            if (dt.Rows.Count == 1)
            {
                foreach (UsersDataSet.GetUserByLoginNameRow dtr in dt)
                {
                    {
                        this.Id = dtr.Id;
                        this.Name = dtr.Name;
                        this.LoginName = dtr.LoginName;
                        this.Password = dtr.Password;
                        this.BadgeCode = dtr.BadgeCode;
                        this.AutoLogoutTime = dtr.AutoLogoutTime;
                        try
                        {
                            this.LoginDate = dtr.LastLoginDate;
                        }
                        catch (Exception)
                        {
                            this.LoginDate = System.DateTime.MinValue;
                        }
                        try
                        {
                            this.LogoutDate = dtr.LastLogoutDate;
                        }
                        catch (Exception)
                        {
                            this.LogoutDate = System.DateTime.MinValue;
                        }
                        this.Role = (UserRole)Enum.Parse(typeof(UserRole), dtr.RoleName);
                        this.IsLogged = false;
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
            UsersDataSet.GetUserByBadgeCodeDataTable dt = default(UsersDataSet.GetUserByBadgeCodeDataTable);
            DataSets.UsersDataSetTableAdapters.GetUserByBadgeCodeTableAdapter da = new DataSets.UsersDataSetTableAdapters.GetUserByBadgeCodeTableAdapter();
            dt = da.GetData(userBadgeCode);
            if (dt.Rows.Count == 1)
            {
                foreach (UsersDataSet.GetUserByBadgeCodeRow dtr in dt)
                {
                    {
                        this.Id = dtr.Id;
                        this.Name = dtr.Name;
                        this.LoginName = dtr.LoginName;
                        this.Password = dtr.Password;
                        this.BadgeCode = dtr.BadgeCode;
                        this.AutoLogoutTime = dtr.AutoLogoutTime;
                        try
                        {
                            this.LoginDate = dtr.LastLoginDate;
                        }
                        catch (Exception)
                        {
                            this.LoginDate = System.DateTime.MinValue;
                        }
                        try
                        {
                            this.LogoutDate = dtr.LastLogoutDate;
                        }
                        catch (Exception)
                        {
                            this.LogoutDate = System.DateTime.MinValue;
                        }
                        this.Role = (UserRole)Enum.Parse(typeof(UserRole), dtr.RoleName);
                        this.IsLogged = false;
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
