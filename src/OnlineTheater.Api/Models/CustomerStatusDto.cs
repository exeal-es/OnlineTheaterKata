
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace OnlineTheater.Api.Controllers;

[JsonConverter(typeof(StringEnumConverter))]
public enum CustomerStatusDto
{
    Regular = 1,
    Advanced = 2
}