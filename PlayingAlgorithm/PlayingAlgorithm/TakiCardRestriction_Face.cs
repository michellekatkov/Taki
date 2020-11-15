namespace PlayingAlgorithm
{
    public class TakiCardRestriction_Face : TakiCardRestriction
    {
        TakiFace face;
        public  int[] maxCards;
        public TakiCardRestriction_Face(TakiFace face, int minValue, int maxValue) :
            base(minValue, maxValue)
        {
            this.face = face;
            maxCards = new int[11];
            for (int i = 1; i < 10; i++)
            {
                maxCards[i] = 100;
            }
        }
    }
}


