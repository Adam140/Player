using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Player
{
    class AudioFile
    {
        public int id;
        public string path;
        public int clicked;
        private int i;
        private string p1;
        private int p2;


        public AudioFile(int id, string path, int clicked)
        {
            this.id = id;
            this.path = path;
            this.clicked = clicked;
        }

    }
}
