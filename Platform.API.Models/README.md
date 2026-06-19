# YouVersion.Platform.API.Models

Domain model types for the [YouVersion Platform REST API](https://developers.youversion.com).

This package is a zero-dependency, pure-POCO library - no external NuGet dependencies required.
All types are immutable record types with System.Text.Json property-name attributes.

## Included types

| Type | Description |
|---|---|
| BibleVersionSummary | Lightweight version item returned by the list endpoint |
| BibleVersion | Full version metadata including available books |
| Book | A book within a Bible version (USFM code + human name) |
| Chapter | A chapter within a book |
| Verse | A single verse identifier |
| Passage | Scripture content returned by a passage fetch |
| PassageRequestOptions | Options controlling format, headings, and footnotes |
| PassageFormat | Text or Html enum |
| Highlight | A user Bible verse highlight |
| HighlightColor | Six-color enum for highlights |
| PagedResult(T) | Generic paged envelope with Data and NextPageToken |

## Target framework

net10.0

## Attribution

Always display the version Copyright field alongside Passage.Reference when showing Bible text.
