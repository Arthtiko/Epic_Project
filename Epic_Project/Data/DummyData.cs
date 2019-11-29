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
            if (await userManager.FindByNameAsync("Nashwa.Rashed@its.ws") == null)
            {
                var user = new ApplicationUser
                {
                    UserName = "Nashwa.Rashed@its.ws",
                    Email = "Nashwa.Rashed@its.ws",
                    FirstName = "Nashwa Tawfik Nasr Rashed",
                    LastName = "Nashwa Tawfik Nasr Rashed",
                    EmployeeId = 4
                };

                var result = await userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    await userManager.AddPasswordAsync(user, password);
                    await userManager.AddToRoleAsync(user, "Team Leader");
                }
            }
            if (await userManager.FindByNameAsync("moataz.mohamed@its.ws") == null)
            {
                var user = new ApplicationUser
                {
                    UserName = "moataz.mohamed@its.ws",
                    Email = "moataz.mohamed@its.ws",
                    FirstName = "Moataz Mohamed Anwar Gaafar",
                    LastName = "Moataz Mohamed Anwar Gaafar",
                    EmployeeId = 2
                };

                var result = await userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    await userManager.AddPasswordAsync(user, password);
                    await userManager.AddToRoleAsync(user, "Team Leader");
                }
            }
            if (await userManager.FindByNameAsync("hisham.nassef@its.ws") == null)
            {
                var user = new ApplicationUser
                {
                    UserName = "hisham.nassef@its.ws",
                    Email = "hisham.nassef@its.ws",
                    FirstName = "Hisham Nassef",
                    LastName = "Hisham Nassef",
                    EmployeeId = 28
                };

                var result = await userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    await userManager.AddPasswordAsync(user, password);
                    await userManager.AddToRoleAsync(user, "Team Leader");
                }
            }
            if (await userManager.FindByNameAsync("mohamed.konswa@its.ws") == null)
            {
                var user = new ApplicationUser
                {
                    UserName = "mohamed.konswa@its.ws",
                    Email = "mohamed.konswa@its.ws",
                    FirstName = "Mohamed Konswa",
                    LastName = "Mohamed Konswa",
                    EmployeeId = 29
                };

                var result = await userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    await userManager.AddPasswordAsync(user, password);
                    await userManager.AddToRoleAsync(user, "Team Leader");
                }
            }
            if (await userManager.FindByNameAsync("Sayed.Hussein@its.ws") == null)
            {
                var user = new ApplicationUser
                {
                    UserName = "Sayed.Hussein@its.ws",
                    Email = "Sayed.Hussein@its.ws",
                    FirstName = "Sayed Hussein Sayed",
                    LastName = "Sayed Hussein Sayed",
                    EmployeeId = 30
                };

                var result = await userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    await userManager.AddPasswordAsync(user, password);
                    await userManager.AddToRoleAsync(user, "Team Leader");
                }
            }
            if (await userManager.FindByNameAsync("Fayza.Mahfouz@its.ws") == null)
            {
                var user = new ApplicationUser
                {
                    UserName = "Fayza.Mahfouz@its.ws",
                    Email = "Fayza.Mahfouz@its.ws",
                    FirstName = "Fayza Ahmed Mahfouz",
                    LastName = "Fayza Ahmed Mahfouz",
                    EmployeeId = 31
                };

                var result = await userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    await userManager.AddPasswordAsync(user, password);
                    await userManager.AddToRoleAsync(user, "Team Leader");
                }
            }
            if (await userManager.FindByNameAsync("may.hassan@its.ws") == null)
            {
                var user = new ApplicationUser
                {
                    UserName = "may.hassan@its.ws",
                    Email = "may.hassan@its.ws",
                    FirstName = "May Hassan",
                    LastName = "May Hassan",
                    EmployeeId = 32
                };

                var result = await userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    await userManager.AddPasswordAsync(user, password);
                    await userManager.AddToRoleAsync(user, "Team Leader");
                }
            }
            if (await userManager.FindByNameAsync("mohammad.alsayed@its.ws") == null)
            {
                var user = new ApplicationUser
                {
                    UserName = "mohammad.alsayed@its.ws",
                    Email = "mohammad.alsayed@its.ws",
                    FirstName = "Mohammad AlSayed",
                    LastName = "Mohammad AlSayed",
                    EmployeeId = 33
                };

                var result = await userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    await userManager.AddPasswordAsync(user, password);
                    await userManager.AddToRoleAsync(user, "Team Leader");
                }
            }
            if (await userManager.FindByNameAsync("ahmed.elsalakawy@its.ws") == null)
            {
                var user = new ApplicationUser
                {
                    UserName = "ahmed.elsalakawy@its.ws",
                    Email = "ahmed.elsalakawy@its.ws",
                    FirstName = "Ahmed Salakawy",
                    LastName = "Ahmed Salakawy",
                    EmployeeId = 34
                };

                var result = await userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    await userManager.AddPasswordAsync(user, password);
                    await userManager.AddToRoleAsync(user, "Team Leader");
                }
            }
            if (await userManager.FindByNameAsync("Wessam.tohamy@its.ws") == null)
            {
                var user = new ApplicationUser
                {
                    UserName = "Wessam.tohamy@its.ws",
                    Email = "Wessam.tohamy@its.ws",
                    FirstName = "Wessam Salah",
                    LastName = "Wessam Salah",
                    EmployeeId = 35
                };

                var result = await userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    await userManager.AddPasswordAsync(user, password);
                    await userManager.AddToRoleAsync(user, "Team Leader");
                }
            }
            if (await userManager.FindByNameAsync("rafeek.inani@its.ws") == null)
            {
                var user = new ApplicationUser
                {
                    UserName = "rafeek.inani@its.ws",
                    Email = "rafeek.inani@its.ws",
                    FirstName = "Rafeek Inani",
                    LastName = "Rafeek Inani",
                    EmployeeId = 36
                };

                var result = await userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    await userManager.AddPasswordAsync(user, password);
                    await userManager.AddToRoleAsync(user, "Team Leader");
                }
            }
            if (await userManager.FindByNameAsync("Mohamed.elkhattab@its.ws") == null)
            {
                var user = new ApplicationUser
                {
                    UserName = "Mohamed.elkhattab@its.ws",
                    Email = "Mohamed.elkhattab@its.ws",
                    FirstName = "Mohamed Omar El Khattab",
                    LastName = "Mohamed Omar El Khattab",
                    EmployeeId = 37
                };

                var result = await userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    await userManager.AddPasswordAsync(user, password);
                    await userManager.AddToRoleAsync(user, "Project Manager");
                }
            }
        }
    }
}
