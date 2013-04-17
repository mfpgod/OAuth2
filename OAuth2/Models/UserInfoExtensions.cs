using OAuth2.Infrastructure;

namespace OAuth2.Models
{
    public static class UserInfoExtensions
    {
        public static void FillNamesFromString(this UserInfo user, string name)
        {
            if (!name.IsEmpty())
            {
                var index = name.IndexOf(' ');
                if (index != -1)
                {
                    user.FirstName = name.Substring(0, index);
                    user.LastName = name.Substring(index + 1);
                }
                else
                {
                    user.FirstName = name;
                }
            }
        }
    }
}