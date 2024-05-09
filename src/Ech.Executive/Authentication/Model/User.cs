using System.Text.Json.Serialization;

namespace Ech.Executive.Authentication.Model
{
    public class User
    {
        public int id { get; set; }
        public string? forename { get; set; }
        public string? surname { get; set; }
        public string? alias { get; set; }
        public string? name { get; set; }
        public string? email { get; set; }

        [JsonIgnore]
        public string? hashedPassword { get; set; }
        public bool isActive { get; set; }
    }
}
