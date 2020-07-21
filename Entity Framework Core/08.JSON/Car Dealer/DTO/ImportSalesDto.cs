using Newtonsoft.Json;

namespace CarDealer.DTO
{
    public class ImportSalesDto
    {
        [JsonProperty("carId")]
        public int CarId { get; set; }

        [JsonProperty("customerId")]
        public int CustomerId { get; set; }

        [JsonProperty("discount")]
        public int Discount { get; set; }
    }
}
