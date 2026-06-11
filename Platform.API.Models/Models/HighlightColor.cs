using System.Text.Json.Serialization;

namespace Platform.API.Models;

/// <summary>
/// Color options available when creating or updating a Bible verse highlight.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum HighlightColor
{
    /// <summary>Yellow highlight.</summary>
    Yellow,

    /// <summary>Green highlight.</summary>
    Green,

    /// <summary>Blue highlight.</summary>
    Blue,

    /// <summary>Orange highlight.</summary>
    Orange,

    /// <summary>Pink highlight.</summary>
    Pink,

    /// <summary>Purple highlight.</summary>
    Purple
}
