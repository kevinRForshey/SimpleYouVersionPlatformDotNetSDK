using Platform.API.Models;

namespace PlatformTestApp.Services;

public interface IBibleReaderStateService
{
    BibleVersionSummary? SelectedVersion { get; }
    Book? SelectedBook { get; }
    int? SelectedChapter { get; }
    int? SelectedVerseStart { get; }
    int? SelectedVerseEnd { get; }

    void SelectVersion(BibleVersionSummary version);
    void SelectBook(Book book);
    void SelectChapter(int chapter);
    void SelectVerseRange(int start, int? end);
    void Reset();

    event Action? OnStateChanged;
}
