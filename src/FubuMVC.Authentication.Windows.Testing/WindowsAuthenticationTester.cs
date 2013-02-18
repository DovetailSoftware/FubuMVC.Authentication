﻿using System.Security.Principal;
using FubuMVC.Core.Continuations;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Authentication.Windows.Testing
{
    [TestFixture]
    public class when_the_windows_authentication_is_successful : InteractionContext<WindowsAuthentication>
    {
        private FubuContinuation theContinuation;
        private WindowsPrincipal thePrincipal;
        private WindowsSignInRequest theRequest;

        protected override void beforeEach()
        {
            thePrincipal = new WindowsPrincipal(WindowsIdentity.GetAnonymous());
            theRequest = new WindowsSignInRequest();

            MockFor<IWindowsPrincipalHandler>().Stub(x => x.Authenticated(thePrincipal))
                                               .Return(true);

            theContinuation = ClassUnderTest.Authenticate(theRequest, thePrincipal);
        }

        [Test]
        public void should_be_auditing_the_success()
        {
            MockFor<ILoginAuditor>().AssertWasCalled(x => x.Audit(new SuccessfulWindowsAuthentication{User = thePrincipal.Identity.Name}));
        }

        [Test]
        public void invokes_the_principal_handler()
        {
            MockFor<IWindowsPrincipalHandler>().AssertWasCalled(x => x.Authenticated(thePrincipal));
        }

        [Test]
        public void redirects_the_redirect_url()
        {
            theContinuation.AssertWasRedirectedTo(theRequest.Url);
        }

        [Test]
        public void marks_the_session_as_authenticated()
        {
            MockFor<IAuthenticationSession>().AssertWasCalled(x => x.MarkAuthenticated(thePrincipal.Identity.Name));
        }
    }


    [TestFixture]
    public class when_the_windows_authentication_fails : InteractionContext<WindowsAuthentication>
    {
        private FubuContinuation theContinuation;
        private WindowsPrincipal thePrincipal;
        private WindowsSignInRequest theRequest;

        protected override void beforeEach()
        {
            thePrincipal = new WindowsPrincipal(WindowsIdentity.GetAnonymous());
            theRequest = new WindowsSignInRequest
            {
                Url = "some url"
            };

            MockFor<IWindowsPrincipalHandler>().Stub(x => x.Authenticated(thePrincipal))
                                               .Return(false);

            theContinuation = ClassUnderTest.Authenticate(theRequest, thePrincipal);
        }

        [Test]
        public void should_be_auditing_the_failure()
        {
            MockFor<ILoginAuditor>().AssertWasCalled(x => x.Audit(new FailedWindowsAuthentication { User = thePrincipal.Identity.Name }));
        }

        [Test]
        public void invokes_the_principal_handler()
        {
            MockFor<IWindowsPrincipalHandler>().AssertWasCalled(x => x.Authenticated(thePrincipal));
        }

        [Test]
        public void should_redirect_to_the_login_page()
        {
            theContinuation.AssertWasRedirectedTo(new LoginRequest{Url = theRequest.Url});
        }

        [Test]
        public void DOES_NOT_marks_the_session_as_authenticated()
        {
            MockFor<IAuthenticationSession>().AssertWasNotCalled(x => x.MarkAuthenticated(thePrincipal.Identity.Name));
        }
    }



}