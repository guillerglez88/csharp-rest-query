using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static CsharpMultimethod.Multi;
using static CsharpMultimethod.PropBasedMultiExtensions;
using CsharpDataOriented;
using static CsharpDataOriented.Sequence;
using System.Web;

namespace RestQuery;

public class UrlRoute
{
    private static Func<string, Component, Component> match;

    static UrlRoute()
    {
        var matchMulti = DefMulti(
            contract: ((string token, Component component) args) => default(Component),
            dispatch: ((string token, Component component) args) => DispatchByProp<Component>(
                interestProps: new[] { nameof(Component.Values) }).Invoke(args.component));

        matchMulti
            .DefMethod(nameof(Component.Values), (args) => MatchValue(args.token, args.component))
            .DefDefault((_dispatchingVal, args) => MatchAll(args.token, args.component));

        match = (token, component) => matchMulti.Invoke((token, component));
    }

    public static Route MatchRoute(Route route, string urlStr)
    {
        var uri = new Uri(urlStr);
        var urlSegs = SanitizeSegs(uri.Segments);
        var qs = HttpUtility.ParseQueryString(uri.Query);
        var lookup = route.Components.ToLookup(c => c.Source);
        var routeSegs = Get(lookup, ComponentSource.Path);
        var routeParams = Get(lookup, ComponentSource.Query);

        var segments = routeSegs
            .Zip(urlSegs)
            .Select(pair => match(pair.Second, pair.First))
            .ToArray();

        var parameters = qs.AllKeys
            .Select(k => new Component(
                Source: ComponentSource.Query,
                Name: k,
                Values: qs.GetValues(k),
                Matched: true))
            .ToArray();

        var matches = segments
            .Concat(parameters)
            .All(seg => seg.Matched);

        return route with
        {
            Components = segments.Concat(parameters),
            Matched = matches
        };
    }

    private static IEnumerable<Component> Get(
        ILookup<ComponentSource, Component> lookup,
        ComponentSource source)
        => lookup.Contains(source) ? lookup[source] : Enumerable.Empty<Component>();

    private static Component MatchAll(string token, Component component)
    {
        return component with
        {
            Matched = true,
            Values = new[] { token }
        };
    }

    private static Component MatchValue(string token, Component component)
    {
        var matches = (component.Values ?? new[] { token })
            .Where(value => Equals(token, value))
            .ToList();

        return component with
        {
            Matched = matches.Any(),
            Values = matches
        };
    }

    private static IEnumerable<string> SanitizeSegs(IEnumerable<string> urlSegs) => urlSegs
        .Select(s => s.Trim('/'))
        .Where(s => !string.IsNullOrWhiteSpace(s));
}
