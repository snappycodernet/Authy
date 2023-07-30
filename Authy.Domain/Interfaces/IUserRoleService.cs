using Authy.Common.Entities;
using System.Data;

namespace Authy.Domain.Interfaces
{
    public interface IUserRoleService
    {
        Task<bool> AddUserToRole(User user, int roleId);
        Task<bool> AddUserToRole(User user, string roleName);
        Task<Role> CreateRole(string name, string code, string description, bool isCoreRole);
        Task<List<Role>> GetRoles();
        Task<bool> RemoveRole(int roleId);
        Task<bool> RemoveUserFromRole(User user, int roleId);
    }
}