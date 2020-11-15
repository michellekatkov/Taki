using System;


namespace PlayingAlgorithm
{
    public class MyRandom
    {
        /**
         * This is proxy class initially that we can replace fo speed
         */
        static Random rnd = new Random();
        static public int randomUniform( int max)
        {
            return rnd.Next(max);
        }
    }
}


