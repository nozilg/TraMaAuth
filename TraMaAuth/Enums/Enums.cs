namespace Cizeta.TraMaAuth
{
    public enum UserRole
    {
        Administrator = 1,
        Maintenance = 2,
        ShiftLeader = 3,
        TeamLeader = 4,
        MachineLeader = 5,
        User = 6,
        Viewer = 7
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
