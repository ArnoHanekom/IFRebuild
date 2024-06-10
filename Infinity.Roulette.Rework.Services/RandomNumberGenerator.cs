namespace Infinity.Roulette.Rework.Services;

public static class RandomNumberGenerator
{
    private static readonly Random rand = new();
    private static readonly object synclock = new();

    public static int NextRandomNumber()
    {
        lock (synclock)
            return rand.Next(1, 37);
    }

    public static async Task<int> NextRandomNumberAsync(CancellationToken ct = default)
    {
        try
        {
            return await Task.Run(NextRandomNumber, ct);
        }
        catch
        {
            return NextRandomNumber();
        }
    }
}