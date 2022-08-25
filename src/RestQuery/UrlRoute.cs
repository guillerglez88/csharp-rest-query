using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static CsharpMultimethod.Multi;
using CsharpDataOriented;

namespace RestQuery;

public class UrlRoute
{
    private static Func<string, Seq, Seq> matchSegment;

    static UrlRoute()
    {
        var matchSegmentMulti = DefMulti(
            contract: ((string urlSeg, Seq routeSeg) args) => default(Seq),
            dispatch: ((string urlSeg, Seq routeSeg) args) => args.routeSeg
                .Partition(size: 2)
                .Select(part => part.Nth<string>(0))
                .FirstOrDefault(prop => new[] { "value", "regex" }.Contains(prop)));

        matchSegmentMulti
            .DefMethod("value", (args) => MatchValue(args.urlSeg, args.routeSeg));

        matchSegment = (urlSeg, routeSeg) => matchSegmentMulti.Invoke((urlSeg, routeSeg));
    }

    public static Seq MatchRoute(Seq route, string urlStr)
    {
        var uri = new Uri(urlStr);

        var segments = route.Get<Seq>(new[] { "segments" });
    }

    private static Seq MatchValue(string urlSeg, Seq routeSeg)
    {
    }
}
