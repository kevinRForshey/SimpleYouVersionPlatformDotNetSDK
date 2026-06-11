namespace Platform.API.Models;

/// <summary>
/// Options that control how a passage is fetched from the YouVersion Platform API.
/// </summary>
public sealed record PassageRequestOptions
{
    /// <summary>
    /// The content format to return.
    /// Defaults to <see cref="PassageFormat.Text"/>.
    /// </summary>
    public PassageFormat Format { get; init; } = PassageFormat.Text;

    /// <summary>
    /// When <see langword="true"/>, section headings are included in the response.
    /// Only applies to <see cref="PassageFormat.Html"/>.
    /// Defaults to <see langword="false"/>.
    /// </summary>
    public bool IncludeHeadings { get; init; } = false;

    /// <summary>
    /// When <see langword="true"/>, footnotes are included in the response.
    /// Only applies to <see cref="PassageFormat.Html"/>.
    /// Defaults to <see langword="false"/>.
    /// </summary>
    public bool IncludeNotes { get; init; } = false;

    /// <summary>A shared instance representing the default text-only options.</summary>
    public static readonly PassageRequestOptions Default = new();
}
