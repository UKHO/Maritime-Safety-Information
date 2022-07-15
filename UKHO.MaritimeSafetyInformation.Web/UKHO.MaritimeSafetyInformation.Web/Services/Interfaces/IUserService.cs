namespace UKHO.MaritimeSafetyInformation.Web.Services.Interfaces
{
    public interface IUserService
    {
        bool IsDistributorUser { get; }
        string UserIdentifier { get; }
        string SignInName { get; }
    }
}
