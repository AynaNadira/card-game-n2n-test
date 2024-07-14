using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardGame
{
    public class Card
    {
        public int CardId { get; set; }
        public string? Value { get; set; }
        public char? Suit {  get; set; }
    }
}
