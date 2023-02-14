using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Project.Cad.Data.Models;

public sealed class DefaultFilter
{
    /// <summary>
    /// Contains the page number that will be returned, starting at 0
    /// </summary>
    [DefaultValue(0)]
    [JsonPropertyName("_page")]
    public int Page { get; set; } = 0;

    /// <summary>
    /// Indicates the page size (or number of records to return)
    /// </summary>
    [DefaultValue(50)]
    [JsonPropertyName("_size")]
    public int Size { get; set; } = 50;

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("_order")]
    public string Order { get; set; }
}
