namespace Infinity.Services.Interfaces;

public interface ICountColorService
{
    public void SetOtherStyle(object other);
    public object? GetOtherStyle();
    public void ResetOtherStyle();
}