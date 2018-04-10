using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cizeta.TraMaAuth
{

    public enum UserRole
    {
        Administrator = 1,
        TeamLeader = 2,
        Maintenance = 3,
        User = 4,
        Viewer = 5
    }

    public enum AuthenticationMode
    {
        Any,
        UserPassword,
        BadgeCode
    }

    public enum UserLoginResult
    {
        Ok,
        Failed
    }

    public enum WorkerLoginResult
    {
        Ok,
        Failed,
        NotEnabled
    }

}
