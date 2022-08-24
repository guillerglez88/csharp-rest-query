# rest-query

Exploring dynamic query building from http query string

## Example

```csharp
const string url = "http://www.domain.com/Patient?_id=087b36e0-dc10-44a5-8b65-8794d10d7c0f";

var query = ParseUrlExp(url).Compile(
    contract: (string resourceType, string id) => default(bool));

query("Patient", "087b36e0-dc10-44a5-8b65-8794d10d7c0f"); // => true
query("Practitioner", "087b36e0-dc10-44a5-8b65-8794d10d7c0f"); // => false
query("Patient", "f3fd2e68-ddb4-4fe0-a9e0-6b72a00f1567"); // => false
```

The url following url:

```
http://www.domain.com/Patient?_id=087b36e0-dc10-44a5-8b65-8794d10d7c0f
```

generates this expression tree:

```json
["fn", 
    ["resource-type", "{{typeof(string)}}",
     "_id", "{{typeof(string)}}"], 
    ["and",
        ["eq", 
            ["const", "Patient"], 
            ["param", "resource-type"]],
        ["eq", 
            ["const", "087b36e0-dc10-44a5-8b65-8794d10d7c0f"], 
            ["param", "_id"]]]]
```

Which can be compiled to a function like this:

```
query: (resourceType: string, id: string) -> bool
```

Or translated to a linq expression and converted into a SQL query