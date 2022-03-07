using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Epidemic
{
    class People
    {
        public double X, Y;
        public string State;
        public Vector velocity, acceleration, border;
        public Point center;
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
