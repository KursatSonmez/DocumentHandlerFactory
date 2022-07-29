using System.IO;
using System.Linq;

namespace DocumentHandlerFactory.Extensions
{
    public static class BetterPath
    {
        public static string Combine(params string[] paths)
        {
            char directorySeparator = Path.DirectorySeparatorChar;

            string[] arr = paths
                .Select((x, i) =>
                {
                    string replaced = x.Replace('/', directorySeparator);

                    if (i > 0 && replaced.StartsWith(directorySeparator))
                        replaced = replaced[1..];

                    return replaced;
                })
                .ToArray();

            return Path.Combine(arr);
        }

        public static bool IsValiFilePath(this string path)
        {
            bool isValid;

            try
            {
                string fullPath = Path.GetFullPath(path);

                isValid = path.IndexOfAny(Path.GetInvalidPathChars()) == -1;
            }
            catch
            {
                isValid = false;
            }

            return isValid;
        }
    }
}
