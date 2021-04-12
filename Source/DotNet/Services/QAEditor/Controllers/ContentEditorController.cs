using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDB.Bson;
using Models;
using QAEditor.Helpers;

namespace QAEditor.Controllers
{
    [ApiController]
    [Route("")]
    public class ContentEditorController : ControllerBase
    {
        private readonly IMongoDatabase _db;
        private readonly IKafkaService _kafkaService;
        public ContentEditorController(IMongoDatabase db, IKafkaService kafkaService)
        {
            _kafkaService = kafkaService;
            _db = db;
        }

        [HttpPut("qas/{projectId}")]
        public async Task<string> Upsert(string projectId, QAModel qaData)
        {
            var collection = _db.GetCollection<BsonDocument>("qamodels");
            var filter = new BsonDocument("projectId", projectId);
            var options = new UpdateOptions() { IsUpsert = true };
            var update = Builders<BsonDocument>.Update.Set("data", qaData.data);
            collection.UpdateOne(filter, update, options);
            await _kafkaService.ProduceMessage("qa-updated", new { projectId = projectId });
            return "OK";
        }

        // [HttpPut("intents/{projectId}")]
        // public async Task<string> Upsert(string projectId, IntentProjectData intentData)
        // {
        //     var collection = _db.GetCollection<BsonDocument>("intentprojectdatas");
        //     var filter = new BsonDocument("projectId", projectId);
        //     var options = new UpdateOptions() { IsUpsert = true };
        //     var update = Builders<BsonDocument>.Update.Set("data", intentData.data);
        //     collection.UpdateOne(filter, update, options);
        //     await _kafkaService.ProduceMessage("intents-updated", new { projectId = projectId });
        //     return "OK";
        // }
    }
}
