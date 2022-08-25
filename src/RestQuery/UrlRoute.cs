using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static CsharpMultimethod.Multi;
using CsharpDataOriented;
using System.Text.RegularExpressions;

namespace RestQuery;

public class UrlRoute
{
    private static Func<string, Seq, Seq> matchSegment;

    static UrlRoute()
    {
        var matchSegmentMulti = DefMulti(
            contract: ((string urlSeg, Seq routeSeg) args) => default(Seq),
            dispatch: ((string urlSeg, Seq routeSeg) args) => args.routeSeg
                .Cast<Seq>()
                .Select(prop => prop.Nth<string>(0))
                .FirstOrDefault(prop => new[] { "value" }.Contains(prop)));

        matchSegmentMulti
            .DefMethod("value", (args) => MatchValue(args.urlSeg, args.routeSeg));

        matchSegment = (urlSeg, routeSeg) => matchSegmentMulti.Invoke((urlSeg, routeSeg));
    }

    public static Seq MatchRoute(Seq route, string urlStr)
    {
        var uri = new Uri(urlStr);

        var segments = route
            .Get("segments")
            .Cast<Seq>()
            .Select(item => item.Nth<Seq>(1))
            .Zip(uri.Segments.Where(s => !Equals("/", s)))
            .Select(pair => matchSegment(pair.Second, pair.First))
            .ToArray();

        var matches = segments.All(seg => seg.Get<bool>("matches"));

        return route.With(new { segments, matches });
    }

    private static Seq MatchValue(string urlSeg, Seq routeSeg)
    {
        var value = routeSeg.Get<string>("value");
        var matches = Equals(urlSeg, value);

        var validated = routeSeg.With(new { matches });

        return validated;
    }
}
