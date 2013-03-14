using System.Net;
using FubuMVC.Core;
using FubuMVC.Core.Ajax;
using FubuMVC.Core.Endpoints;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Authentication.IntegrationTesting
{
    [TestFixture]
    public class unauthenticated_ajax_request_against_an_authenticated_route : AuthenticationHarness
    {
        private HttpResponse theResponse;

        protected override void beforeEach()
        {
            theResponse = endpoints.GetByInput(new TargetModel(), acceptType: "application/json", configure: r =>
            {
                r.AllowAutoRedirect = false;
                r.Headers.Add(AjaxExtensions.XRequestedWithHeader, AjaxExtensions.XmlHttpRequestValue);
            });
        }

        [Test]
        public void unauthorized_status_code()
        {
            theResponse.StatusCode.ShouldEqual(HttpStatusCode.Unauthorized);
        }

        [Test]
        public void writes_the_navigate_continuation()
        {
            var continuation = theResponse.ReadAsJson<AjaxContinuation>();

            continuation.Success.ShouldBeFalse();
            continuation.NavigatePage.ShouldEqual(Urls.UrlFor(new LoginRequest
            {
                Url = null
            }, "GET"));
        }
    }
}