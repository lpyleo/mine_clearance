using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mine_clearance
{
    public class LevelArgs : EventArgs
    {
        public int gear;

        public int Gear
        {
            get { return gear; }
            set { gear = value; }
        }

    }
}
