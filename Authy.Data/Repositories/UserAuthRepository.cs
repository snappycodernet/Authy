using Authy.Data.Interfaces;
using Authy.Data.Models;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Authy.Data.Repositories
{
    public class UserAuthRepository : IAsyncRepository<User, long>
    {
        IDbConnectionFactory _db;

        public UserAuthRepository(IDbConnectionFactory dbFactory)
        {
            _db = dbFactory;
        }

        public async Task<User> CreateAsync(User entity)
        {
            using (var db = _db.CreateDbConnection())
            {
                await db.SaveAsync(entity);

                return entity;
            }
        }

        public async Task DeleteAsync(long id)
        {
            using(var db = _db.CreateDbConnection())
            {
                await db.DeleteByIdAsync<User>(id);
            }
        }

        public async Task DeleteAsync(User entity)
        {
            using (var db = _db.CreateDbConnection())
            {
                await db.DeleteAsync(entity);
            }
        }

        public async Task<IEnumerable<User>> FindAllAsync()
        {
            using (var db = _db.CreateDbConnection())
            {
                var users = await db.LoadSelectAsync(db.From<User>());

                return users;
            }
        }

        public async Task<IEnumerable<User>> FindByConditionAsync(Expression<Func<User, bool>> condition)
        {
            using (var db = _db.CreateDbConnection())
            {
                var users = await db.LoadSelectAsync<User>(condition);

                return users;
            }
        }

        public async Task<User> FindByIdAsync(long id)
        {
            using(var db = _db.CreateDbConnection())
            {
                var user = await db.LoadSingleByIdAsync<User>(id);

                return user;
            }
        }

        public async Task<User> UpdateAsync(long id, User entity)
        {
            using (var db = _db.CreateDbConnection())
            {
                var existingUser = await FindByIdAsync(id);
                existingUser.Email = entity.Email;

                existingUser.FirstName = entity.FirstName;
                existingUser.MiddleName = entity.MiddleName;
                existingUser.LastName = entity.LastName;
                existingUser.PIN = entity.PIN;
                existingUser.IsActive = entity.IsActive;
                existingUser.LastModifiedUserId = entity.LastModifiedUserId;
                existingUser.PasswordHash = entity.PasswordHash;
                existingUser.LastModifiedTimestamp = DateTime.UtcNow;

                db.Update(existingUser);

                return existingUser;
            }
        }
    }
}
