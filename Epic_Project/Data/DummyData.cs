using Epic_Project.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Epic_Project.Data
{
    public class DummyData
    {
        public static async Task Initialize(ApplicationDbContext context, 
            UserManager<ApplicationUser> userManager, 
            RoleManager<ApplicationRole> roleManager)
        {
            context.Database.EnsureCreated();

            //string adminId1 = "";
            //string adminId2 = "";

            string roleProjectManager = "Project Manager";
            string descProjectManager = "Role for Project Managers";

            string roleProgramManager = "Program Manager";
            string descProgramManager = "Role for Program Managers";

            string roleTeamLeader = "Team Leader";
            string descTeamLeader = "Role for Team Leaders";

            string roleTester = "Tester";
            string descTester = "Role for Testers";

            string roleOther = "Other";
            string descOther = "Role for Others";
            
            string password = "Asd.12345";

            if (await roleManager.FindByNameAsync(roleProjectManager) == null)
            {
                await roleManager.CreateAsync(new ApplicationRole(roleProjectManager, descProjectManager, DateTime.Now));
            }
            if (await roleManager.FindByNameAsync(roleProgramManager) == null)
            {
                await roleManager.CreateAsync(new ApplicationRole(roleProgramManager, descProgramManager, DateTime.Now));
            }
            if (await roleManager.FindByNameAsync(roleTeamLeader) == null)
            {
                await roleManager.CreateAsync(new ApplicationRole(roleTeamLeader, descTeamLeader, DateTime.Now));
            }
            if (await roleManager.FindByNameAsync(roleOther) == null)
            {
                await roleManager.CreateAsync(new ApplicationRole(roleOther, descOther, DateTime.Now));
            }
            if (await roleManager.FindByNameAsync(roleTester) == null)
            {
                await roleManager.CreateAsync(new ApplicationRole(roleTester, descTester, DateTime.Now));
            }

            //if (await userManager.FindByNameAsync("admin@admin.admin") == null)
            //{
            //    var user = new ApplicationUser
            //    {
            //        UserName = "admin@admin.admin",
            //        Email = "admin@admin.admin",
            //        FirstName = "Admin Name",
            //        LastName = "Admin Lastname",
            //        EmployeeId = 1001
            //    };

            //    var result = await userManager.CreateAsync(user);
            //    if (result.Succeeded)
            //    {
            //        await userManager.AddPasswordAsync(user, password);
            //        await userManager.AddToRoleAsync(user, role1);
            //    }
            //    adminId1 = user.Id;
            //}
            //if (await userManager.FindByNameAsync("user@user.user") == null)
            //{
            //    var user = new ApplicationUser
            //    {
            //        UserName = "user@user.user",
            //        Email = "user@user.user",
            //        FirstName = "User Name",
            //        LastName = "User Lastname",
            //        EmployeeId = 1002
            //    };

            //    var result = await userManager.CreateAsync(user);
            //    if (result.Succeeded)
            //    {
            //        await userManager.AddPasswordAsync(user, password);
            //        await userManager.AddToRoleAsync(user, role2);
            //    }
            //    adminId1 = user.Id;
            //}
            if (await userManager.FindByNameAsync("test@test.test") == null)
            {
                var user = new ApplicationUser
                {
                    UserName = "test@test.test",
                    Email = "test@test.test",
                    FirstName = "Test Tester Turkey",
                    LastName = "Test Tester Turkey",
                    EmployeeId = 1003
                };

                var result = await userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    await userManager.AddPasswordAsync(user, password);
                    await userManager.AddToRoleAsync(user, "Tester");
                }
            }
            if (await userManager.FindByNameAsync("etest@etest.etest") == null)
            {
                var user = new ApplicationUser
                {
                    UserName = "etest@etest.etest",
                    Email = "etest@etest.etest",
                    FirstName = "Test Tester Egypt",
                    LastName = "Test Tester Egypt",
                    EmployeeId = 2003
                };

                var result = await userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    await userManager.AddPasswordAsync(user, password);
                    await userManager.AddToRoleAsync(user, "Tester");
                }
            }

        }
    }
}
