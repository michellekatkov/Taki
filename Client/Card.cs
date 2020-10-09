using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Card
    {
        private Dictionary<string, string> card;

        public Card(string type, string color, string value)
        {
            this.card = new Dictionary<string, string>();
            this.card.Add("type", type);
            this.card.Add("color", color);
            this.card.Add("value", value);
        }

        public Card(string type, string color) // card without value
        {
            this.card = new Dictionary<string, string>();
            this.card.Add("type", type);
            this.card.Add("color", color);
            this.card.Add("value", "");
        }

        public string GetCardType()
        {
            return this.card["type"];
        }

        public string GetColor()
        {
            return this.card["color"];
        }

        public string GetValue()
        {
            return this.card["value"];
        }

        public Dictionary<string,string> GetCardDict()
        {
            return this.card;
        }

    }
}
