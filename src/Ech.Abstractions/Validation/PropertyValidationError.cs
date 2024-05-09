using Newtonsoft.Json;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Ech.Abstractions.Validation
{
    //
    // Summary:
    //     Property validation error
    public class PropertyValidationError
    {
        public PropertyValidationError() { }

        //
        // Summary:
        //     Property name
        [DataMember(EmitDefaultValue = false)]
        [DefaultValue("")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string PropertyName { get; set; }
        //
        // Summary:
        //     Validation error message
        [DataMember(EmitDefaultValue = false)]
        [DefaultValue("")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string ErrorMessage { get; set; }
    }
}
