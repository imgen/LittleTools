using System.Text.Json.Serialization;

namespace SimpleTemple;

[JsonSerializable(typeof(JsonOptions))]
public partial class AppJsonSerializerContext : JsonSerializerContext
{
}