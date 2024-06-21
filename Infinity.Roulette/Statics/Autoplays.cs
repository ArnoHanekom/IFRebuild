namespace Infinity.Roulette.Statics;

public static class Autoplays
{
    public static List<int> Selection
    {
        get
        {
            List<int> source = [];
            for (int index = 10; index <= 100; index += 10)
                source.Add(index);
            for (int index = 120; index <= 200; index += 20)
                source.Add(index);
            for (int index = 250; index <= 350; index += 50)
                source.Add(index);
            for (int index = 400; index <= 1000; index += 100)
                source.Add(index);

            return source;
        }
    }
}