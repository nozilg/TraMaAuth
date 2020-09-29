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

        public Role() : this(0, string.Empty, string.Empty, 0) { }

        public Role(string name) : this()
        {
            LoadFromDb(name);
        }

        public Role(int id, string name, string description, int accessLevel)
        {
            Id = id;
            Name = name;
            Description = description;
            AccessLevel = accessLevel;
        }

        public void LoadFromDb(string name)
        {
            try
            {
                WorkersDataSet.GetRoleByNameDataTable dt;
                WorkersDataSetTableAdapters.GetRoleByNameTableAdapter da =
                    new WorkersDataSetTableAdapters.GetRoleByNameTableAdapter();
                dt = da.GetData(name);
                if (dt.Rows.Count == 1)
                {
                    foreach (WorkersDataSet.GetRoleByNameRow dtr in dt)
                    {
                        Id = dtr.Id;
                        Name = dtr.Name;
                        Description = dtr.Description;
                        AccessLevel = dtr.AccessLevel;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ExceptionBuilder.ComposeMessage(
                    MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex,
                    new string[] { name }));
            }
        }

        public static bool Exists(string name)
        {
            bool ret = false;
            try
            {
                using (WorkersDataSetTableAdapters.QueriesTableAdapter q = new WorkersDataSetTableAdapters.QueriesTableAdapter())
                {
                    ret = q.RoleExists(name) ?? false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ExceptionBuilder.ComposeMessage(
                    MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex,
                    new string[] { name }));
            }
            return ret;
        }

        public static int Create(string name, string description, int accessLevel)
        {
            int ret = 0;
            try
            {
                using (WorkersDataSetTableAdapters.QueriesTableAdapter q = new WorkersDataSetTableAdapters.QueriesTableAdapter())
                {
                    q.CreateRole(name, description, accessLevel);
                    ret = q.GetLastId("Roles") ?? 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ExceptionBuilder.ComposeMessage(
                    MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex,
                    new string[] { name, description, accessLevel.ToString() }));
            }
            return ret;
        }

        public static void Merge(int id, string name, string description, int accessLevel)
        {
            try
            {
                using (WorkersDataSetTableAdapters.QueriesTableAdapter q = new WorkersDataSetTableAdapters.QueriesTableAdapter())
                {
                    q.MergeRole(id, name, description, accessLevel);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ExceptionBuilder.ComposeMessage(
                    MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex,
                    new string[] { id.ToString(), name, description, accessLevel.ToString() }));
            }
        }

        public static void Delete(string name)
        {
            try
            {
                using (WorkersDataSetTableAdapters.QueriesTableAdapter q = new WorkersDataSetTableAdapters.QueriesTableAdapter())
                {
                    q.DeleteRole(name);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ExceptionBuilder.ComposeMessage(
                    MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex,
                    new string[] { name }));
            }
        }
    }
}
