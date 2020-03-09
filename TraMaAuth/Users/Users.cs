using Cizeta.TraMaAuth.DataSets;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Cizeta.TraMaAuth
{
    public class Users : List<User>
    {
        public User this[string loginName]
        {
            get
            {
                try
                {
                    return Find(x => x.LoginName == loginName);
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public User FindByLoginName(string loginName)
        {
            return Find(x => x.LoginName == loginName);
        }

        public User FindByBadgeCode(string badgeCode)
        {
            return Find(x => x.BadgeCode == badgeCode);
        }

        public void LoadFromDb()
        {
            Clear();
            UsersDataSet.GetUsersDataTable dt;
            DataSets.UsersDataSetTableAdapters.GetUsersTableAdapter da = 
                new DataSets.UsersDataSetTableAdapters.GetUsersTableAdapter();
            dt = da.GetData();
            foreach (UsersDataSet.GetUsersRow dtr in dt.Rows)
                Add(
                    new User(
                        dtr.Id, 
                        dtr.Name, 
                        dtr.LoginName, 
                        dtr.BadgeCode,
                        (UserRole)Enum.Parse(typeof(UserRole), dtr.RoleName), 
                        dtr.AutoLogoutTime));
        }

        public static void MergeByWorkers()
        {
            try
            {
                using (DataSets.UsersDataSetTableAdapters.QueriesTableAdapter q = 
                    new DataSets.UsersDataSetTableAdapters.QueriesTableAdapter())
                {
                    q.MergeUsersByWorkers();
                }
            }
            catch (Exception ex)
            {
                throw new DbException(
                    ExceptionBuilder.ComposeMessage(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex));
            }
        }
    }
}
