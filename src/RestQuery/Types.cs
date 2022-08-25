using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestQuery;

public record Route(
    IEnumerable<Segment> Segments,
    );

public record Segment(
    string? Name = null,
    string? Value = null,
    string? Default = null,
    string? Regex = null);
