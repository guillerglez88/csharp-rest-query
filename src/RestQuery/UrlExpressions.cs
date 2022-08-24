using CsharpMacros;
using System.Web;
using System.Linq;

using static CsharpMacros.Exp;
using static CsharpDataOriented.BasicFuncs;

namespace RestQuery;

public class UrlExpressions
{
    public static Exp ParseUrlExp(string url)
    {
        var uri = new Uri(url);
        var resourceType = uri.Segments.Last();
        var qs = HttpUtility.ParseQueryString(uri.Query);

        var pathFields = new[] { new { name = "resource-type", values = new[]{ resourceType } } };
        var qsFields = qs.AllKeys.Select(k => new { name = k, values = qs.GetValues(k) });
        var fields = pathFields.Concat(qsFields).ToList();

        var args = E(fields.SelectMany(f => E(f.name, typeof(string))).ToArray());

        var predicate = fields
            .Select(field => field.values
                .Select(value => 
                    E("eq", 
                        E("const", value), 
                        E("param", field.name)))
                .Aggregate((acc, curr) => 
                    E("or", acc, curr)))
            .Aggregate((acc, curr) => 
               E("and", acc, curr));

        var exp = E("fn", args, predicate);

        return exp;
    }
}
