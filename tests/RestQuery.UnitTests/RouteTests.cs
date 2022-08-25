using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static CsharpDataOriented.SequenceModule;
using static RestQuery.UrlRoute;

namespace RestQuery.UnitTests;

public class RouteTests
{
    [Fact]
    public void CanParseSimpleRoute()
    {
        var route = Seq(
            "segments", Seq(
                Seq("name", "resource-type",
                    "value", "Patient")));

        var result = MatchRoute(route, "http://www.domain.com/Patient");

        Assert.Contains();
    }
}
