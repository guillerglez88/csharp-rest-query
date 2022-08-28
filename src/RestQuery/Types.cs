using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestQuery;

public record Route(
    IEnumerable<Component> Components,
    bool Matched = false);

public record Component(
    ComponentSource Source,
    string Name,
    IEnumerable<string> Values = null,
    bool Matched = false);

public enum ComponentSource
{
    Path,
    Query,
    Headers
}
