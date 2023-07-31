using Authy.Common.Entities;
using Authy.Data.Interfaces;
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
    public class UserAuthRepositorySync : IRepository<User, long>
    {
        private static object _lock = new object();

        IDbConnectionFactory _db;

        public UserAuthRepositorySync(IDbConnectionFactory dbFactory)
        {
            _db = dbFactory;
        }

        public User Create(User entity)
        {
            lock(_lock)
            {
                using (var db = _db.OpenDbConnection())
                {
                    entity.CreatedTimestamp = DateTime.UtcNow;
                    entity.LastModifiedTimestamp = DateTime.UtcNow;

                    db.Save(entity);
                    db.LoadReferences<User>(entity);

                    return entity;
                }
            }
        }

        public void Delete(long id)
        {
            lock(_lock)
            {
                using (var db = _db.OpenDbConnection())
                {
                    db.DeleteById<User>(id);
                }
            }
        }

        public void Delete(User entity)
        {
            lock (_lock)
            {
                using (var db = _db.OpenDbConnection())
                {
                    db.Delete(entity);
                }
            }
        }

        public IEnumerable<User> FindAll()
        {
            lock (_lock)
            {
                using (var db = _db.OpenDbConnection())
                {
                    var users = db.LoadSelect(db.From<User>());

                    foreach (var role in users.SelectMany(x => x.Roles))
                    {
                        db.LoadReferences(role);
                    }

                    return users;
                }
            }
        }

        public IEnumerable<User> FindByCondition(Expression<Func<User, bool>> condition)
        {
            lock (_lock)
            {
                using (var db = _db.OpenDbConnection())
                {
                    var users = db.LoadSelect<User>(condition);

                    foreach (var role in users.SelectMany(x => x.Roles))
                    {
                        db.LoadReferences(role);
                    }

                    return users;
                }
            }
        }

        public User FindById(long id)
        {
            lock (_lock)
            {
                using (var db = _db.OpenDbConnection())
                {
                    var user = db.LoadSingleById<User>(id);

                    foreach (var role in user.Roles)
                    {
                        db.LoadReferences(role);
                    }

                    return user;
                }
            }
        }

        public User Update(long id, User entity)
        {
            lock (_lock)
            {
                using (var db = _db.OpenDbConnection())
                {
                    var existingUser = FindById(id);
                    existingUser.Email = entity.Email;

                    existingUser.FirstName = entity.FirstName;
                    existingUser.MiddleName = entity.MiddleName;
                    existingUser.LastName = entity.LastName;
                    existingUser.PIN = entity.PIN;
                    existingUser.IsActive = entity.IsActive;
                    existingUser.LastModifiedUserId = entity.LastModifiedUserId;
                    existingUser.PasswordHash = entity.PasswordHash;
                    existingUser.Salt = entity.Salt;
                    existingUser.LastModifiedTimestamp = DateTime.UtcNow;

                    db.Update(existingUser);

                    return existingUser;
                }
            }
        }
    }
}
