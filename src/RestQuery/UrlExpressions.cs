using CsharpMacros;
using System.Web;
using System.Linq;

using static CsharpMacros.Exp;
using static CsharpDataOriented.BasicFuncs;

namespace RestQuery;

public class UrlExpressions
{
    public static Exp BuildRouteExp(Route route)
    {
        var args = E(route.Components.SelectMany(f => E(f.Name, typeof(string))).ToArray());

        var predicate = route.Components
            .Select(cmp => cmp.Values
                .Select(value => 
                    E("eq", 
                        E("const", value), 
                        E("param", cmp.Name)))
                .Aggregate((acc, curr) => 
                    E("or", acc, curr)))
            .Aggregate((acc, curr) => 
               E("and", acc, curr));

        var exp = E("fn", args, predicate);

        return exp;
    }
}
