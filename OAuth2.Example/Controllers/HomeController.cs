using System;
using System.Linq;
using System.Web.Mvc;
using OAuth2.Client;
using OAuth2.Example.Models;

namespace OAuth2.Example.Controllers
{
    /// <summary>
    /// The only controller in this example app.
    /// </summary>
    public class HomeController : Controller
    {
        private readonly AuthorizationRoot authorizationRoot;

        private const string ProviderNameKey = "providerName";
        private const string ClientKey = "client";

        private const string TokenKey = "token;

        private string ProviderName
        {
            get { return (string)Session[ProviderNameKey]; }
            set { Session[ProviderNameKey] = value; }
        }

        private IClient Client
        {
            get { return (ICliSession[ProviderNameKey + ClientKey] as IClient; }
            set { Session[ProviderNameKey + ClientKey] = value; }
        }

        private OauthAccessToken AccessToken
        {
            get { return Session[ProviderNameKey + ClientKey + TokenKey] as OauthAccessToken; }
            set { Session[ProviderNameKey + ClientKey + Token value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController"/> class.
        /// </summary>
        /// <param name="authorizationRoot">The authorization manager.</param>
        public HomeController(AuthorizationRoot authorizationRoot)
        {
            this.authorizationRoot = authorizationRoot;
        }

        /// <summary>
        /// Renders home page with login link.
        /// </summary>
        public ActionResult Index()
        {
            var model = authorizationRoot.Clients.Select(client => new LoginInfoModel
                {
                    ProviderName = client.Name
                });
            return View(model);
        }

        /// <summary>
        /// Redirect to login url of selected provider.
        /// </summary>        
        public RedirectResult Login(string providerName)
        {
            ProviderName = providerName;
            Client = GetClient();
       

            return new RedirectResult(Client.GetLoginLinkUri(Guid.NewGuid().ToString()));
        }

        /// <summary>
        /// Renders information received from authentication service.
        /// </summary>
        public ActionResult User()
        {
            if (Client != null && AccessToken != null)
            {
                var userInfo = Client.GetUserInfo(AccessToken);
                return View(userInfo);
            }

            return RedirectToAction("Index"); }

        /// <summary>
        /// Renders information received from authentication service.
        /// </summary>
        public ActionResult Auth()
        {
            if (Client != null)
            {
                var accessToAccessToken = Client.Finalize(Request.QueryString);
                return RedirectToAction("User");
            }

            return RedirectToAction("Index"     private IClient GetClient()
        {
            return authorizationRoot.Clients.First(c => c.Name == ProviderName);
        }
    }
}