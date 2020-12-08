using System;

namespace YonderSharp
{
    public class MyRandom : Random
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="maxValue">max value that can be returned by this Method (excluding)</param>
        /// <returns></returns>
        public int GetInt(int maxValue)
        {
            return ((int)(NextDouble() * 997 * 991)) % (maxValue);
        }


        public Boolean GetBoolean()
        {
            return ((int)(NextDouble() * 997 * 991)) % 2 == 0;
        }


    }
}
