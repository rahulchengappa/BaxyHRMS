namespace HRMS.Application.Interfaces.Security
{
    public interface IIdProtector
    {
        string Protect(string value);
        string Unprotect(string value);
    }
}
