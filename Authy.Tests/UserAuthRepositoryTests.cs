using Authy.Common.Entities;
using Authy.Common.Enums;
using Authy.Data.Interfaces;
using Authy.Data.Repositories;
using Authy.Domain.Interfaces;
using Authy.Domain.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceStack.Configuration;
using ServiceStack.OrmLite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Authy.Common.Entities;
using Authy.Data.Interfaces;
using Authy.Data.Repositories;

namespace Authy.Tests
{
    [TestClass]
    public class UserAuthRepositoryTests
    {
        [TestMethod]
        public async Task TestUserAuthRepoCreateUserWorks()
        {
            try
            {
                var dbConnStr = "Server=tcp:obrc-redemptionsolution.database.windows.net,1433;Initial Catalog=OBRC-RS-UAT;Persist Security Info=False;User ID=RedemptionAdmin;Password=!@redEm{}NOW;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
                var dbFactory = new OrmLiteConnectionFactory(dbConnStr, SqlServer2012Dialect.Provider);

                IAsyncRepository<User, long> authRepo = new UserAuthRepository(dbFactory);

                var user = new User()
                {
                    TenantId = 1,
                    FirstName = "Test",
                    LastName = "Testerson",
                    Email = "test@obrc.com",
                    PIN = "1234",
                    PasswordHash = Guid.NewGuid().ToString(),
                    IsActive = true,
                    Salt = Guid.NewGuid().ToString(),
                    CreatedTimestamp = DateTime.UtcNow,
                    LastModifiedTimestamp = DateTime.UtcNow,
                };

                var createdUser = await authRepo.CreateAsync(user);

                Assert.IsTrue(createdUser.Id > 0);
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
            }
        }

        [TestMethod]
        public async Task TestUserAuthRepoFetchUsersWorks()
        {
            try
            {
                var dbConnStr = "Server=tcp:obrc-redemptionsolution.database.windows.net,1433;Initial Catalog=OBRC-RS-UAT;Persist Security Info=False;User ID=RedemptionAdmin;Password=!@redEm{}NOW;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
                var dbFactory = new OrmLiteConnectionFactory(dbConnStr, SqlServer2012Dialect.Provider);

                IAsyncRepository<User, long> authRepo = new UserAuthRepository(dbFactory);

                var users = await authRepo.FindAllAsync();

                Assert.IsTrue(users.Any());
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
            }
        }

        [TestMethod]
        public async Task TestUserRoleServiceCreateRoleWorks()
        {
            try
            {
                var dbConnStr = "Server=tcp:obrc-redemptionsolution.database.windows.net,1433;Initial Catalog=OBRC-RS-UAT;Persist Security Info=False;User ID=RedemptionAdmin;Password=!@redEm{}NOW;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
                var dbFactory = new OrmLiteConnectionFactory(dbConnStr, SqlServer2012Dialect.Provider);

                IUserRoleService roleService = new UserRoleService(dbFactory);
                IAsyncRepository<User, long> authRepo = new UserAuthRepository(dbFactory);

                var user = await authRepo.FindByIdAsync(1);

                Assert.IsNotNull(user);

                var admRole = await roleService.CreateRole("Admin", "ADMIN", "Super user with all the powah", true);
                var userRole = await roleService.CreateRole("User", "USER", "Just a plain old boring regular user", false);

                await roleService.AddUserToRole(user, admRole.Id);
                await roleService.AddUserToRole(user, userRole.Id);

                Assert.IsTrue(user.Roles.Count == 2);
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
            }
        }

        [TestMethod]
        public async Task TestUserRoleServiceDeleteRoleWorks()
        {
            try
            {
                var dbConnStr = "Server=tcp:obrc-redemptionsolution.database.windows.net,1433;Initial Catalog=OBRC-RS-UAT;Persist Security Info=False;User ID=RedemptionAdmin;Password=!@redEm{}NOW;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
                var dbFactory = new OrmLiteConnectionFactory(dbConnStr, SqlServer2012Dialect.Provider);

                IUserRoleService roleService = new UserRoleService(dbFactory);
                IAsyncRepository<User, long> authRepo = new UserAuthRepository(dbFactory);

                var roles = await roleService.GetRoles();

                foreach (var role in roles)
                {
                    await roleService.RemoveRole(role.Id);
                }

                var user = await authRepo.FindByIdAsync(1);

                Assert.IsNotNull(user);

                Assert.IsTrue(user.Roles.Count == 0);
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
            }
        }

        [TestMethod]
        public async Task TestUserRoleServiceRemoveRoleFromUserWorks()
        {
            try
            {
                var dbConnStr = "Server=tcp:obrc-redemptionsolution.database.windows.net,1433;Initial Catalog=OBRC-RS-UAT;Persist Security Info=False;User ID=RedemptionAdmin;Password=!@redEm{}NOW;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
                var dbFactory = new OrmLiteConnectionFactory(dbConnStr, SqlServer2012Dialect.Provider);

                IUserRoleService roleService = new UserRoleService(dbFactory);
                IAsyncRepository<User, long> authRepo = new UserAuthRepository(dbFactory);

                var user = await authRepo.FindByIdAsync(1);

                Assert.IsNotNull(user);

                foreach (var userRole in user.Roles)
                {
                    await roleService.RemoveUserFromRole(user, userRole.RoleId);
                }

                Assert.IsTrue(user.Roles.Count == 0);
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
            }
        }
    }
}
