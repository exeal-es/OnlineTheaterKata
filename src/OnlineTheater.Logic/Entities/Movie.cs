using Newtonsoft.Json;

namespace OnlineTheater.Logic.Entities;

public class Movie : Entity
{
    public string Name { get; set; }

    [JsonIgnore]
    public LicensingModel LicensingModel { get; set; }
}