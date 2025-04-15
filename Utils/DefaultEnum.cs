namespace Project.Utils
{
    public enum UserType
    {
        Manager,
        Lecturer,
    }

    public enum LoginType
    {
        Google,
        Standard,
    }

    public enum UsernameType
    {
        Email,
        PhoneNumber,
        UserName,
    }

    public enum ItemStatus
    {
        Pending,
        Approved,
        Rejected,
        Borrowing,
        Returned,
        Overdue,
    }
}
