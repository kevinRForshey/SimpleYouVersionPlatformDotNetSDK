#region usings
using Microsoft.AspNetCore.Components;
using Platform.API.Models;
using Platform.API.OAuth;
using Platform.SDK.Services;
using YouVersion.UsfmReferences;
#endregion

namespace Platform.SDK.Components.BibleComponents
{
    public partial class BibleReader : IDisposable
    {
        // ── Injected services ────────────────────────────────────────────────
        [Inject] private IBibleReaderStateService State { get; set; } = default!;
        [Inject] private ITokenProvider TokenProvider { get; set; } = default!;
        [Inject] private NavigationManager Nav { get; set; } = default!;
        [Inject] private IPassageService PassageService { get; set; } = default!;

        // ── Customisation parameters ─────────────────────────────────────────

        /// <summary>Heading displayed at the top of the component. Defaults to "Bible Reader".</summary>
        [Parameter] public string Title { get; set; } = "Bible Reader";

        /// <summary>
        /// BCP-47 language range used to filter the Bible version list (e.g. "en", "es", "fr").
        /// Cascades down to <see cref="VersionPicker"/>. Defaults to English.
        /// </summary>
        [Parameter] public string LanguageRange { get; set; } = "en";

        /// <summary>
        /// Passage format requested from the API. Defaults to <see cref="PassageFormat.Html"/>.
        /// Use <see cref="PassageFormat.Text"/> to receive plain text suitable for display or processing.
        /// </summary>
        [Parameter] public PassageFormat Format { get; set; } = PassageFormat.Html;

        /// <summary>
        /// Fired after a passage is successfully loaded.
        /// Use this to update parent state, log analytics, copy text, etc.
        /// </summary>
        [Parameter] public EventCallback<Passage> OnPassageLoaded { get; set; }

        /// <summary>
        /// Optional custom rendering for the loaded passage.
        /// The <see cref="Passage"/> is passed as the render-fragment context.
        /// When omitted the built-in <c>VerseComponent</c> is used.
        /// </summary>
        [Parameter] public RenderFragment<Passage>? PassageTemplate { get; set; }

        /// <summary>
        /// Invoked when the user clicks "Sign in".
        /// When no delegate is provided the component falls back to navigating to <see cref="LoginPath"/>.
        /// </summary>
        [Parameter] public EventCallback OnSignInRequested { get; set; }

        /// <summary>
        /// Invoked when the user clicks "Sign out".
        /// When no delegate is provided the component falls back to navigating to <see cref="LogoutPath"/>.
        /// </summary>
        [Parameter] public EventCallback OnSignOutRequested { get; set; }

        /// <summary>Sign-in route used when <see cref="OnSignInRequested"/> has no delegate. Defaults to "/auth/login".</summary>
        [Parameter] public string LoginPath { get; set; } = "/auth/login";

        /// <summary>Sign-out route used when <see cref="OnSignOutRequested"/> has no delegate. Defaults to "/auth/logout".</summary>
        [Parameter] public string LogoutPath { get; set; } = "/auth/logout";

        /// <summary>
        /// OAuth error message to surface (e.g. forwarded from the host page's <c>?oauth_error=</c> query parameter).
        /// </summary>
        [Parameter] public string? OAuthError { get; set; }

        // ── Private state ────────────────────────────────────────────────────
        private Passage? _passage;
        private bool _loading;
        private string? _error;
        private CancellationTokenSource? _cts;

        private bool _isSignedIn;
        private string? _userName;
        private string? _copyright;

        // ── Lifecycle ────────────────────────────────────────────────────────
        protected override async Task OnInitializedAsync()
        {
            State.OnStateChanged += OnStateChangedHandler;
            await CheckSignInAsync();
        }

        // Re-check sign-in on the first interactive render so a token stored during
        // the OAuth callback round-trip is visible even when prerender and circuit
        // DI scopes differ.
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
                await CheckSignInAsync();
        }

        private async Task CheckSignInAsync()
        {
            var token = await TokenProvider.GetTokenAsync();
            var signedIn = token is not null && !token.IsExpired();
            var userName = token?.GetDisplayIdentity();

            if (signedIn != _isSignedIn || userName != _userName)
            {
                _isSignedIn = signedIn;
                _userName = userName;
                await InvokeAsync(StateHasChanged);
            }
        }

        // ── Reading ──────────────────────────────────────────────────────────
        private bool CanRead =>
            State.SelectedVersion is not null &&
            State.SelectedBook is not null &&
            State.SelectedChapter is not null &&
            State.SelectedVerseStart is not null;

        private async Task ReadPassageAsync()
        {
            if (!CanRead) return;

            _cts?.Cancel();
            _cts = new CancellationTokenSource();

            _loading = true;
            _error = null;
            _passage = null;

            try
            {
                var reference = BuildReference();

                _passage = await PassageService.GetPassageAsync(
                    State.SelectedVersion!.Id,
                    reference,
                    new PassageRequestOptions { Format = Format },
                    _cts.Token);

                _copyright = State.SelectedVersion.Copyright;

                if (OnPassageLoaded.HasDelegate)
                    await OnPassageLoaded.InvokeAsync(_passage);
            }
            catch (OperationCanceledException)
            {
                // Superseded by a newer request — ignore.
            }
            catch (Exception ex)
            {
                _error = $"Could not load passage: {ex.Message}";
            }
            finally
            {
                _loading = false;
                await InvokeAsync(StateHasChanged);
            }
        }

        private Reference BuildReference()
        {
            var book = State.SelectedBook!.Usfm;
            var chapter = State.SelectedChapter!.Value;
            var verseStart = State.SelectedVerseStart!.Value;
            var verseEnd = State.SelectedVerseEnd ?? verseStart;

            return new Reference(
                book: book,
                chapter: chapter,
                verses: [new VerseRange(verseStart, verseEnd)]);
        }

        // ── Auth actions ─────────────────────────────────────────────────────
        private async Task SignInAsync()
        {
            if (OnSignInRequested.HasDelegate)
                await OnSignInRequested.InvokeAsync();
            else
                Nav.NavigateTo(LoginPath, forceLoad: true);
        }

        private async Task SignOutAsync()
        {
            if (OnSignOutRequested.HasDelegate)
                await OnSignOutRequested.InvokeAsync();
            else
                Nav.NavigateTo(LogoutPath, forceLoad: true);
        }

        // ── State change ─────────────────────────────────────────────────────
        private void OnStateChangedHandler()
            => InvokeAsync(() =>
            {
                _passage = null;
                _error = null;
                StateHasChanged();
            });

        public void Dispose()
        {
            State.OnStateChanged -= OnStateChangedHandler;
            _cts?.Cancel();
            _cts?.Dispose();
        }
    }
}
