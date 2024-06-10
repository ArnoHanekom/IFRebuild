namespace Infinity.Engine;

public class SpinPhase
{
    public SpinPhase(int id, string key, int? startedBy)
    {
        Id = id;
        Key = key;
        StartedBy = startedBy;
    }
    public SpinPhase(string key, int? startedBy)
    {
        Key = key;
        StartedBy = startedBy;
    }
    public SpinPhase()
    {
        
    }
    public int Id { get; init; }
    public string Key { get; init; } = default!;
    public int? StartedBy { get; init; } = null!;
    public bool Started { get; set; }
    public bool Ended { get; set; }
}