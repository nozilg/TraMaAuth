﻿using System;
using System.Collections.Generic;

namespace Cizeta.TraMaAuth
{
    public class Roles : List<Role>
    {
        public Role this[string name]
        {
            get
            {
                try
                {
                    return Find(x => x.Name == name);
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public void LoadFromDb()
        {
            Clear();
            WorkersDataSet.GetRolesDataTable dt;
            WorkersDataSetTableAdapters.GetRolesTableAdapter da = new WorkersDataSetTableAdapters.GetRolesTableAdapter();
            dt = da.GetData();
            foreach (WorkersDataSet.GetRolesRow dtr in dt.Rows)
                Add(new Role(dtr.Id, dtr.Name, dtr.Description, dtr.AccessLevel));
        }
    }
}
