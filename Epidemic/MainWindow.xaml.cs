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
        private const int QuantityOfPeople = 100, DiameterOfCircle = 15, BorderThickness1 = 4, maxSpeed = 100, accelerationRange = 20, velocityRange = 50, BorderOffset = 40;
        Random rnd;
        Stopwatch stopwatch = new Stopwatch();
        public int frames;
        public MainWindow()
        {
            InitializeComponent();
            people = new People[QuantityOfPeople];
            rnd = new Random();
            stopwatch.Start();
            InitPeople();
            CompositionTarget.Rendering += UpdateFrame;
        }

        private void UpdateFrame(object sender, EventArgs e)
        {
            Field.Children.Clear();
            UpdateCenterPoint();
            UpdateVectors();
            Move(stopwatch.Elapsed);
            DrawAllPeople();
            //DrawVectors();
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
                people[i].X = people[i].X + people[i].velocity.X * ts.TotalSeconds;
                people[i].Y = people[i].Y + people[i].velocity.Y * ts.TotalSeconds;
                //if (people[i].X > Field.Width)
                //{
                //    people[i].X = 0;
                //}
                //if (people[i].X < 0)
                //{
                //    people[i].X = Field.Width;
                //}
                //if (people[i].Y > Field.Height)
                //{
                //    people[i].Y = 0;
                //}
                //if (people[i].Y < 0)
                //{
                //    people[i].Y = Field.Height;
                //}
            }
        }
        public void UpdateVectors()
        {
            for(int i = 0;i< QuantityOfPeople; i++) {
                if(frames % 70 == 0)
                {
                    people[i].acceleration = new Vector(rnd.Next(-accelerationRange, accelerationRange), rnd.Next(-accelerationRange, accelerationRange));
                }
                //people[i].border = new Vector((maxSpeed / Math.Pow(people[i].X + BorderThickness1 + DiameterOfCircle / 2, 1)) - (maxSpeed / Math.Pow(Field.Width - BorderThickness1 - people[i].X - DiameterOfCircle / 2, 1)),
                //    (maxSpeed / Math.Pow(people[i].Y + BorderThickness1 + DiameterOfCircle / 2, 1)) - (maxSpeed / Math.Pow(Field.Height - BorderThickness1 - people[i].Y - DiameterOfCircle / 2, 1)));
                people[i].border = new Vector((maxSpeed * maxSpeed / Math.Pow(people[i].X, 2)) - (maxSpeed * maxSpeed / Math.Pow(Field.Width - BorderThickness1 - people[i].X + DiameterOfCircle / 2 - BorderOffset, 2)),
                    (maxSpeed * maxSpeed / Math.Pow(people[i].Y, 2)) - (maxSpeed * maxSpeed / Math.Pow(Field.Height - BorderThickness1 - people[i].Y + DiameterOfCircle - BorderOffset, 2)));
                //people[i].border = new Vector((1 / (people[i].X + BorderThickness1)) - (1 / (Field.Width - BorderThickness1 - people[i].X)),
                //    (1 / (people[i].Y + BorderThickness1)) - (1 / (Field.Height - BorderThickness1 - people[i].Y)));
                if (people[i].velocity.Length > maxSpeed * 2)
                {
                    people[i].velocity = new Vector(0, 0);
                }
            }
        }
        public void UpdateCenterPoint()
        {
            for(int i = 0;i < QuantityOfPeople; i++)
            {
                people[i].center = new Point(people[i].X + DiameterOfCircle / 2, people[i].Y + DiameterOfCircle / 2);
            }
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
        public void DrawVectors()
        {
            for(int i = 0;i < QuantityOfPeople; i++)
            {
                double startx = people[i].center.X, starty = people[i].center.Y;
                Line acc = new Line();
                acc.X1 = startx;
                acc.Y1 = starty;
                acc.X2 = startx + people[i].acceleration.X;
                acc.Y2 = starty + people[i].acceleration.Y;
                acc.Stroke = Brushes.Red;
                Line vel = new Line();
                vel.X1 = startx;
                vel.Y1 = starty;
                vel.X2 = startx + people[i].velocity.X;
                vel.Y2 = starty + people[i].velocity.Y;
                vel.Stroke = Brushes.Yellow;
                Line bor = new Line();
                bor.X1 = startx;
                bor.Y1 = starty;
                bor.X2 = startx + people[i].border.X;
                bor.Y2 = starty + people[i].border.Y;
                bor.Stroke = Brushes.Green;
                Field.Children.Add(acc);
                Field.Children.Add(vel);
                Field.Children.Add(bor);
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
                case "П":
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
        }
    }
}
