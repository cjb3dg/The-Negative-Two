﻿﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace The_Negative_One
{
    class MovementPattern //Movement pattern made up of three same-sized lists (xvel, yvel, and time for each "section" of movement) plus total cycle time
    {
        public List<double> xVList;
        public List<double> yVList;
        public List<bool> shootList;

        public MovementPattern(List<double> xVList, List<double> yVList, List <bool> shootList)
        {
            this.xVList = xVList;
            this.yVList = yVList;
            this.shootList = shootList;

        }
    }
}
