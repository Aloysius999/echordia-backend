using Newtonsoft.Json;

namespace Ech.WebApi.API
{
    public class ApiResponseModel
    {
        public ApiResponseModel()
        {
        }

        [JsonProperty("value")]
        public object Value { get; set; }
    }
}
