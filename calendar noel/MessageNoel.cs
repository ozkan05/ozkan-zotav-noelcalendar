using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace calendar_noel
{
    public class MessageNoel
    {
        public string Texte { get; set; }
        public string Image { get; set; }

        public MessageNoel(string texte, string image)
        {
            Texte = texte;
            Image = image;
        }
    }
}