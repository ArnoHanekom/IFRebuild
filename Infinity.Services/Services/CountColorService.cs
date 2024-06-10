using Infinity.Services.Interfaces;
using System.Diagnostics;

namespace Infinity.Services.Services;

public class CountColorService : ICountColorService
{
    private object? otherCountStyle { get; set; } = default!;

    public void SetOtherStyle(object other)
    {
        otherCountStyle = other;
    }

    public object? GetOtherStyle() => otherCountStyle;

    public void ResetOtherStyle()
    {
        otherCountStyle = null;
    }
}