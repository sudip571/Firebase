using Newtonsoft.Json.Converters;
using System.Text.Json.Serialization;

namespace TestRichard.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Status
    {        
        Failed,
        Success
    }
}
