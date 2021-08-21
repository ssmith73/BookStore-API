using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore_API.Data
{
    public static class SeedData
    {
        public async static Task Seed(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            await SeedRoles(roleManager);
            await SeedUsers(userManager);

        }

        private async static Task SeedUsers(UserManager<IdentityUser> userManager)
        {

#if false
            var adminUser = userManager.Users.FirstOrDefault(x => x.UserName == "customer1@bookstore.com");
            await userManager.AddToRoleAsync(adminUser, "Customer");
            await userManager.RemoveFromRoleAsync(adminUser, "Administrator");
            //adminUser.Email = "notADuplicate@ohShit.com";
            //await userManager.RemoveLoginAsync(adminUser,adminUser.ToString(),"");
            //var testAdminUser = await userManager.FindByEmailAsync("admin@bookstore.com");
#endif

            if (await userManager.FindByEmailAsync("admin@bookstore.com") == null)
            {
                var user = new IdentityUser
                {
                    UserName = "admin",
                    Email = "admin@bookstore.com"
                };
                var result = await userManager.CreateAsync(user, "P@ssword1");
                if(result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Administrator");
                }
            }

            if (await userManager.FindByEmailAsync("customer1@bookstore.com") == null)
            {
                var user = new IdentityUser
                {
                    UserName = "customer1",
                    Email = "customer1@bookstore.com"
                };
                var result = await userManager.CreateAsync(user, "P@ssword1");
                if(result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Customer");
                }
            }
            if (await userManager.FindByEmailAsync("customer3@bookstore.com") == null)
            {
                var user = new IdentityUser
                {
                    UserName = "customer3",
                    Email = "customer3@bookstore.com"
                };
                var result = await userManager.CreateAsync(user, "P@ssword1");
                if(result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Customer");
                }
            }

            if (await userManager.FindByEmailAsync("customer2@gmail.com") == null)
            {
                var user = new IdentityUser
                {
                    UserName = "customer2",
                    Email = "customer2@bookstore.com"
                };
                var result = await userManager.CreateAsync(user, "P@ssword1");
                if(result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Customer");
                }
            }
        }

        private async static Task  SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if(await roleManager.RoleExistsAsync("Administrator") == false)
            {
                var role = new IdentityRole
                {
                    Name = "Administrator"
                };

                await roleManager.CreateAsync(role);
            }

            if(await roleManager.RoleExistsAsync("Customer") == false)
            {
                //create object
                var role = new IdentityRole
                {
                    Name = "Customer"
                };

                //create entry into database
                await roleManager.CreateAsync(role);
            }
        }
    }
}
