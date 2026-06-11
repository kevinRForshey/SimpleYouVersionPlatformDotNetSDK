using System.Collections.Generic;
using System.Text.Json;
using FluentAssertions;
using Platform.API.Models;
using Xunit;

namespace Platform.API.Tests.Models;

public sealed class PagedResultTests
{
    [Fact]
    public void PagedResult_DefaultData_IsEmpty()
    {
        var result = new PagedResult<string>();
        result.Data.Should().BeEmpty();
        result.NextPageToken.Should().BeNull();
    }

    [Fact]
    public void PagedResult_InitProperties_AreSet()
    {
        var result = new PagedResult<int>
        {
            Data = new List<int> { 1, 2, 3 },
            NextPageToken = "next"
        };

        result.Data.Should().HaveCount(3);
        result.NextPageToken.Should().Be("next");
    }

    [Fact]
    public void PagedResult_DeserializesFromJson_Correctly()
    {
        const string json = """
            { "data": [10, 20], "next_page_token": "tok-abc" }
            """;

        var result = JsonSerializer.Deserialize<PagedResult<int>>(json)!;

        result.Data.Should().HaveCount(2);
        result.Data[0].Should().Be(10);
        result.NextPageToken.Should().Be("tok-abc");
    }

    [Fact]
    public void PagedResult_DeserializesNullNextPageToken_Correctly()
    {
        const string json = """{"data":[],"next_page_token":null}""";

        var result = JsonSerializer.Deserialize<PagedResult<int>>(json)!;

        result.NextPageToken.Should().BeNull();
    }

    [Fact]
    public void PagedResult_SupportsWithExpression_ForImmutableUpdates()
    {
        var original = new PagedResult<string>
        {
            Data = new[] { "a" },
            NextPageToken = "tok"
        };

        var updated = original with { NextPageToken = "new-tok" };

        updated.NextPageToken.Should().Be("new-tok");
        updated.Data.Should().BeSameAs(original.Data);
        original.NextPageToken.Should().Be("tok"); // original unchanged
    }
}

public sealed class PassageRequestOptionsTests
{
    [Fact]
    public void Default_HasTextFormat_AndNoExtras()
    {
        var opts = PassageRequestOptions.Default;

        opts.Format.Should().Be(PassageFormat.Text);
        opts.IncludeHeadings.Should().BeFalse();
        opts.IncludeNotes.Should().BeFalse();
    }

    [Fact]
    public void PassageRequestOptions_CanBeConfiguredForHtml()
    {
        var opts = new PassageRequestOptions
        {
            Format = PassageFormat.Html,
            IncludeHeadings = true,
            IncludeNotes = true
        };

        opts.Format.Should().Be(PassageFormat.Html);
        opts.IncludeHeadings.Should().BeTrue();
        opts.IncludeNotes.Should().BeTrue();
    }

    [Fact]
    public void PassageRequestOptions_WithExpression_ProducesNewInstance()
    {
        var original = PassageRequestOptions.Default;
        var updated = original with { Format = PassageFormat.Html };

        updated.Format.Should().Be(PassageFormat.Html);
        original.Format.Should().Be(PassageFormat.Text); // original unchanged
    }

    [Fact]
    public void Default_IsSingletonInstance()
    {
        PassageRequestOptions.Default.Should().BeSameAs(PassageRequestOptions.Default);
    }
}

public sealed class BibleVersionTests
{
    [Fact]
    public void BibleVersion_DeserializesFromJson_Correctly()
    {
        const string json = """
            {
              "id": 3034, "abbreviation": "BSB", "localized_abbreviation": "BSB",
              "title": "Berean Standard Bible", "localized_title": "Berean Standard Bible",
              "language_tag": "en", "copyright": "Public Domain",
              "promotional_content": "Free to use", "publisher_url": null,
              "books": ["GEN","EXO"],
              "youversion_deep_link": "https://www.bible.com/versions/3034"
            }
            """;

        var version = JsonSerializer.Deserialize<BibleVersion>(json)!;

        version.Id.Should().Be(3034);
        version.Abbreviation.Should().Be("BSB");
        version.LanguageTag.Should().Be("en");
        version.Copyright.Should().Be("Public Domain");
        version.Books.Should().HaveCount(2);
        version.Books[0].Should().Be("GEN");
        version.PublisherUrl.Should().BeNull();
        version.YouVersionDeepLink.Should().Contain("3034");
    }

    [Fact]
    public void BibleVersion_Books_DefaultsToEmpty()
    {
        var version = new BibleVersion();
        version.Books.Should().BeEmpty();
    }
}

public sealed class PassageTests
{
    [Fact]
    public void Passage_DeserializesFromJson_Correctly()
    {
        const string json = """
            { "id": "JHN.3.16", "content": "For God so loved...", "reference": "John 3:16" }
            """;

        var passage = JsonSerializer.Deserialize<Passage>(json)!;

        passage.Id.Should().Be("JHN.3.16");
        passage.Content.Should().Contain("God so loved");
        passage.Reference.Should().Be("John 3:16");
    }

    [Fact]
    public void Passage_WithExpression_PreservesUnchangedProperties()
    {
        var original = new Passage { Id = "GEN.1.1", Content = "In the beginning...", Reference = "Genesis 1:1" };
        var updated = original with { Content = "Updated content" };

        updated.Id.Should().Be("GEN.1.1");
        updated.Reference.Should().Be("Genesis 1:1");
        original.Content.Should().Be("In the beginning..."); // original unchanged
    }
}

public sealed class HighlightTests
{
    [Fact]
    public void Highlight_DeserializesFromJson_Correctly()
    {
        const string json = """
            {
              "id": "hl-1", "usfm": "JHN.3.16", "version_id": 3034,
              "color": "Yellow",
              "created_at": "2024-06-01T12:00:00Z",
              "updated_at": "2024-06-01T13:00:00Z"
            }
            """;

        var highlight = JsonSerializer.Deserialize<Highlight>(json)!;

        highlight.Id.Should().Be("hl-1");
        highlight.Usfm.Should().Be("JHN.3.16");
        highlight.VersionId.Should().Be(3034);
        highlight.Color.Should().Be(HighlightColor.Yellow);
        highlight.CreatedAt.Should().BeAfter(System.DateTimeOffset.MinValue);
    }

    [Fact]
    public void Highlight_DefaultValues_AreEmpty()
    {
        var h = new Highlight();
        h.Id.Should().BeEmpty();
        h.Usfm.Should().BeEmpty();
        h.VersionId.Should().Be(0);
    }
}
