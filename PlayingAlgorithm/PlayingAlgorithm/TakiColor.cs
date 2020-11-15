using System;


namespace PlayingAlgorithm
{
    public class TakiColor
    {
        public TakiColor(Color _color)
        {
            myColor = _color;
        }
        public enum Color
        {
            red=0,
            green=1,
            blue=2,
            yellow=3,
            any
        }
        public Color myColor { get; private set; }
         
        public static readonly TakiColor red = new TakiColor(TakiColor.Color.red);
        public static readonly TakiColor green = new TakiColor(TakiColor.Color.green);
        public static readonly TakiColor blue = new TakiColor(TakiColor.Color.blue);
        public static readonly TakiColor yellow = new TakiColor(Color.yellow);
        public static readonly TakiColor any = new TakiColor(Color.any);
        public static readonly TakiColor[] colors = { red, green, blue, yellow };

        public static TakiColor GetRandomColor()
        {
            return colors[MyRandom.randomUniform(4)];
        }

        public bool isAnyColor()
        {
            return myColor == Color.any;
        }
        static public TakiColor FromString( string v)
        {
            switch( v)
            {
                case "red": return red;
                case "green": return green;
                case "yellow": return yellow;
                case "blue": return blue;
                case "": return any;
            }
            Console.WriteLine("unknown color");
            throw new Exception(" unknown color ");
        }
        public override string ToString()
        {
            switch (myColor)
            {
                case Color.red:
                    return "red";
                case Color.blue:
                    return "blue";
                case Color.green:
                    return "green";
                case Color.yellow:
                    return "yellow";
            }
            return " ";
        }
        public void CopyColorFrom(TakiColor color)
        {
            myColor = color.myColor;
        }
        public bool SameColor(TakiColor color)
        {
            return myColor == color.myColor || myColor == Color.any || color.myColor == Color.any;
        }
    }
}


