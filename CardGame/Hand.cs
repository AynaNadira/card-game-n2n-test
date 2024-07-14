using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardGame.Models
{
    public class Hand
    {
        public int HandId { get; set; }
        public int PlayerId { get; set; }
        public Player? Player { get; set; }

        public int CardId { get; set; }
        public Card? Card { get; set; }
    }
}
