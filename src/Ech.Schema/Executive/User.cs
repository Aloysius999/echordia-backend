using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Ech.Schema.Executive
{
    public class User
    {
        public enum Role
        {
            User,
            Admin,
            SuperUser,
            System,
        }

        public int id { get; set; }
        public string? forename { get; set; }
        public string? surname { get; set; }
        public string? alias { get; set; }
        public string? name { get; set; }
        public string? email { get; set; }

        [JsonIgnore]
        public string? hashedPassword { get; set; }
        public bool isActive { get; set; }

        [NotMapped]
        public Role roleId { get; set; }
        public string? role
        {
            get => roleId.ToString();
            set => roleId = (Role)Enum.Parse(typeof(Role), value);
        }

        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
    }
}
