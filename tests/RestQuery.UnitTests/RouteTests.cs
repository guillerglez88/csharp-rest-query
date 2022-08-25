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
    public void CanParseSimpleRoute()
    {
        var route = Seq(new {
            segments = new[] { new { 
                name = "resource-type",
                value = "Patient" } } });

        var result = MatchRoute(route, "http://www.domain.com/Patient");
        var matched = result.Get<bool>("matches");

        Assert.True(matched);
    }
}
