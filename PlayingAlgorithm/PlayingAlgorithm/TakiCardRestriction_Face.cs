namespace PlayingAlgorithm
{
    public class TakiCardRestriction_Face 
    {       
        public  int[] maxCards;
        public TakiCardRestriction_Face()            
        {            
            maxCards = new int[11];
            for (int i = 1; i < 10; i++)
            {
                maxCards[i] = 100;
            }
        }
    }
}


