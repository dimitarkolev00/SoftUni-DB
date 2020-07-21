
using Newtonsoft.Json;

namespace CarDealer.DTO
{
    public class ExportPartDto
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Price")]
        public string Price { get; set; }
    }
}
