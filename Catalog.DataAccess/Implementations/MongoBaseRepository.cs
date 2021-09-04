using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Catalog.DataAccess.Interfaces;
using Catalog.DataAccess.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Catalog.DataAccess.Implementations
{
    // TODO: Exception handling!!!
    public class MongoBaseRepository<TRepoEntity> : IBaseRepository<TRepoEntity> where TRepoEntity: MongoRepoEntity
    {
        private readonly ILogger<MongoBaseRepository<TRepoEntity>> _logger;
        protected readonly IMongoCollection<TRepoEntity> Items;
        
        public MongoBaseRepository(ILogger<MongoBaseRepository<TRepoEntity>> logger, IMongoOptions mongoOptions)
        {
            _logger = logger;
            var client = new MongoClient(mongoOptions.ConnectionString);
            var database = client.GetDatabase(mongoOptions.DatabaseName);

            Items = database.GetCollection<TRepoEntity>(typeof(TRepoEntity).Name);
        }
        
        public async Task<List<TRepoEntity>> GetAsync() => 
            await (await Items.FindAsync(item => true)).ToListAsync();

        public async Task<TRepoEntity> GetAsync(string id) =>
            await (await Items.FindAsync(item => item.Id == id)).FirstOrDefaultAsync();

        public async Task<TRepoEntity> CreateAsync(TRepoEntity item)
        {
            // Assign unique id
            item.Id = ObjectId.GenerateNewId().ToString();
            await Items.InsertOneAsync(item);
            return item;
        }
        
        public async Task UpdateAsync(string id, TRepoEntity itemIn) =>
            await Items.ReplaceOneAsync(item => item.Id == id, itemIn);

        public async Task RemoveAsync(TRepoEntity itemIn) =>
            await Items.DeleteOneAsync(item => item.Id == itemIn.Id);

        public async Task RemoveAsync(string id) =>
            await Items.DeleteOneAsync(item => item.Id == id);

        public async Task RemoveManyAsync(string[] ids) =>
            await Items.DeleteManyAsync(item => ids.Contains(item.Id));
    }
}