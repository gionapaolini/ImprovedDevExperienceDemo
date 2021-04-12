using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Models
{
    [BsonIgnoreExtraElements]
    public class QAModel
    {   
        public string projectId { get; set; }

        public string data { get; set; }
    }
}