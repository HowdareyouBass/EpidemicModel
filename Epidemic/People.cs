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
        public Vector velocity, acceleration, border, socialdiastancing;
        public bool BeingMoved,Locked;
        public Point center,target;
        public TimeSpan startTime;
        public People()
        {

        }
        public People(double x,double y,string s)
        {
            X = x;
            Y = y;
            State = s;
            BeingMoved = false;
            Locked = false;
        }
        public static int CompareByX(People people1,People people2)
        {
            return people1.X.CompareTo(people2.X);
        }
    }
}
