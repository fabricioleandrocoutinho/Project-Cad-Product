
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace Project.Cad.Data.Entities
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum StatusProduct
    {
        [BsonRepresentation(BsonType.String)]
        [Display(Name = "active")]
        ACTIVE = 0,

        [BsonRepresentation(BsonType.String)]
        [Display(Name = "inactive")]
        INACTIVE = 1,

        [BsonRepresentation(BsonType.String)]
        [Display(Name = "bloked")]
        BLOCKED = 2
    }

}
