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
        private const int QuantityOfPeople = 10, DiameterOfCircle = 15, BorderThickness1 = 4, maxSpeed = 500, accelerationRange = 100, velocityRange = 1000;
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
            DrawBorder();
            stopwatch.Restart();
            frames++;
        }
        public void Move(TimeSpan ts)
        {
            for(int i = 0;i < QuantityOfPeople; i++)
            {
                if(people[i].velocity.Length <= maxSpeed && Vector.Multiply(Vector.Add(people[i].velocity, people[i].acceleration), ts.TotalSeconds).Length <= maxSpeed)
                {
                    people[i].velocity = Vector.Multiply(Vector.Add(people[i].velocity, people[i].acceleration), ts.TotalSeconds);
                    people[i].velocity = Vector.Multiply(Vector.Add(people[i].velocity, people[i].border), ts.TotalSeconds);
                }
                people[i].X += people[i].velocity.X * ts.TotalSeconds;
                people[i].Y += people[i].velocity.Y * ts.TotalSeconds;
            }
        }
        public void UpdateVectors()
        {
            for(int i = 0;i< QuantityOfPeople; i++) {
                if(frames % 100 == 0)
                {
                    people[i].acceleration = new Vector(rnd.Next(-accelerationRange, accelerationRange), rnd.Next(-accelerationRange, accelerationRange));
                }
                people[i].border = new Vector((1 / Math.Pow(people[i].X + BorderThickness1, 2)) - (1 / Math.Pow(Field.Width - BorderThickness1 - people[i].X, 2)),
                    (1 / Math.Pow(people[i].Y + BorderThickness1, 2)) - (1 / Math.Pow(Field.Height - BorderThickness1 - people[i].Y, 2)));
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
