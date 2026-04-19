namespace XMachine.Web.Auth;

public interface IApplicationUserSignInService
{
    Task<ApplicationUserSignInResult> SignInAsync(string username, string password, CancellationToken cancellationToken = default);
}
