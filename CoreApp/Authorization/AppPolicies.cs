namespace CoreApp.Authorization;

public enum AppPolicies
{
    Administrator,
    AdminOnly,
    ActiveUser,
    SalesDepartment,
    Lecturer,
    DeanOffice,
    LecturerOrDeanOffice
}

public static class AppPoliciesExtensions
{
    public static string Name(this AppPolicies policy)
    {
        return policy.ToString();
    }
}

