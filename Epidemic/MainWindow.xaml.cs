using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Epidemic
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        People[] people;
        private const int aSquare = 25,QuantityOfPeople = 10, DiameterOfCircle = 25, BorderThickness1 = 4, maxSpeed = 100, accelerationRange = 20, velocityRange = 50, BorderOffset = 40, TotalTimeOfInfectionInSeconds = 5, SocialDistancingRadius = 100;
        private int InfectionRadius = DiameterOfCircle + 30;
        private double InfectionChance = 0.10;
        private Random rnd;
        public bool Manual = false;
        List<Point> PossibleCollisionsList = new List<Point>();
        Stopwatch stopwatch = new Stopwatch();
        Stopwatch time = new Stopwatch();
        List<int> InfectedList = new List<int>();
        List<int> HealthyList = new List<int>();
        public int frames;
        public MainWindow()
        {
            InitializeComponent();
            people = new People[QuantityOfPeople];
            rnd = new Random();
            stopwatch.Start();
            time.Start();
            InitPeople();
            if (!Manual)
            {
                CompositionTarget.Rendering += UpdateFrame;
            }
        }

        private void UpdateFrame(object sender, EventArgs e)
        {
            Field.Children.Clear();
            UpdateAll();
            UpdateInfectedPeople();
            UpdateRemovedPeople();
            UpdateSocialDistancing();
            Move(stopwatch.Elapsed);
            DrawAllPeople();
            DrawVectors(false, false, false, true);
            DrawBorder();
            stopwatch.Restart();
            frames++;
        }
        private void nextFrame()
        {
            Field.Children.Clear();
            UpdateAll();
            UpdateInfectedPeople();
            UpdateRemovedPeople();
            UpdateSocialDistancing();
            Move(stopwatch.Elapsed);
            DrawAllPeople();
            DrawVectors(false, false, false, true);
            DrawBorder();
            stopwatch.Restart();
            frames++;
        }
        public void Move(TimeSpan ts)
        {
            for(int i = 0;i < QuantityOfPeople; i++)
            {
                if (Vector.Add(Vector.Multiply(people[i].acceleration, ts.TotalSeconds), people[i].velocity).Length <= maxSpeed)
                {
                    people[i].velocity = Vector.Add(Vector.Multiply(people[i].acceleration, ts.TotalSeconds), people[i].velocity);
                }
                if (Vector.Add(Vector.Multiply(people[i].border, ts.TotalSeconds), people[i].velocity).Length <= maxSpeed)
                {
                    people[i].velocity = Vector.Add(Vector.Multiply(people[i].border, ts.TotalSeconds), people[i].velocity);
                }
                if (Vector.Add(Vector.Multiply(people[i].socialdiastancing, ts.TotalSeconds), people[i].velocity).Length <= maxSpeed)
                {
                    people[i].velocity = Vector.Add(Vector.Multiply(people[i].socialdiastancing, ts.TotalSeconds), people[i].velocity);
                }
                people[i].X = people[i].X + people[i].velocity.X * ts.TotalSeconds;
                people[i].Y = people[i].Y + people[i].velocity.Y * ts.TotalSeconds;
            }
        }
        public void UpdateRemovedPeople()
        {
            for(int i = 0;i < InfectedList.Count; i++)
            {
                if(people[InfectedList[i]].startTime.TotalSeconds + TotalTimeOfInfectionInSeconds <= time.Elapsed.TotalSeconds)
                {
                    people[InfectedList[i]].State = "В";
                }
            }
        }
        private void UpdateAll()
        {
            InfectedList.Clear();
            HealthyList.Clear();
            for (int i = 0; i < QuantityOfPeople; i++)
            {
                UpdateCenterPoint(i);
                UpdateVector(i);
                UpdateList(i);
                if (people[i].X > Field.Width || people[i].X < -10 || people[i].Y > Field.Height || people[i].Y < -10) {
                    people[i].X = 100;
                    people[i].Y = 100;
                }
            }
        }
        private People[] Sorting()
        {
            People[] Sorted = people;
            for (int i = 0; i < QuantityOfPeople - 1; i++)
            {
                for (int j = 0; j < QuantityOfPeople - i - 1; j++)
                {
                    if(Sorted[j].X > Sorted[j + 1].X)
                    {
                        People temp = Sorted[j];
                        Sorted[j] = Sorted[j + 1];
                        Sorted[j + 1] = temp;
                    }
                }
            }
            return Sorted;
        }
        private void UpdateSocialDistancing()
        {
            people = Sorting();
            double first;
            double last;
            for(int i = 0; i < QuantityOfPeople; i++)
            {
                first = people[i].X - SocialDistancingRadius;
                last = people[i].X + DiameterOfCircle + SocialDistancingRadius;
                for (int j = 0; people[i + j].X + SocialDistancingRadius > last && people[i + j].X - SocialDistancingRadius < first && i + j < QuantityOfPeople; j++)
                {
                    PossibleCollisionsList.Add(new Point(i, i + j));
                }
            }
            for (int i = 0; i < PossibleCollisionsList.Count; i++)
            {
                People tempor1 = people[(int)PossibleCollisionsList[i].X];
                People tempor2 = people[(int)PossibleCollisionsList[i].Y];
                double coefficient = maxSpeed * maxSpeed / Math.Pow(Distance(tempor1.X, tempor1.Y, tempor2.X, tempor2.Y), 2);
                Vector temp = new Vector((tempor1.X - tempor2.X) * coefficient,(tempor1.Y - tempor2.Y) * coefficient);
                tempor1.socialdiastancing = Vector.Add(tempor1.socialdiastancing, temp);
            }
        }
        public void UpdateVector(int a)
        {
            if(frames % 70 == 0)
            {
                people[a].acceleration = new Vector(rnd.Next(-accelerationRange, accelerationRange), rnd.Next(-accelerationRange, accelerationRange));
            }
            people[a].border = new Vector((maxSpeed * maxSpeed / Math.Pow(people[a].X, 2)) - (maxSpeed * maxSpeed / Math.Pow(Field.Width - BorderThickness1 - people[a].X + DiameterOfCircle / 2 - BorderOffset, 2)),
                (maxSpeed * maxSpeed / Math.Pow(people[a].Y, 2)) - (maxSpeed * maxSpeed / Math.Pow(Field.Height - BorderThickness1 - people[a].Y + DiameterOfCircle - BorderOffset, 2)));
            if (people[a].velocity.Length > maxSpeed * 2)
            {
                people[a].velocity = new Vector(0, 0);
            }
        }
        public void UpdateList(int a)
        {
            if(people[a].State == "З")
            {
                InfectedList.Add(a);
            }
            if(people[a].State == "НЗ")
            {
                HealthyList.Add(a);
            }
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            nextFrame();
        }

        public double Distance(Point p1,Point p2)
        {
            return Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
        }
        public double Distance(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
        }
        public void UpdateInfectedPeople()
        {
            if(frames % 10 == 0)
            {
                for (int i = 0; i < InfectedList.Count; i++)
                {
                    for (int j = 0; j < HealthyList.Count; j++)
                    {
                        if (Distance(people[InfectedList[i]].center, people[HealthyList[j]].center) < DiameterOfCircle + InfectionRadius)
                        {
                            if (rnd.Next(0, 100) <= InfectionChance * 100)
                            {
                                people[HealthyList[j]].State = "З";
                                people[HealthyList[j]].startTime = time.Elapsed;
                            }
                        }
                    }
                }
            }
        }
        public void UpdateCenterPoint(int a)
        {
           people[a].center = new Point(people[a].X + DiameterOfCircle / 2, people[a].Y + DiameterOfCircle / 2);
        }
        private void DrawBorder()
        {
            Rectangle border = new Rectangle()
            {
                Stroke = new SolidColorBrush(Color.FromArgb(255,200,200,200)),
                StrokeThickness = BorderThickness1,
                Width = Field.Width - 2*BorderThickness1,
                Height = Field.Height - 2*BorderThickness1
            };
            Field.Children.Add(border);
        }
        public void DrawVectors(bool drawacc, bool drawvel, bool drawbor, bool drawsocdis)
        {
            for(int i = 0;i < QuantityOfPeople; i++)
            {
                double startx = people[i].center.X, starty = people[i].center.Y;
                if (drawacc)
                {
                    Line acc = new Line();
                    acc.X1 = startx;
                    acc.Y1 = starty;
                    acc.X2 = startx + people[i].acceleration.X;
                    acc.Y2 = starty + people[i].acceleration.Y;
                    acc.Stroke = Brushes.Red;
                    Field.Children.Add(acc);
                }
                if (drawvel)
                {
                    Line vel = new Line();
                    vel.X1 = startx;
                    vel.Y1 = starty;
                    vel.X2 = startx + people[i].velocity.X;
                    vel.Y2 = starty + people[i].velocity.Y;
                    vel.Stroke = Brushes.Yellow;
                    Field.Children.Add(vel);
                }
                if (drawbor)
                {
                    Line bor = new Line();
                    bor.X1 = startx;
                    bor.Y1 = starty;
                    bor.X2 = startx + people[i].border.X;
                    bor.Y2 = starty + people[i].border.Y;
                    bor.Stroke = Brushes.Green;
                    Field.Children.Add(bor);
                }
                if (drawsocdis)
                {
                    Line socdis = new Line();
                    socdis.X1 = startx;
                    socdis.Y1 = starty;
                    socdis.X2 = startx + people[i].socialdiastancing.X;
                    socdis.Y2 = starty + people[i].socialdiastancing.Y;
                    socdis.Stroke = Brushes.Blue;
                    Field.Children.Add(socdis);
                }
            }
        }
        public void DrawPeople(double x, double y, string state)
        {
            Ellipse man = new Ellipse();
            man.Width = man.Height = DiameterOfCircle;
            man.Margin = new Thickness(x,y,0,0);
            switch (state){
                case "НЗ":
                    {
                        man.Fill = new SolidColorBrush(Color.FromArgb(255, 87, 175, 250));
                        break;
                    }
                case "З":
                    {
                        man.Fill = new SolidColorBrush(Color.FromArgb(255, 250, 71, 56));
                        break;
                    }
                case "В":
                    {
                        man.Fill = new SolidColorBrush(Color.FromArgb(255, 100, 100, 100));
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
            Field.Children.Add(man);
        }
        public void DrawAllPeople()
        {
            for(int i = 0;i < QuantityOfPeople; i++)
            {
                DrawPeople(people[i].X, people[i].Y, people[i].State);
            }
        }
        public void InitPeople()
        {
            for(int i = 0;i < QuantityOfPeople;i++)
            {
                people[i] = new People(rnd.Next(BorderThickness1,(int)Field.Width - DiameterOfCircle - BorderThickness1),rnd.Next(BorderThickness1,(int)Field.Height) - DiameterOfCircle - BorderThickness1,"НЗ");
                people[i].velocity = new Vector(rnd.Next(-velocityRange,velocityRange), rnd.Next(-velocityRange,velocityRange));
            }
            people[0].State = "З";
            people[0].startTime = new TimeSpan(0, 0, 0);
        }
    }
}
