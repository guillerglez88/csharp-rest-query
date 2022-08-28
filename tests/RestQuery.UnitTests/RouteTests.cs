using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CsharpDataOriented;
using static CsharpDataOriented.Sequence;
using static RestQuery.UrlRoute;

namespace RestQuery.UnitTests;

public class RouteTests
{
    [Fact]
    public void CanMatchSimpleRoute()
    {
        var route = new Route(
            Components: new[] { new Component(
                Source: ComponentSource.Path,
                Name: "resource-type",
                Values: new[]{ "Patient" } ) });

        var result = MatchRoute(route, "http://www.domain.com/Patient");

        Assert.True(result.Matched);
    }

    [Fact]
    public void CanMatchTwoSegmentsRoute()
    {
        var route = new Route(
            Components: new[] {
                new Component (
                    Source: ComponentSource.Path,
                    Name: "resource-type",
                    Values: new[]{ "Patient" }),
                new Component (
                    Source: ComponentSource.Path,
                    Name: "id" ) });

        var result = MatchRoute(route, "http://www.domain.com/Patient/818d2646");

        Assert.True(result.Matched);
        Assert.Contains("818d2646", result.Components.Skip(1).First().Values);
    }

    [Fact]
    public void CanCaptureAllRoute()
    {
        var route = new Route(
            Components: new[] {
                new Component (
                    Source: ComponentSource.Path,
                    Name: "resource-type" )});

        var result = MatchRoute(route,
            "http://www.domain.com/Patient" +
                "?active=true" +
                "&gender=male");

        Assert.True(result.Matched);
        Assert.Contains("Patient", result.Components.First().Values);
        Assert.Contains("true", result.Components.Skip(1).First().Values);
        Assert.Contains("male", result.Components.Skip(2).First().Values);
    }
}
