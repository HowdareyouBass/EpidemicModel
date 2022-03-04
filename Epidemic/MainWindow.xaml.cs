using System;
using System.Collections.Generic;
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
        private const int QuantityOfPeople = 50, DiameterOfCircle = 15, BorderThickness = 4;
        Random rnd;
        public MainWindow()
        {
            InitializeComponent();
            people = new People[QuantityOfPeople];
            rnd = new Random();
            InitPeople();
            CompositionTarget.Rendering += UpdateFrame;
        }

        private void UpdateFrame(object sender, EventArgs e)
        {
            Field.Children.Clear();
            DrawAllPeople();
            DrawBorder();
        }
        public void Move()
        {

        }
        private void DrawBorder()
        {
            Rectangle border = new Rectangle()
            {
                Stroke = new SolidColorBrush(Color.FromArgb(255,200,200,200)),
                StrokeThickness = BorderThickness,
                Width = Field.Width - 2*BorderThickness,
                Height = Field.Height - 2*BorderThickness
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
                people[i] = new People(rnd.Next(BorderThickness,(int)Field.Width - DiameterOfCircle - BorderThickness),rnd.Next(BorderThickness,(int)Field.Height) - DiameterOfCircle - BorderThickness,"НЗ");
            }
        }
    }
}
