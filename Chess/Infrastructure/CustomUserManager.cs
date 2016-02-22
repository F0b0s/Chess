using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Chess.Infrastructure
{
    public class CustomUserManager : UserManager<IdentityUser>
    {
        public CustomUserManager(IUserStore<IdentityUser> store) : base(store)
        {
            UserValidator = new UserValidator<IdentityUser>(this)
                            {
                                AllowOnlyAlphanumericUserNames = false,
                                RequireUniqueEmail = true
                            };
        }
    }
}