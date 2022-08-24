using CsharpMacros;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static CsharpMacros.Exp;

using static RestQuery.UrlExpressions;

namespace RestQuery.UnitTests;

public class BasicFieldsTests
{
    public BasicFieldsTests()
    {
        Module.InitializeAllModules();
    }

    [Fact]
    public void CanMatchId()
    {
        const string url = "http://www.domain.com" +
            "/Patient" +
            "?_id=087b36e0-dc10-44a5-8b65-8794d10d7c0f";

        var query = ParseUrlExp(url).Compile(
            contract: (string resourceType, string id) => default(bool));

        Assert.True(query("Patient", "087b36e0-dc10-44a5-8b65-8794d10d7c0f"));
        Assert.False(query("Practitioner", "087b36e0-dc10-44a5-8b65-8794d10d7c0f"));
        Assert.False(query("Patient", "f3fd2e68-ddb4-4fe0-a9e0-6b72a00f1567")); 
    }
}
