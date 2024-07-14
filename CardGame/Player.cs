using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardGame.Models
{
    public class Player
    {
        public int PlayerId { get; set; }
        public string? Name { get; set; }
        public ICollection<Hand>? Hands { get; set; }
    }
}
