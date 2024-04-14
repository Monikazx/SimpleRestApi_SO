using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace RestApiSO.Models
{
    public class CollectiveExternalLink
    {
        public int CollectiveExternalLinkId;

        [JsonProperty("link")]
        public string Link { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }  // one of website, twitter, github, facebook, instagram, support, or linkedin
    }
}
