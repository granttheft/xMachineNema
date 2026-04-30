using System.Globalization;

namespace XMachine.Web.Middleware;

/// <summary>
/// Sets thread UI culture from the <c>xm_lang</c> cookie for formatting; translation keys use the same cookie in <see cref="Services.ITranslationService"/>.
/// </summary>
public static class XmLanguageCookieMiddleware
{
    private static readonly CultureInfo EnUs = new("en-US");
    private static readonly CultureInfo TrTr = new("tr-TR");
    private static readonly CultureInfo DeDe = new("de-DE");
    private static readonly CultureInfo PlPl = new("pl-PL");

    /// <summary>Registers middleware that applies culture from the <c>xm_lang</c> cookie.</summary>
    public static IApplicationBuilder UseXmLanguageCookie(this IApplicationBuilder app) =>
        app.Use(async (context, next) =>
        {
            var raw = context.Request.Cookies["xm_lang"]?.Trim().ToLowerInvariant();
            var culture = raw switch
            {
                "tr" => TrTr,
                "de" => DeDe,
                "pl" => PlPl,
                "en" => EnUs,
                _ => EnUs,
            };

            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
            await next.Invoke().ConfigureAwait(false);
        });
}
