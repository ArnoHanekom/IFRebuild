namespace Infinity.Roulette.Statics;

public static class ResourceHelper
{
    public static string KeyFromResource(object lookupResource)
    {
        foreach (var dictionary in App.Current.Resources.MergedDictionaries)
        {
            foreach (var itemKey in dictionary.Keys)
            {
                if (dictionary[itemKey] == lookupResource)
                {
                    return itemKey.ToString()!;
                }
            }
        }

        return null!;
    }
}