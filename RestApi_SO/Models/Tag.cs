using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestApiSO.Models
{
    public class Tag
    {
        public int TagId { get; set; }

        [JsonProperty("collectives")]
        public LinkedList<Collective> Collectives { get; set; }

        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("has_synonyms")]
        public bool HasSynonyms { get; set; }

        [JsonProperty("is_moderator_only")]
        public bool IsModeratorOnly { get; set; }

        [JsonProperty("is_required")]
        public bool IsRequired { get; set; }

        [JsonProperty("last_activity_date")]
        public DateTime LastActivityDate { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }


        [JsonProperty("user_id")]
        public int UserId { get; set; }

       
    }
}
