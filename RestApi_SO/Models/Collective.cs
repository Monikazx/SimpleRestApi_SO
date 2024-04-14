using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestApiSO.Models
{
    public class Collective
    {
        public int CollectiveId;
        
        [JsonProperty("description")]
        public string Description;

        [JsonProperty("external_links")]
        public LinkedList<CollectiveExternalLink> ExternalLinks;

        [JsonProperty("link")]
        public string Link;

        [JsonProperty("name")]
        public string Name;

        [JsonProperty("slug")]
        public string Slug;

        [JsonProperty("tags")]
        public string[] Tags;
    }
}
