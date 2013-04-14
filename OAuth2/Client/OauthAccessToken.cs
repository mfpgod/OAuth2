namespace OAuth2.Client
{
    public abstract class OauthAccessToken
    {
        protected OauthAccessToken(string token = null)
        {
            Token = token;
        }

        public string Token { get; set; } 
    }
}