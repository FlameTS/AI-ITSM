using AIITSM.Application.Common;

namespace AIITSM.Web.Common
{
    // TEMPORARY stand-in for ICurrentUserService until M1 (Identity/Access)
    // ships real login. Everything downstream (IncidentService, the
    // controller, "My Incidents" filtering) already codes against
    // ICurrentUserService, so swapping this out later for a real
    // implementation (e.g. reading claims from HttpContext.User) is a
    // one-file change — nothing else needs to move.
    public class DemoCurrentUserService : ICurrentUserService
    {
        public int UserId => 1;

        public string UserName => "Demo Employee";
    }
}
