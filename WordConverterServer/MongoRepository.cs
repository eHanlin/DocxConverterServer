using System;
using System.Configuration;
using MongoDB.Driver;
using WordConverterServer.Models;

namespace WordConverterServer
{
    public class MongoRepository
    {
        private readonly IMongoCollection<ConvertTask> _collection;
        public MongoRepository()
        {
            MongoClient mongoClient = new MongoClient(ConfigurationManager.ConnectionStrings["mongo"].ConnectionString);
            var db = mongoClient.GetDatabase("wordconverter");
             _collection = db.GetCollection<ConvertTask>("WordConvertLogs");
        }

        public void Create(ConvertTask task)
        {
            try
            {
                _collection.InsertOne(task);
            }
            catch (Exception)
            {
               Delete(task.TaskId);
               Create(task);
            }
        }

        public ConvertTask Find(string taskId)
        {
            var filter = Builders<ConvertTask>.Filter.Eq(c=>c.TaskId, taskId);
            var result = _collection.Find(filter).First();
            return result;
        }

        public void Update(ConvertTask task)
        {
            Delete(task.TaskId);
            Create(task);
        }

        public void ConvertSuccese(ConvertTask task)
        {
            var filter = Builders<ConvertTask>.Filter.Eq(c => c.TaskId, task.TaskId);
            var update = Builders<ConvertTask>.Update.Set(c => c.ConvertSuccess, true);
            _collection.UpdateOne(filter, update);
        }

        public void ConvertFailed(ConvertTask task)
        {
            var filter = Builders<ConvertTask>.Filter.Eq(c => c.TaskId, task.TaskId);
            var update = Builders<ConvertTask>.Update.Set(c => c.ConvertSuccess, false);
            _collection.UpdateOne(filter, update);
        }

        public void HookSuccess(ConvertTask task)
        {
            var filter = Builders<ConvertTask>.Filter.Eq(c => c.TaskId, task.TaskId);
            var update = Builders<ConvertTask>.Update.Set(c => c.HookSuccess, true);
            _collection.UpdateOne(filter, update);
        }

        public void Delete(string taskId)
        {
            var filter = Builders<ConvertTask>.Filter.Eq(c=>c.TaskId, taskId);
            _collection.FindOneAndDelete(filter);
        }
    }
}