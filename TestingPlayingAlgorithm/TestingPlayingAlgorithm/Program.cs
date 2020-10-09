using System;
using PlayingAlgorithm;

namespace TestingPlayingAlgorithm
{
    class Program
    {
        static void Main(string[] args)
        {
          
            Console.WriteLine("Hello World!");

            SpecialTakiCard.SpecialTakiCardType plus = new SpecialTakiCard.SpecialTakiCardType(SpecialTakiCard.SpecialTakiCardType.plus);
            ColoredSpecialTakiCard plusCard = new ColoredSpecialTakiCard(plus, new TakiColor(TakiColor.Color.red) );
            Console.WriteLine("my card is "+ plusCard);
            Console.ReadLine();
        }
    }
}
