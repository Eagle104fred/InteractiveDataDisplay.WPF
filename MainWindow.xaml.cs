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
using InteractiveDataDisplay.WPF;
using System.Threading;
using Unity.Mathematics;
using Random = System.Random;


namespace IDD
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
   


    public partial class MainWindow : Window
    {


        //KS:这个地方和button里面的内容搭配使用 
        private Random rand = new Random(0);
        private double[] RandomWalk(int points = 5, double start = 100, double mult = 50)
        {
            // return an array of difting random numbers
            double[] values = new double[points];
            values[0] = start;
            for (int i = 1; i < points; i++)
                values[i] = values[i - 1] + (rand.NextDouble() - .5) * mult;
            return values;
        }
        private double[] Consecutive(int points, double offset = 0, double stepSize = 1)
        {
            // return an array of ascending numbers starting at 1
            double[] values = new double[points];
            for (int i = 0; i < points; i++)
                values[i] = i * stepSize + 1 + offset;
            return values;
        }

        //KS:画图工具 
        public void PenDrawKeepOld(LineGraph line,List<Point> pointList)
        {
            List<Double> list_X = new List<Double>();
            List<Double> list_Y = new List<Double>();
            for (int i = 0; i < pointList.Count; i++)
            {
                list_X.Add(pointList[i].X);
                list_Y.Add(pointList[i].Y);

            }

            DrawLineKeepOld(line, list_X, list_Y);
        }
        public void PenDrawRefresh(LineGraph line, List<Point> pointList)
        {
            List<Double> list_X = new List<Double>();
            List<Double> list_Y = new List<Double>();
            for (int i = 0; i < pointList.Count; i++)
            {
                list_X.Add(pointList[i].X);
                list_Y.Add(pointList[i].Y);

            }

            DrawLineRefresh(line, list_X, list_Y);
        }
        //KS:画之前保留页面 
        private void DrawLineKeepOld(LineGraph line,List<double> X,List<double> Y)
        {
            _ = Dispatcher.BeginInvoke(new Action(delegate
            {
                var lg = new LineGraph();
                
                line.Children.Add(lg);
                lg.Stroke = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
                
                lg.StrokeThickness = 2;
                //KS:绘制 
                lg.Plot(X, Y);

            }));
        }
        //KS:画之前刷新页面 
        private void DrawLineRefresh(LineGraph line, List<double> X, List<double> Y)
        {
            _ = Dispatcher.BeginInvoke(new Action(delegate
            {
                var lg = new LineGraph();
                line.Children.Clear();
                line.Children.Add(lg);
                lg.Stroke = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));

                lg.StrokeThickness = 2;
                //KS:绘制 
                lg.Plot(X, Y);

            }));
        }
        //KS:窗口载入函数(主函数) 
        public MainWindow()
        {
            InitializeComponent();
            LineGraph line = new LineGraph();
            myChart.Content = line; //KS:使得图标包含你要画的元素 

            //DrawTrapeter(line);
            DrawFieldOfViewDynamic(line);
            //DrawLineWithDynamic(line);

        }
        
        /// <summary>
        /// //KS:调用案例 
        /// </summary>
        /// <param name="line"></param>
        public void DrawLineWithDynamic(LineGraph line)
        {
            List<Point> pointList = new List<Point>();

            Task.Run(async() => {
                double x_1 = 0;
                double y_1 = 0;
                double x_2 = 10;
                double y_2 = 10;
                for (int i = 0; i < 10; i++)
                {
                    Thread.Sleep(100);
                    pointList.Clear();

                    pointList.Add(new Point(x_1, y_1));
                    pointList.Add(new Point(x_2, y_2));
                    x_1 += 10;
                    x_2 += 10;
                    //KS:切换两种刷新方法 
                    //PenDrawKeepOld(line, pointList);
                    PenDrawRefresh(line, pointList);
                }
                await Task.Delay(1);
            });
            
        }
        //KS:画梯形 
        public void DrawTrapeter(LineGraph line)
        {
            List<Point> pointList = new List<Point>();
            Task.Run(async () =>
            {

                pointList.Add(new Point(-4, 6));
                pointList.Add(new Point(4, 6));
                pointList.Add(new Point(1, 1));
                pointList.Add(new Point(-1, 1));
                pointList.Add(new Point(-4, 6));
                PenDrawKeepOld(line, pointList);

                await Task.Delay(1);
            });

        }

        //KS:画旋转梯形 
        public void DrawFieldOfViewDynamic(LineGraph line,double center_X=0,double center_Y=0)
        {
           
            
            Task.Run(async () =>
            {
                //KS:构建原初图形 
                List<Point> pointList = new List<Point>();
                pointList.Add(new Point(-4, 6));
                pointList.Add(new Point(4, 6));
                pointList.Add(new Point(1, 1));
                pointList.Add(new Point(-1, 1));
                pointList.Add(new Point(-4, 6));
                
                for (double i = 0; i < 360;i+=2)
                {
                    Thread.Sleep(1000);
                    List<Point> pointListTrans = new List<Point>();
                    for (int j = 0; j < pointList.Count; j++)
                    {
                        Point tempPoint = RotateTransform(pointList[j].X - center_X, pointList[j].Y - center_Y, i);
                        pointListTrans.Add(tempPoint);
                        
                    }

                    //KS:切换两种刷新方法 
                    PenDrawKeepOld(line, pointListTrans);
                    //PenDrawRefresh(line, pointListTrans);
                }



                await Task.Delay(1);
            });
            
        }
        public Point RotateTransform(double x, double y , double angle)
        {

            angle = angle * Math.PI / 180; //KS:角度转弧度

            //KS:构造旋转矩阵 
            double angleSin = Math.Sin(angle);
            double angleCos = Math.Cos(angle);
            double2x2 rotation = new double2x2(angleCos,-angleSin,angleSin,angleCos);

            //KS:原本的向量 
            double2 vector = new double2(x,y);
            vector = math.mul(rotation, vector); //KS:矩阵乘以向量 

            return new Point(vector.x, vector.y);

        }


        private void button_Click(object sender, RoutedEventArgs e)
        {
            int pointCount = 10_000;
            double[] xs = Consecutive(pointCount);
            double[] ys1 = RandomWalk(pointCount);
            double[] ys2 = RandomWalk(pointCount);

            // create the lines and describe their styling
            var line1 = new InteractiveDataDisplay.WPF.LineGraph
            {
                Stroke = new SolidColorBrush(Colors.Blue),
                Description = "Line A",
                StrokeThickness = 2
            };

            var line2 = new InteractiveDataDisplay.WPF.LineGraph
            {
                Stroke = new SolidColorBrush(Colors.Red),
                Description = "Line B",
                StrokeThickness = 2
            };

            // load data into the lines
            line1.Plot(xs, ys1);
            line2.Plot(xs, ys2);

            // add lines into the grid
            myGrid.Children.Clear();
            myGrid.Children.Add(line1);
            myGrid.Children.Add(line2);

            // customize styling
            myChart.Title = $"Line Plot ({pointCount:n0} points each)";
            myChart.BottomTitle = $"Horizontal Axis Label";
            myChart.LeftTitle = $"Vertical Axis Label";
            myChart.IsAutoFitEnabled = true;
            myChart.LegendVisibility = Visibility.Visible;
        }
    }
}
