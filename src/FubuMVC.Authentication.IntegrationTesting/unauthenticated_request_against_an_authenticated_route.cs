﻿using System.Net;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Authentication.IntegrationTesting
{
    [TestFixture]
    public class unauthenticated_request_against_an_authenticated_route : AuthenticationHarness
    {
        [Test]
        public void redirects_to_login()
        {
            var response = endpoints.GetByInput(new TargetModel(), acceptType: "text/html", configure: r => r.AllowAutoRedirect = false);
            response.StatusCode.ShouldEqual(HttpStatusCode.Redirect);

            var loginUrl = Urls.UrlFor(new LoginRequest { Url = "/some/authenticated/route"}, "GET");
            response.ResponseHeaderFor(HttpResponseHeader.Location).ShouldEqual(loginUrl);
        }
    }
}