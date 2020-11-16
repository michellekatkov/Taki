namespace PlayingAlgorithm
{
    public class TakiCardRestriction_Color
    {
        TakiColor color;
        public int[] maxCards;
        public TakiCardRestriction_Color( )
        {            
            maxCards = new int[4];
            for (int i = 0; i < 4; i++)
            {
                maxCards[i] = 100;
            }
        }
    }
}


