using Cizeta.TraMaAuth.DataSets;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cizeta.TraMaAuth
{
    public class Workers : List<Worker>
    {
        public Worker this[string loginName]
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

        public Worker FindByBadgeCode(string badgeCode)
        {
            return Find(x => x.BadgeCode == badgeCode);
        }

        public void LoadFromDb()
        {
            Clear();
            WorkersDataSet.GetWorkersDataTable dt;
            DataSets.WorkersDataSetTableAdapters.GetWorkersTableAdapter da = 
                new DataSets.WorkersDataSetTableAdapters.GetWorkersTableAdapter();
            dt = da.GetData();
            foreach (WorkersDataSet.GetWorkersRow dtr in dt.Rows)
            {
                Add(
                    new Worker(
                        dtr.Id, 
                        dtr.Name, 
                        dtr.LoginName, 
                        dtr.BadgeCode, 
                        dtr.Code, 
                        dtr.RoleName,
                        dtr.AccessLevel));
                this.Last().LoadStationsConfigFromDb(dtr.LoginName);
            }
        }
    }
}
