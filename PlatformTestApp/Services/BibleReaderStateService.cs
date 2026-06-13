using Platform.API.Models;

namespace PlatformTestApp.Services;

public sealed class BibleReaderStateService : IBibleReaderStateService
{
    public BibleVersionSummary? SelectedVersion { get; private set; }
    public Book? SelectedBook { get; private set; }
    public int? SelectedChapter { get; private set; }
    public int? SelectedVerseStart { get; private set; }
    public int? SelectedVerseEnd { get; private set; }

    public event Action? OnStateChanged;

    public void SelectVersion(BibleVersionSummary version)
    {
        SelectedVersion = version;
        SelectedBook = null;
        SelectedChapter = null;
        SelectedVerseStart = null;
        SelectedVerseEnd = null;
        NotifyStateChanged();
    }

    public void SelectBook(Book book)
    {
        SelectedBook = book;
        SelectedChapter = null;
        SelectedVerseStart = null;
        SelectedVerseEnd = null;
        NotifyStateChanged();
    }

    public void SelectChapter(int chapter)
    {
        SelectedChapter = chapter;
        SelectedVerseStart = null;
        SelectedVerseEnd = null;
        NotifyStateChanged();
    }

    public void SelectVerseRange(int start, int? end)
    {
        SelectedVerseStart = start;
        SelectedVerseEnd = end;
        NotifyStateChanged();
    }

    public void Reset()
    {
        SelectedVersion = null;
        SelectedBook = null;
        SelectedChapter = null;
        SelectedVerseStart = null;
        SelectedVerseEnd = null;
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnStateChanged?.Invoke();
}
