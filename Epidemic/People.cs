using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epidemic
{
    class People
    {
        public double X, Y;
        public string State;
        public People()
        {

        }
        public People(double x,double y,string s)
        {
            X = x;
            Y = y;
            State = s;
        }
    }
}
