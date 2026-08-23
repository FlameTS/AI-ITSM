namespace AIITSM.Application.Common
{
    // Abstraction over "who is the logged-in employee". Incident creation
    // and "My Incidents" both need this, but M1 (Identity/Access) hasn't
    // been built yet. Application code depends on this interface only —
    // it never touches HttpContext or any auth mechanism directly, so
    // swapping in real authentication later (M1) won't require touching
    // the Incident module at all. See Web/Common/DemoCurrentUserService.cs
    // for the temporary implementation.
    public interface ICurrentUserService
    {
        int UserId { get; }

        string UserName { get; }
    }
}
