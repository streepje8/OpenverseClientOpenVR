//-------------------------------
//Util
//Utility functions used by other scripts
//
//Author: streep
//Creation Date: 12-08-2022
//--------------------------------
namespace Openverse.SupportSystems
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    public class Util
    {
        public static byte[] MergeArrays(byte[] a, byte[] b)
        {
            byte[] array1 = a;
            byte[] array2 = b;
            byte[] newArray = new byte[array1.Length + array2.Length];
            Array.Copy(array1, newArray, array1.Length);
            Array.Copy(array2, 0, newArray, array1.Length, array2.Length);
            return newArray;
        }

        public static IEnumerable<IEnumerable<byte>> SplitArray(byte[] array, int maxElements)
        {
            for (var i = 0; i < (float)array.Length / maxElements; i++)
            {
                yield return array.Skip(i * maxElements).Take(maxElements);
            }
        }
    }
}