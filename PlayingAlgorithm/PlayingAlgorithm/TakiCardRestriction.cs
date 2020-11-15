namespace PlayingAlgorithm
{
    public class TakiCardRestriction
    {
        /* This is a base class to make restriction for taki collections*/
       int minValue, maxValue; // number of cards a given color collection can have

        public TakiCardRestriction(int minValue, int maxValue)
        {
            this.minValue = minValue;
            this.maxValue = maxValue;
        }
    }
}


