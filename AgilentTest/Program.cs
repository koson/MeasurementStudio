﻿using AgilentU2442A;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgilentTest
{
    class Program
    {
        static void Main(string[] args)
        {
            AgilentU2542A ag = new AgilentU2542A("Agilent", "asrdasd","USB0::0x0957::0x1718::TW52524501::0::INSTR");
            
        }
    }
}
