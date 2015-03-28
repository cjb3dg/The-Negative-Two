using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Platformer
{
    class Boss : Invertible
    {
        public List<MovementPattern> mPatternList; //A Boss has a list of possible movement patterns it can switch between on the fly
        public int curHP;
        public int maxHP;
    }
}
