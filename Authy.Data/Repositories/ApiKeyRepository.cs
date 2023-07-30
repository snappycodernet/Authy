﻿using Authy.Data.Interfaces;
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
    public class ApiKeyRepository : IAsyncRepository<ApiKey, string>
    {
        IDbConnectionFactory _db;

        public ApiKeyRepository(IDbConnectionFactory dbFactory)
        {
            _db = dbFactory;
        }

        public async Task<ApiKey> CreateAsync(ApiKey entity)
        {
            using (var db = _db.OpenDbConnection())
            {
                await db.SaveAsync(entity);

                return entity;
            }
        }

        public async Task DeleteAsync(string id)
        {
            using (var db = _db.OpenDbConnection())
            {
                await db.DeleteByIdAsync<ApiKey>(id);
            }
        }

        public async Task DeleteAsync(ApiKey entity)
        {
            using (var db = _db.OpenDbConnection())
            {
                await db.DeleteAsync(entity);
            }
        }

        public async Task<IEnumerable<ApiKey>> FindAllAsync()
        {
            using (var db = _db.OpenDbConnection())
            {
                var keys = await db.LoadSelectAsync(db.From<ApiKey>());

                return keys;
            }
        }

        public async Task<IEnumerable<ApiKey>> FindByConditionAsync(Expression<Func<ApiKey, bool>> condition)
        {
            using (var db = _db.OpenDbConnection())
            {
                var keys = await db.LoadSelectAsync<ApiKey>(condition);

                return keys;
            }
        }

        public async Task<ApiKey> FindByIdAsync(string id)
        {
            using (var db = _db.OpenDbConnection())
            {
                var key = await db.LoadSingleByIdAsync<ApiKey>(id);

                return key;
            }
        }

        public async Task<ApiKey> UpdateAsync(string id, ApiKey entity)
        {
            using (var db = _db.OpenDbConnection())
            {
                var existingKey = await FindByIdAsync(id);
                existingKey.UserAuthId = entity.UserAuthId;
                existingKey.Environment = entity.Environment;
                existingKey.KeyType = entity.KeyType;
                existingKey.ExpiryDate = entity.ExpiryDate;
                existingKey.CancelledDate = entity.CancelledDate;
                existingKey.Notes = entity.Notes;
                existingKey.RefIdStr = entity.RefIdStr;
                existingKey.RefId = entity.RefId;
                existingKey.Meta = entity.Meta;

                db.Update(existingKey);

                return existingKey;
            }
        }
    }
}