namespace ImeSense.Launchers.Belarus.Core.Models;

public sealed class LangNewsContent {
    public Locale? Locale { get; set; }
    public IEnumerable<NewsContent>? NewsContents { get; set; }

    public LangNewsContent() {
    }

    public LangNewsContent(Locale? locale, IEnumerable<NewsContent>? newsContents) {
        Locale = locale;
        NewsContents = newsContents;
    }
}
