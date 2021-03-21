using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BARAKVPNSERVER.Helpers
{
    public static partial class Extensions
    {
        /// <summary>
        ///     A string extension method that queries if a not is empty.
        /// </summary>
        /// <param name="this">The @this to act on.</param>
        /// <returns>true if a not is empty, false if not.</returns>
        public static bool IsNotEmpty(this string @this)
        {
            return @this != null && @this != "";
        } 
        public static bool IsNotEmpty(this List<string> @this)
        {
            return @this != null && @this.Count > 0 ;
        }
    }
}
