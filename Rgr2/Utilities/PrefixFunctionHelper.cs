namespace KMPStringSearchProject.Utilities
{
    public static class PrefixFunctionHelper
    {
        public static int[] ComputePrefixFunction(string pattern)
        {
            int[] prefix = new int[pattern.Length];
            int length = 0;
            int i = 1;

            while (i < pattern.Length)
            {
                if (pattern[i] == pattern[length])
                {
                    length++;
                    prefix[i] = length;
                    i++;
                }
                else
                {
                    if (length != 0)
                    {
                        length = prefix[length - 1];
                    }
                    else
                    {
                        prefix[i] = 0;
                        i++;
                    }
                }
            }

            return prefix;
        }
    }
}