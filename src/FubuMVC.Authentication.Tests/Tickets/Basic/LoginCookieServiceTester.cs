﻿using System;
using System.Web;
using FubuCore.Dates;
using FubuMVC.Authentication.Tickets.Basic;
using FubuMVC.Core.Http;
using FubuMVC.Core.Runtime;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Authentication.Tests.Tickets.Basic
{
    [TestFixture]
    public class LoginCookieServiceTester : InteractionContext<LoginCookieService>
    {
        private CookieSettings theSettings;
        private ISystemTime theSystemTime;
        private DateTime today;

        protected override void beforeEach()
        {
            theSettings = new CookieSettings();
            theSystemTime = MockRepository.GenerateStub<ISystemTime>();

            today = DateTime.Today;
            theSystemTime.Stub(x => x.UtcNow()).Return(today);

            Services.Inject(theSettings);
        }

        private HttpCookie theCookie { get { return ClassUnderTest.CreateCookie(theSystemTime); } }

        [Test]
        public void sets_the_name_from_the_settings()
        {
            theCookie.Name.ShouldEqual(theSettings.Name);
        }

        [Test]
        public void sets_the_domain_if_specified_in_the_settings()
        {
            theSettings.Domain = "test.com";
            theCookie.Domain.ShouldEqual(theSettings.Domain);
        }

        [Test]
        public void sets_the_path_if_specified_in_the_settings()
        {
            theSettings.Path = "/test";
            theCookie.Path.ShouldEqual(theSettings.Path);
        }

        [Test]
        public void does_not_set_the_path_if_not_specified_in_the_settings()
        {
            theSettings.Path = null;
            theCookie.Path.ShouldNotBeNull();
        }

        [Test]
        public void sets_the_secure_flag_from_the_settings()
        {
            theSettings.Secure = false;
            theCookie.Secure.ShouldBeFalse();

            theSettings.Secure = true;
            theCookie.Secure.ShouldBeTrue();
        }

        [Test]
        public void sets_the_httponly_flag_from_the_settings()
        {
            theSettings.HttpOnly = false;
            theCookie.HttpOnly.ShouldBeFalse();

            theSettings.HttpOnly = true;
            theCookie.HttpOnly.ShouldBeTrue();
        }

        [Test]
        public void sets_the_expiration_date()
        {
            theCookie.Expires.ShouldEqual(theSettings.ExpirationFor(today));
        }
    }

    [TestFixture]
    public class when_getting_the_current_cookie : InteractionContext<LoginCookieService>
    {
        private HttpCookie theCookie;

        protected override void beforeEach()
        {
            theCookie = new HttpCookie(CookieSettings.DefaultCookieName);

            MockFor<ICookies>().Stub(x => x.Get(CookieSettings.DefaultCookieName)).Return(theCookie);
        }

        [Test]
        public void finds_the_cookie_by_name_from_the_settings()
        {
            ClassUnderTest.Current().ShouldBeTheSameAs(theCookie);
        }
    }

    [TestFixture]
    public class when_updating_the_login_cookie : InteractionContext<LoginCookieService>
    {
        private HttpCookie theCookie;

        protected override void beforeEach()
        {
            theCookie = new HttpCookie("Test");
            ClassUnderTest.Update(theCookie);
        }

        [Test]
        public void adds_the_cookie_to_the_response()
        {
            MockFor<IOutputWriter>().AssertWasCalled(x => x.AppendCookie(theCookie));
        }
    }
}