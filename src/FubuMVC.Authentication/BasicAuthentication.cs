using System.Security.Principal;
using FubuCore;

namespace FubuMVC.Authentication
{
    public class BasicAuthentication : IAuthenticationStrategy
    {
        private readonly IAuthenticationSession _session;
        private readonly IPrincipalContext _context;
        private readonly IPrincipalBuilder _builder;
        private readonly ICredentialsAuthenticator _authenticator;
        private readonly ILockedOutRule _lockedOutRule;

        public BasicAuthentication(IAuthenticationSession session, IPrincipalContext context, IPrincipalBuilder builder, ICredentialsAuthenticator authenticator, ILockedOutRule lockedOutRule)
        {
            _session = session;
            _context = context;
            _builder = builder;
            _authenticator = authenticator;
            _lockedOutRule = lockedOutRule;
        }

        public bool TryToApply()
        {
            string userName = _session.PreviouslyAuthenticatedUser();
            if (userName.IsNotEmpty())
            {
                _session.MarkAccessed();
                IPrincipal principal = _builder.Build(userName);
                _context.Current = principal;

                return true;
            }

            return false;
        }

        public bool Authenticate(LoginRequest request)
        {
            if (_lockedOutRule.IsLockedOut(request) == LoginStatus.LockedOut)
            {
                request.Status = LoginStatus.LockedOut;
            }
            else if (_authenticator.AuthenticateCredentials(request))
            {
                request.Status = LoginStatus.Succeeded;
                _session.MarkAuthenticated(request.UserName);
            }
            else
            {
                request.Status = LoginStatus.Failed;
                request.NumberOfTries++;

                _lockedOutRule.ProcessFailure(request);
            }

            return request.Status == LoginStatus.Succeeded;
        }
    }
}