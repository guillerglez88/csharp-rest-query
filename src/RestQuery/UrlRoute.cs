using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static CsharpMultimethod.Multi;
using CsharpDataOriented;
using static CsharpDataOriented.Sequence;
using System.Web;

namespace RestQuery;

public class UrlRoute
{
    private static Func<string, Seq, Seq> match;

    static UrlRoute()
    {
        var matchMulti = DefMulti(
            contract: ((string token, Seq routeSeg) args) => default(Seq),
            dispatch: ((string token, Seq routeSeg) args) => args.routeSeg
                .Cast<Seq>()
                .Select(prop => prop.Nth<string>(0))
                .FirstOrDefault(prop => new[] { "value" }.Contains(prop)));

        matchMulti
            .DefMethod("value", (args) => MatchValue(args.token, args.routeSeg))
            .DefDefault((_dispatchingVal, args) => MatchAll(args.token, args.routeSeg));

        match = (urlSeg, routeSeg) => matchMulti.Invoke((urlSeg, routeSeg));
    }

    public static Seq MatchRoute(Seq route, string urlStr)
    {
        var uri = new Uri(urlStr);
        var urlSegs = SanitizeSegs(uri.Segments);
        var qs = HttpUtility.ParseQueryString(uri.Query);
        var routeSegs = route.Get("segments").OrEmpty().Cast<Seq>();
        var routeParams = route.Get("parameters").OrEmpty().Cast<Seq>();

        var segments = routeSegs
            .Select(item => item.Nth<Seq>(1))
            .Zip(urlSegs)
            .Select(pair => match(pair.Second, pair.First))
            .ToArray();

        var parameters = qs.AllKeys
            .Select(k => Seq(new { name = k, value = qs.GetValues(k), matches = true }))
            .ToArray();

        var matches = segments
            .Concat(parameters)
            .All(seg => seg.Get<bool>("matches"));

        return route.With(new { segments, matches , parameters });
    }

    private static Seq MatchAll(string token, Seq routeSeg)
    {
        var result = routeSeg.With(new
        {
            matches = true,
            value = token
        });

        return result;
    }

    private static Seq MatchValue(string token, Seq routeSeg)
    {
        var value = routeSeg.Get<string>("value");
        var matches = Equals(token, value);

        var validated = routeSeg.With(new { matches });

        return validated;
    }

    private static IEnumerable<string> SanitizeSegs(IEnumerable<string> urlSegs) => urlSegs
        .Select(s => s.Trim('/'))
        .Where(s => !string.IsNullOrWhiteSpace(s));
}
