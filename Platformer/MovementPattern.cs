﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Platformer
{
    class MovementPattern //Movement pattern made up of three same-sized lists (xvel, yvel, and time for each "section" of movement) plus total cycle time
    {
        public List<double> xVList;
        public List<double> yVList;

        public MovementPattern(List<double> xVList, List<double> yVList)
        {
            this.xVList = xVList;
            this.yVList = yVList;


        }
    }
}
