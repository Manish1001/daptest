namespace CMS.Shared.Library
{
    public enum AccountStatus
    {
        ServerError = -1,
        Success,
        InvalidUsername,
        InvalidPassword,
        PasswordExpired,
        InactiveUser,
        LockedUser
    }
}