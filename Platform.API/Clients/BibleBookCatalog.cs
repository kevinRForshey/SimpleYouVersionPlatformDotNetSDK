#region  usings
using System.Reflection;
using System.Text.Json;
using Platform.API.Models;
using YouVersion.UsfmReferences;
#endregion

namespace Platform.API.Clients;

/// <summary>
/// Static catalog of Bible books, keyed by USFM book code.
/// Data is loaded from the embedded <c>books.json</c> resource.
/// </summary>
internal static class BibleBookCatalog
{
    private static readonly IReadOnlyDictionary<string, BookEntry> s_books = LoadBooks();

    /// <summary>
    /// Returns a <see cref="Book"/> for the given USFM code, using catalog metadata
    /// for the human-readable name and chapter count.  Falls back to the raw USFM code
    /// as the name when the code is unrecognised (e.g. deuterocanonical books).
    /// </summary>
    internal static Book FromUsfm(string usfm)
    {
        var isKnown = BookCatalog.IsKnownBook(usfm);
        var human = isKnown && s_books.TryGetValue(usfm, out var entry) ? entry.Human : usfm;
        var chapters = isKnown && s_books.TryGetValue(usfm, out var entry2) ? entry2.Chapters : 0;

        return new Book { Usfm = usfm, Human = human, ChapterCount = chapters };
    }

    private static IReadOnlyDictionary<string, BookEntry> LoadBooks()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = $"{assembly.GetName().Name}.Clients.books.json";

        using var stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException($"Embedded resource '{resourceName}' not found.");

        var entries = JsonSerializer.Deserialize<BookEntry[]>(stream,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
            ?? throw new InvalidOperationException("Failed to deserialize books.json.");

        return entries.ToDictionary(e => e.Usfm, e => e, StringComparer.OrdinalIgnoreCase);
    }

    private sealed record BookEntry(string Usfm, string Human, int Chapters);
}
