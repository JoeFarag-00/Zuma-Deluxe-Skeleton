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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace Zuma
{
    public class Frog
    {
        public Bitmap Frog_img;
        public float x, y, w, h;
        public float Direction;

    }
    public class Skull
    {
        public Bitmap Skull_img;
        public float x, y, w, h;
        public bool GameOver = false;
    }
    public class Background
    {
        public Bitmap bg_img;
        public float x, y, w, h;
    }

    public class Ball
    {
        public Bitmap ball_img;
        public float x, y, h, w;
        public bool Ball_Hit = false;
        public bool ishot = false;
        public float Direction;
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
        Random Rand = new Random();
        BezierCurve BezCurve = new BezierCurve();
        List<Frog> LFrogs = new List<Frog>();
        List<Skull> LSkulls = new List<Skull>();
        List<Background> Lbgs = new List<Background>();
        List<Ball> LBalls = new List<Ball>();

        List<float> xpos = new List<float>();
        List<float> ypos = new List<float>();

        List<float> dy = new List<float>();
        List<float> dx = new List<float>();
        List<float> m = new List<float>();

        bool Ballshot = false;

        List<Rectangle> LHits = new List<Rectangle>();
        Rectangle Hit;
        int speed = 5;
        int GiveTime = 0;
        int frame = 0;

        float LineXE;
        float LineYE;
        public Form1()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            /*this.Load += Form1_Load;*/
            this.MouseDown += Form1_MouseDown;
            this.KeyDown += Form1_KeyDown;
            this.Timer.Tick += Timer_Tick;
            this.Timer.Interval = 1;
            Timer.Start();
        }

        private void Create_Skull(float Xf, float Yf)
        {
            /*Skull pnn = new Skull();
            pnn.x = Xf;
            pnn.y = Yf;
            pnn.Skull_img = new Bitmap("./assets/death.png");
            LSkulls.Add(pnn);*/

            Skull pnn = new Skull();
            pnn.x = Xf;
            pnn.y = Yf;

            Bitmap originalSkull = new Bitmap("./assets/death.png");

            int newWidth = 150;
            int newHeight = 150;

            Bitmap resizedSkull = new Bitmap(newWidth, newHeight);
            using (Graphics g = Graphics.FromImage(resizedSkull))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(originalSkull, 0, 0, newWidth, newHeight);
            }

            pnn.Skull_img = resizedSkull;
            LSkulls.Add(pnn);

        }

        private void Create_Frog(float Xf, float Yf)
        {
            Frog pnn = new Frog();
            pnn.x = Xf;
            pnn.y = Yf;
            pnn.Frog_img = new Bitmap("./assets/zuma.png");
            LFrogs.Add(pnn);
        }

        private void Create_Background(float Xf, float Yf)
        {
            /*Skull pnn = new Skull();
            pnn.x = Xf;
            pnn.y = Yf;
            pnn.Skull_img = new Bitmap("./assets/death.png");
            LSkulls.Add(pnn);*/

            Background pnn = new Background();
            pnn.x = Xf;
            pnn.y = Yf;

            Bitmap OG_BG = new Bitmap("./assets/bg.jpg");

            int newWidth = 1920;
            int newHeight = 1080;

            Bitmap resizedbg = new Bitmap(newWidth, newHeight);
            using (Graphics g = Graphics.FromImage(resizedbg))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(OG_BG, 0, 0, newWidth, newHeight);
            }

            pnn.bg_img = resizedbg;
            Lbgs.Add(pnn);

        }

        private void Create_FrogBalls(float Xf, float Yf)
        {
            Ball Fball = new Ball();
            frame = Rand.Next(1, 8);
            Fball.x = Xf;
            Fball.y = Yf;
            Fball.ball_img = new Bitmap("./assets/"+ frame + ".png");
            LBalls.Add(Fball);

        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
            Off = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);

            Create_Frog(this.ClientSize.Width/2 - 100,this.ClientSize.Height/2 - 100);
            Create_FrogBalls(this.ClientSize.Width / 2 + 15, this.ClientSize.Height / 2 - 70);

            Create_Skull(360, 520);
            Create_Background(0, 0);

            BezCurve.SetControlPoint(new Point(80, ClientSize.Height));
            BezCurve.SetControlPoint(new Point(80, 565));
            BezCurve.SetControlPoint(new Point(80, 165));
            BezCurve.SetControlPoint(new Point(520, 60));
            BezCurve.SetControlPoint(new Point(1020, 10));
            /* BezCurve.SetControlPoint(new Point(1120, 30));
             BezCurve.SetControlPoint(new Point(1220, 30));*/
            BezCurve.SetControlPoint(new Point(1320, 30));
            BezCurve.SetControlPoint(new Point(1420, 30));

            BezCurve.SetControlPoint(new Point(1620, 20));
            BezCurve.SetControlPoint(new Point(1820, 20));

            BezCurve.SetControlPoint(new Point(1600, 200));
            BezCurve.SetControlPoint(new Point(1600, 600));

            BezCurve.SetControlPoint(new Point(1920, 200));
            BezCurve.SetControlPoint(new Point(1880, 600));
            BezCurve.SetControlPoint(new Point(1980, 1000));
            BezCurve.SetControlPoint(new Point(2180, 1100));
            BezCurve.SetControlPoint(new Point(2300, 1800));

            BezCurve.SetControlPoint(new Point(900, 1000));
            BezCurve.SetControlPoint(new Point(-500, 900));
            BezCurve.SetControlPoint(new Point(-500, 900));

            BezCurve.SetControlPoint(new Point(1000, 900));
            BezCurve.SetControlPoint(new Point(3900, 820));

            BezCurve.SetControlPoint(new Point(1300, 820));
            BezCurve.SetControlPoint(new Point(1680, 820));

            BezCurve.SetControlPoint(new Point(1380, 780));
            BezCurve.SetControlPoint(new Point(1300, 680));
            BezCurve.SetControlPoint(new Point(960, 580));
            BezCurve.SetControlPoint(new Point(960, 580));

            BezCurve.SetControlPoint(new Point(1360, 480));
            BezCurve.SetControlPoint(new Point(1460, 380));

            BezCurve.SetControlPoint(new Point(1460, 280));
            BezCurve.SetControlPoint(new Point(1760, 280));

            BezCurve.SetControlPoint(new Point(1260, 50));
            BezCurve.SetControlPoint(new Point(1000, 170));



            BezCurve.SetControlPoint(new Point(700, 250));

            BezCurve.SetControlPoint(new Point(450, 590));



            /*BezCurve.SetControlPoint(new Point(300, 450));*/
            /* Hit = new Rectangle(500, 200, 2, 500);*/
        }
        private void Animate_BallShoot()
        {
            dx.Clear();
            dy.Clear();
            m.Clear();
            xpos.Clear();
            ypos.Clear();

            xpos.Add(LBalls[0].x);
            xpos.Add(LineXE);

            ypos.Add(LBalls[0].y);
            ypos.Add(LineYE);

            dx.Add(xpos[1] - xpos[0]);
            dy.Add(ypos[1] - ypos[0]);
            m.Add(dy[0] / dx[0]);


        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if(Ballshot)
            {
                this.Text = Ballshot + " Xst " + xpos[0] + " Yst " + ypos[0] + " Xend " + xpos[1] + " Yend " + ypos[1];
                if (Math.Abs(dx[0]) > Math.Abs(dy[0]))
                {
                    if (xpos[0] < xpos[1])
                    {
                        LBalls[0].x += speed;
                        LBalls[0].y += m[0] * speed;

                    }
                    else
                    {
                        LBalls[0].x -= speed;
                        LBalls[0].y -= m[0] * speed;

                    }
                }
                else
                {
                    if (ypos[0] < ypos[1])
                    {
                        LBalls[0].y += speed;
                        LBalls[0].x += 1 / m[0] * speed;

                    }
                    else
                    {
                        LBalls[0].y -= speed;
                        LBalls[0].x -= 1 / m[0] * speed;

                    }

                }
               /* LBalls[0].x += 20;*/
            }    
            
            DrawDubb();
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            Text = e.Location.X + " : " + e.Location.Y;
            
           
            DrawDubb();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Right)
            {
                LFrogs[0].Direction += 5;
                LBalls[0].Direction += 5;

            }
            if (e.KeyCode == Keys.Left)
            {
                LFrogs[0].Direction -= 5;
                LBalls[0].Direction -= 5;
            }

            if (e.KeyCode == Keys.Space)
            {
                Ballshot = true;
                Animate_BallShoot();
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            DrawDubb();
        }

        private void DrawScene(Graphics g)
        {
            g.Clear(Color.DarkMagenta);
            Pen dotPen = new Pen(Color.Black, 5);
            BezCurve.DrawCurve(g);


            float angleInRadians = (LFrogs[0].Direction - 90f) * (float)Math.PI / 180f;
            float lineLength = 200f;
            float XfrogEnd = LFrogs[0].x + LFrogs[0].Frog_img.Width / 2 + lineLength * (float)Math.Cos(angleInRadians);
            float YfrogEnd = LFrogs[0].y + LFrogs[0].Frog_img.Height / 2 + lineLength * (float)Math.Sin(angleInRadians);

            g.DrawLine(dotPen, LFrogs[0].x + LFrogs[0].Frog_img.Width / 2, LFrogs[0].y + LFrogs[0].Frog_img.Height / 2, XfrogEnd, YfrogEnd);

            LineXE = XfrogEnd;
            LineYE = YfrogEnd;

            g.TranslateTransform(LFrogs[0].x + LFrogs[0].Frog_img.Width / 2, LFrogs[0].y + LFrogs[0].Frog_img.Height / 2);
            g.RotateTransform(LFrogs[0].Direction);
            g.DrawImage(LFrogs[0].Frog_img, -LFrogs[0].Frog_img.Width / 2, -LFrogs[0].Frog_img.Height / 2);
            g.ResetTransform();

            g.DrawImage(LSkulls[0].Skull_img, LSkulls[0].x, LSkulls[0].y);

            for (int i = 0; i < LBalls.Count; i++)
            {
               /* g.DrawImage(LBalls[0].ball_img, LBalls[0].x, LBalls[0].y);*/

                g.TranslateTransform(LFrogs[0].x + LFrogs[0].Frog_img.Width / 2, LFrogs[0].y + LFrogs[0].Frog_img.Height / 2);
                g.RotateTransform(LBalls[i].Direction);
                g.DrawImage(LBalls[i].ball_img, -LBalls[i].ball_img.Width / 2 + 65, -LBalls[i].ball_img.Height / 2);
                g.ResetTransform();
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
