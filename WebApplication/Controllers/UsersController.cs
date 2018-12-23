using System;
using System.Threading.Tasks;
using Common.Extensions;
using Common.Models;
using Common.Models.Entities;
using DataLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;
using WebCommon.BaseControllers;

namespace WebApplication.Controllers
{
    /// <summary>
    /// Users controller.
    /// </summary>
    public class UsersController : AuthController
    {
        private readonly IUserManager userManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:WebApplication.Controllers.UsersController"/> class.
        /// </summary>
        /// <param name="userManager">User manager.</param>
        public UsersController(IUserManager userManager)
        {
            this.userManager = userManager;
        }


        /// <summary>
        /// Get this instance.
        /// </summary>
        /// <returns>The get.</returns>
        [HttpGet]
        public Task<User> Get()
        {
            return userManager.GetUser(Token.UserID);
        }

        /// <summary>
        /// Get this instance.
        /// </summary>
        /// <returns>The get.</returns>
        [HttpGet("/api/Users/Badges")]
        public async Task<GenericCollection<UserBadge>> GetBadges()
        {
            var list = await userManager.GetUserBadges(Token.UserID).ConfigureAwait(false);

            return list.ToCollection();
        }
    }
}
