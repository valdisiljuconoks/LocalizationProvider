using System;
using System.Linq;

namespace DbLocalizationProvider.Internal
{
    internal static class StringExtensions
    {
        internal static string JoinNonEmpty(this string target, string separator, params string[] args)
        {
            if(target == null)
                throw new ArgumentNullException(nameof(target));

            return string.IsNullOrEmpty(target)
                       ? string.Empty
                       : string.Join(separator, new[] { target }.Union(args.Where(s => !string.IsNullOrEmpty(s)).ToArray()));
        }
    }
}
