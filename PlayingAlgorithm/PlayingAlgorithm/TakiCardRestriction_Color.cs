namespace PlayingAlgorithm
{
    public class TakiCardRestriction_Color: TakiCardRestriction
    {
        TakiColor color;
        public int[] maxCards;
        public TakiCardRestriction_Color(TakiColor color, int minValue, int maxValue ):
            base(minValue, maxValue )
        {
            this.color = color;
            maxCards = new int[4];
            for (int i = 0; i < 4; i++)
            {
                maxCards[i] = 100;
            }
        }
    }
}


