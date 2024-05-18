using System.Collections.Generic;
using Newtonsoft.Json;

namespace Ech.WebApi.API
{
    public class ApiQueryResponseModel<T>
    {
        public ApiQueryResponseModel()
        {
            Count = 0;
            Results = new List<T>();
            Success = false;
            Next = "";
            Previous = "";
        }

        public ApiQueryResponseModel(long count, IEnumerable<T> results)
            : this()
        {
            this.Count = count;
            this.Results = results;
            Success = true;
        }

        public ApiQueryResponseModel(IEnumerable<T> results)
            : this()
        {
            var lst = new List<T>(results);
            this.Count = lst.Count;
            this.Results = results;
            Success = true;
        }

        public ApiQueryResponseModel(T result)
            : this()
        {
            this.Count = 1;
            this.Results = new List<T>() { result };
            Success = true;
        }

        public ApiQueryResponseModel(bool success)
            : this()
        {
            Success = success;
        }

        [JsonProperty("count")]
        public long Count { get; set; }

        [JsonProperty("results")]
        public IEnumerable<T> Results { get; set;}

        [JsonProperty("success")]
        public bool Success { get; set; }

        /// <summary>
        /// Provides a URL to the next page when navigating
        /// </summary>
        [JsonProperty("next")]
        public string Next { get; set; }

        /// <summary>
        /// Provides a URL to the previous page when navigating
        /// </summary>       
        [JsonProperty("previous")]
        public string Previous { get; set; }
    }
}
