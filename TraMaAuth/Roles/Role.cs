using Cizeta.TraMaAuth.DataSets;
using System;
using System.Reflection;

namespace Cizeta.TraMaAuth
{
    public class Role
    {
        public int Id;
        public string Name;
        public string Description;
        public int AccessLevel;
        public bool CanRepair;
        //public UserRole UserRole;

        public Role() : this(0, string.Empty, string.Empty, 0, false) { }

        public Role(string name)
        {
            LoadFromDb(name);
        }

        public Role(int id, string name, string description, int accessLevel, bool canRepair)
        {
            Id = id;
            Name = name;
            Description = description;
            AccessLevel = accessLevel;
            CanRepair = canRepair;
        }

        public void LoadFromDb(string name)
        {
            WorkersDataSet.GetRoleByNameDataTable dt;
            DataSets.WorkersDataSetTableAdapters.GetRoleByNameTableAdapter da = new DataSets.WorkersDataSetTableAdapters.GetRoleByNameTableAdapter();
            dt = da.GetData(name);
            if (dt.Rows.Count == 1)
            {
                foreach (WorkersDataSet.GetRoleByNameRow dtr in dt)
                {
                    Id = dtr.Id;
                    Name = dtr.Name;
                    Description = dtr.Description;
                    CanRepair = dtr.CanRepair;
                    AccessLevel = dtr.AccessLevel;
                }
            }
            else
                throw new Exception(string.Format("RoleNotFound"));
        }

        public static bool Exists(string name)
        {
            bool ret = false;
            try
            {
                using (DataSets.WorkersDataSetTableAdapters.QueriesTableAdapter q = new DataSets.WorkersDataSetTableAdapters.QueriesTableAdapter())
                {
                    ret = q.RoleExists(name) ?? false;
                }
            }
            catch (Exception ex)
            {
                throw new DbException(
                    ExceptionBuilder.ComposeMessage(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex,
                        new string[] { name }));
            }
            return ret;
        }

        public static int Create(string name, string description, int accessLevel)
        {
            int ret = 0;
            try
            {
                using (DataSets.WorkersDataSetTableAdapters.QueriesTableAdapter q = new DataSets.WorkersDataSetTableAdapters.QueriesTableAdapter())
                {
                    q.CreateRole(name, description, accessLevel);
                    ret = q.GetLastId("Roles") ?? 0;
                }
            }
            catch (Exception ex)
            {
                throw new DbException(
                    ExceptionBuilder.ComposeMessage(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex,
                        new string[] { name, description, accessLevel.ToString() })) ;
            }
            return ret;
        }

        public static void Merge(int id, string name, string description, int accessLevel)
        {
            try
            {
                using (DataSets.WorkersDataSetTableAdapters.QueriesTableAdapter q = new DataSets.WorkersDataSetTableAdapters.QueriesTableAdapter())
                {
                    q.MergeRole(id, name, description, accessLevel);
                }
            }
            catch (Exception ex)
            {
                throw new DbException(
                    ExceptionBuilder.ComposeMessage(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex,
                        new string[] { id.ToString(), name, description, accessLevel.ToString() }));
            }
        }

        public static void Delete(string name)
        {
            try
            {
                using (DataSets.WorkersDataSetTableAdapters.QueriesTableAdapter q = new DataSets.WorkersDataSetTableAdapters.QueriesTableAdapter())
                {
                    q.DeleteRole(name);
                }
            }
            catch (Exception ex)
            {
                throw new DbException(
                    ExceptionBuilder.ComposeMessage(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex,
                        new string[] { name }));
            }
        }
    }
}
