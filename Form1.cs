using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Zuma
{

    public class Ball
    {
        public float x, y, w = 10, h = 10;
        public Color color = Color.White;
        public bool Bullet_Hit = true;
    }
    public class Rect
    {
        public float x, y, w = 10, h = 10;
        public Color color = Color.Green;
        public bool rec_hit = true;
    }
    public class BezierCurve
    {
        public List<Point> ControlPoints = new List<Point>();
        public float TimeIncrement = 0.001f;
        public bool BeFlg = true;

        private float Factorial(int N)
        {
            float Result = 1.0f;
            for (int i = 2; i <= N; i++)
                Result *= i;

            return Result;
        }

        private float BinomialCoefficient(int N, int i)
        {
            float Result = this.Factorial(N) / (this.Factorial(i) * this.Factorial(N - i));
            return Result;
        }

        private double BernsteinPolynomial(float i, int j)
        {
            int N = this.ControlPoints.Count() - 1;
            double Result = this.BinomialCoefficient(N, j) * Math.Pow((1 - i), (N - j)) * Math.Pow(i, j);
            return Result;
        }

        private void DrawControlPoints(Graphics g)
        {
            for (int i = 0; i < this.ControlPoints.Count(); i++)
            {
                g.FillEllipse(Brushes.Orange, this.ControlPoints[i].X - 5, this.ControlPoints[i].Y - 5, 10, 10);
            }
        }

        private void DrawCurvePoints(Graphics g)
        {
            if (this.ControlPoints.Count() <= 0)
                return;
            PointF CurvePoint;
            for (float t = 0.0f; t <= 1.0; t += this.TimeIncrement)
            {
                CurvePoint = this.CalculateCurvePointAtTimeT(t);
                g.FillEllipse(Brushes.Red, CurvePoint.X - 4, CurvePoint.Y - 4, 4, 4);
              
            }
        }

        public Point GetControlPoint(int i)
        {
            return this.ControlPoints[i];
        }

        public PointF CalculateCurvePointAtTimeT(float t)
        {
            PointF Point = new PointF();
            for (int i = 0; i < this.ControlPoints.Count(); i++)
            {
                float B = (float)this.BernsteinPolynomial(t, i);
                Point.X += B * ControlPoints[i].X;
                Point.Y += B * ControlPoints[i].Y;
            }
            return Point;
        }

        public bool IsCurve_point(float XMouse, float YMouse)
        {
            Rectangle Rectangle;
            PointF CurvePoint;
            int w = 100, h = 100;
            for (float t = 0.0f; t <= 1.0; t += this.TimeIncrement)
            {
                CurvePoint = this.CalculateCurvePointAtTimeT(t);
                if (BeFlg == true)
                {

                }
            }
            return false;
        }

        public int IsControlPoint(int XMouse, int YMouse)
        {
            Rectangle Rectangle;
            for (int i = 0; i < this.ControlPoints.Count(); i++)
            {
                Rectangle = new Rectangle(this.ControlPoints[i].X - 5, this.ControlPoints[i].Y - 5, 10, 10);
                if (XMouse >= Rectangle.Left && XMouse <= Rectangle.Right && YMouse >= Rectangle.Top && YMouse <= Rectangle.Bottom)
                {
                    return i;
                }
            }
            return -1;
        }

        public float GetTimeForPoint(PointF point, float tolerance = 0.01f)
        {
            for (float t = 0.0f; t <= 1.0; t += this.TimeIncrement)
            {
                PointF curvePoint = this.CalculateCurvePointAtTimeT(t);
                if (Math.Abs(curvePoint.X - point.X) <= tolerance && Math.Abs(curvePoint.Y - point.Y) <= tolerance)
                {
                    return t;
                }
            }
            return -1;
        }


        public void ModifyControlPoint(int i, int X, int Y)
        {
            Point Point = this.ControlPoints[i];
            Point.X = X;
            Point.Y = Y;
            this.ControlPoints[i] = Point;
        }

        public void SetControlPoint(Point Point)
        {
            this.ControlPoints.Add(Point);
        }

        public void DrawCurve(Graphics g)
        {
            this.DrawControlPoints(g);
            this.DrawCurvePoints(g);
        }

    }
    public partial class Form1 : Form
    {
        Bitmap Off;
        Timer Timer = new Timer();
        BezierCurve BezCurve = new BezierCurve();
        List<Ball> LBullets = new List<Ball>();
        Rectangle Hit;
        List<Rectangle> LHits = new List<Rectangle>();
        int GiveTime = 0;


        public Form1()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            this.Load += Form1_Load;
            this.MouseDown += Form1_MouseDown;
            this.Timer.Tick += Timer_Tick;
            this.Timer.Interval = 1;
            Timer.Start();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            DrawDubb();
        }

        private void Create_Bullet(float Y)
        {
            Ball pnn = new Ball();
            pnn.x = 0;
            pnn.y = Y;
            LBullets.Add(pnn);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Off = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);

            BezCurve.SetControlPoint(new Point(100, 900));
            BezCurve.SetControlPoint(new Point(80, 565));
            BezCurve.SetControlPoint(new Point(520, 115));
            BezCurve.SetControlPoint(new Point(520, 115));

            Hit = new Rectangle(500, 200, 2, 500);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (GiveTime % 10 == 0)
            {
                if (BezCurve.BeFlg == true)
                {
                    BezCurve.BeFlg = false;
                }
                else
                {
                    BezCurve.BeFlg = true;
                }

            }
            GiveTime++;

            for (int i = 0; i < LBullets.Count; i++)
            {
                if (LBullets[i].x >= Hit.Left && LBullets[i].x <= Hit.Right && LBullets[i].y >= Hit.Top && LBullets[i].y <= Hit.Bottom)
                {

                    LBullets[i].x = Hit.Left;

                    LBullets[i].Bullet_Hit = false;
                }
                if (LBullets[i].Bullet_Hit)
                {
                    LBullets[i].x += 10;
                }
                else
                {
                    LBullets[i].x = Hit.Left;
                }

            }

            DrawDubb();
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            Text = e.Location.X + " : " + e.Location.Y;

            if (e.Button == MouseButtons.Left)
            {
                /*Create_Bullet(e.Y);*/
                BezCurve.SetControlPoint(new Point(e.X, e.Y));
            }
           
            DrawDubb();
        }

        private void DrawScene(Graphics g)
        {
            g.Clear(Color.DarkGreen);
            BezCurve.DrawCurve(g);

            for (int i = 0; i < LBullets.Count; i++)
            {
                SolidBrush brs = new SolidBrush(LBullets[i].color);
                g.FillEllipse(brs, LBullets[i].x - 4, LBullets[i].y - 4, LBullets[i].w, LBullets[i].h);
            }
        }

        void DrawDubb()
        {
            Graphics G = this.CreateGraphics();
            Graphics G2 = Graphics.FromImage(Off);
            DrawScene(G2);
            G.DrawImage(Off, 0, 0);
        }
    }
}
