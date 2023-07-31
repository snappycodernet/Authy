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
    public class ApiKeyRepositorySync : IRepository<ApiKey, long>
    {
        private static object _lock = new object();

        IDbConnectionFactory _db;

        public ApiKeyRepositorySync(IDbConnectionFactory dbFactory)
        {
            _db = dbFactory;
        }

        public ApiKey Create(ApiKey entity)
        {
            lock(_lock)
            {
                using (var db = _db.OpenDbConnection())
                {
                    db.Save(entity);

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
                    db.DeleteById<ApiKey>(id);
                }
            }
        }

        public void Delete(ApiKey entity)
        {
            lock (_lock)
            {
                using (var db = _db.OpenDbConnection())
                {
                    db.Delete(entity);
                }
            }
        }

        public IEnumerable<ApiKey> FindAll()
        {
            lock (_lock)
            {
                using (var db = _db.OpenDbConnection())
                {
                    var keys = db.LoadSelect(db.From<ApiKey>());

                    return keys;
                }
            }
        }

        public IEnumerable<ApiKey> FindByCondition(Expression<Func<ApiKey, bool>> condition)
        {
            lock (_lock)
            {
                using (var db = _db.OpenDbConnection())
                {
                    var keys = db.LoadSelect<ApiKey>(condition);

                    return keys;
                }
            }
        }

        public ApiKey FindById(long id)
        {
            lock (_lock)
            {
                using (var db = _db.OpenDbConnection())
                {
                    var key = db.LoadSingleById<ApiKey>(id);

                    return key;
                }
            }
        }

        public ApiKey Update(long id, ApiKey entity)
        {
            lock (_lock)
            {
                using (var db = _db.OpenDbConnection())
                {
                    var existingKey = FindById(id);

                    existingKey.CancelledDate = entity.CancelledDate;
                    existingKey.Notes = entity.Notes;
                    existingKey.RefId = entity.RefId;
                    existingKey.RefIdStr = entity.RefIdStr;

                    db.Update(existingKey);

                    return existingKey;
                }
            }
        }
    }
}
