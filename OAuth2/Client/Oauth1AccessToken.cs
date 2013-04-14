namespace OAuth2.Client
{
    public class Oauth1AccessToken : OauthAccessToken
    {
        public Oauth1AccessToken(string token = null, string tokenSecret = null) : base(token)
        {
            TokenSecret = tokenSecret;
        }

        public string TokenSecret { get; set; } 
    }
}