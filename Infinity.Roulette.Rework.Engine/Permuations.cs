namespace Infinity.Roulette.Rework.Engine;

public class Permutations
{
    static List<List<int>> _currentList;

    static Permutations()
    {
        _currentList = new List<List<int>>(720);
    }

    public static List<List<int>> CurrentList
    {
        get { return _currentList; }
    }

    public static void Permut(int k, int n, int[] nums)
    {
        int i, j, tmp;

        /* when k > n we are done and should print */
        if (k <= n)
        {

            for (i = k; i <= n; i++)
            {
                tmp = nums[i];
                for (j = i; j > k; j--)
                {
                    nums[j] = nums[j - 1];
                }
                nums[k] = tmp;

                /* recurse on k+1 to n */
                Permut(k + 1, n, nums);

                for (j = k; j < i; j++)
                {
                    nums[j] = nums[j + 1];
                }
                nums[i] = tmp;
            }
        }
        else
        {
            var list = new List<int>();
            for (i = 1; i <= n; i++)
            {
                list.Add(nums[i]);
            }
            _currentList.Add(list);
        }
    }
}
