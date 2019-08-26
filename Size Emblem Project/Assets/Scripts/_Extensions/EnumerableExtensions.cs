using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizeEmblem.Scripts.Extensions
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// From: https://stackoverflow.com/questions/1211608/possible-to-iterate-backwards-through-a-foreach/1211626#1211626
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        public static IEnumerable<T> FastReverse<T>(this IList<T> items)
        {
            for (var i = items.Count - 1; i >= 0; i--)
                yield return items[i];
        }
    }
}
