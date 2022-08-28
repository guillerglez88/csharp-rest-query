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
        var route = Seq(new {
            segments = new[] { new { 
                name = "resource-type",
                value = "Patient" } } });

        var result = MatchRoute(route, "http://www.domain.com/Patient");
        var matched = result.Get<bool>("matches");

        Assert.True(matched);
    }

    [Fact]
    public void CanMatchTwoSegmentsRoute()
    {
        var route = Seq(new {
            segments = new object [] { 
                new { name = "resource-type",
                      value = "Patient" },
                new { name = "id" } } }); 

        var result = MatchRoute(route, "http://www.domain.com/Patient/818d2646");
        var matched = result.Get<bool>("matches");
        var patId = result.Get<string>("segments", "1", "value");

        Assert.True(matched);
        Assert.Equal("818d2646", patId);
    }

    [Fact]
    public void CanCaptureAllRoute()
    {
        var route = Seq(new {
            segments = new object [] { 
                new { name = "resource-type" }}}); 

        var result = MatchRoute(route, 
            "http://www.domain.com/Patient" +
                "?active=true" +
                "&gender=male");
        var matched = result.Get<bool>("matches");
        var resType = result.Get<string>("segments", "0", "value");
        var active = result.Get<string>("parameters", "0", "value", "0");
        var gender = result.Get<string>("parameters", "1", "value", "0");

        Assert.True(matched);
        Assert.Equal("Patient", resType);
        Assert.Equal("true", active);
        Assert.Equal("male", gender);
    }
}
