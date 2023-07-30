using Authy.Common.Entities;
using Authy.Domain.Interfaces;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authy.Domain.Services
{
    public class UserRoleService : IUserRoleService
    {
        private readonly IDbConnectionFactory _dbFactory;

        public UserRoleService(IDbConnectionFactory dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public async Task<List<Role>> GetRoles()
        {
            using (IDbConnection conn = _dbFactory.OpenDbConnection())
            {
                var roles = await conn.SelectAsync<Role>();

                return roles;
            }
        }

        public async Task<Role> CreateRole(string name, string code, string description, bool isCoreRole)
        {
            var role = new Role() { Name = name, Code = code, Description = description, IsCoreRole = isCoreRole };

            role.CreatedTimestamp = DateTime.UtcNow;
            role.LastModifiedTimestamp = DateTime.UtcNow;

            using (IDbConnection conn = _dbFactory.OpenDbConnection())
            {
                await conn.SaveAsync(role);
            }

            return role;
        }

        public async Task<bool> RemoveRole(int roleId)
        {
            using (IDbConnection conn = _dbFactory.OpenDbConnection())
            {
                await conn.DeleteWhereAsync<UserRole>("RoleId = {0}", new object[] { roleId });
                await conn.DeleteByIdAsync<Role>(roleId);
            }

            return true;
        }

        public async Task<bool> AddUserToRole(User user, int roleId)
        {
            var userRole = new UserRole()
            {
                TenantId = user.TenantId,
                UserId = user.Id,
                RoleId = roleId,
                CreatedTimestamp = DateTime.UtcNow,
                LastModifiedTimestamp = DateTime.UtcNow
            };

            using (IDbConnection conn = _dbFactory.OpenDbConnection())
            {
                await conn.SaveAsync(userRole);

                user.Roles.Add(userRole);
            }

            return true;
        }

        public async Task<bool> AddUserToRole(User user, string roleName)
        {
            using (IDbConnection conn = _dbFactory.OpenDbConnection())
            {
                var role = await conn.SingleWhereAsync<Role>("Name", roleName);

                if (role == null) return false;
                
                var userRole = new UserRole()
                {
                    TenantId = user.TenantId,
                    UserId = user.Id,
                    RoleId = role.Id,
                    CreatedTimestamp = DateTime.UtcNow,
                    LastModifiedTimestamp = DateTime.UtcNow,
                    Role = role
                };

                await conn.SaveAsync(userRole);

                user.Roles.Add(userRole);
            }

            return true;
        }

        public async Task<bool> RemoveUserFromRole(User user, int roleId)
        {
            using (IDbConnection conn = _dbFactory.OpenDbConnection())
            {
                var userRole = await conn.SingleAsync<UserRole>(x => x.RoleId == roleId && x.TenantId == user.TenantId && x.UserId == user.Id);
                await conn.DeleteAsync(userRole);

                user.Roles = user.Roles.Where(x => x.RoleId != roleId).ToList();
            }

            return true;
        }
    }
}
