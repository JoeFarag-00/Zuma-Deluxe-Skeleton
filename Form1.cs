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
        public bool Ball_Hit;
        public bool ishot;
        public float Direction;
        public int type;

        public float tpos;
        public RectangleF HitBox;

        public float Xstart;
        public float Ystart;

        public float Xend;
        public float Yend;

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

        private double Factorial(int N)
        {
            double Result = 1.0f;
            for (int i = 2; i <= N; i++)
                Result *= i;

            return Result;
        }

        private double BinomialCoefficient(int N, int i)
        {
            double Result = this.Factorial(N) / (this.Factorial(i) * this.Factorial(N - i));
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
        List<Ball> LBezBalls = new List<Ball>();

        List<float> xpos = new List<float>();
        List<float> ypos = new List<float>();

        List<float> dy = new List<float>();
        List<float> dx = new List<float>();
        List<float> m = new List<float>();

        float DX, DY, M;

        List<PointF> CurvePoints = new List<PointF>();

        bool Ballshot = false;

        List<Rectangle> LHits = new List<Rectangle>();
        Rectangle Hit;
        int speed = 15;
        int GiveTime = 0;

        int GiveTimeanim = 0;
        int frame = 0;
        int ballshotct = 0;
        int MoveBalls = 0;

        float PushBall = 0;
        float LineXE;
        float LineYE;

        public Form1()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            /*this.Load += Form1_Load;*/
            this.MouseDown += Form1_MouseDown;
            this.KeyDown += Form1_KeyDown;
            this.Paint += Form1_Paint;
            this.Timer.Tick += Timer_Tick;
            this.Timer.Interval = 1;
            Timer.Start();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Off = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);
        

            Create_Frog(this.ClientSize.Width/2 - 100,this.ClientSize.Height/2 - 100);
            Create_FrogBalls(this.ClientSize.Width / 2 + 15, this.ClientSize.Height / 2 - 70);

            Create_Skull(300, 480);
            Create_Background(0, 0);

            BezCurve.SetControlPoint(new Point(14, 242));
            BezCurve.SetControlPoint(new Point(750, -10));
            BezCurve.SetControlPoint(new Point(1556, -20));
            BezCurve.SetControlPoint(new Point(1525, 109));
            BezCurve.SetControlPoint(new Point(1945, 400));
            BezCurve.SetControlPoint(new Point(2000, 371));
            BezCurve.SetControlPoint(new Point(2000, 416));
            BezCurve.SetControlPoint(new Point(1945, 800));

            BezCurve.SetControlPoint(new Point(1945, 800));
            BezCurve.SetControlPoint(new Point(1945, 800));
            BezCurve.SetControlPoint(new Point(1945, 1000));

            BezCurve.SetControlPoint(new Point(400, 100));
/*            BezCurve.SetControlPoint(new Point(1604, 600));
*/
            BezCurve.SetControlPoint(new Point(1604, 559));
            BezCurve.SetControlPoint(new Point(1546, 513));
            BezCurve.SetControlPoint(new Point(1497, 432));
            BezCurve.SetControlPoint(new Point(1448, 355));
            BezCurve.SetControlPoint(new Point(1395, 281));
            BezCurve.SetControlPoint(new Point(1289, 154));
            BezCurve.SetControlPoint(new Point(1227, 168));
            BezCurve.SetControlPoint(new Point(1137, 160));
            BezCurve.SetControlPoint(new Point(917, 131));
            BezCurve.SetControlPoint(new Point(820, 220));
            BezCurve.SetControlPoint(new Point(567, 215));
            BezCurve.SetControlPoint(new Point(329, 263));
            BezCurve.SetControlPoint(new Point(143, 333));
            BezCurve.SetControlPoint(new Point(83, 507));

            BezCurve.SetControlPoint(new Point(-800, 600));

            BezCurve.SetControlPoint(new Point(-800, 800));

           /* BezCurve.SetControlPoint(new Point(-1000, 2000));*/

            BezCurve.SetControlPoint(new Point(-400, 507));
            BezCurve.SetControlPoint(new Point(36, 744));
            BezCurve.SetControlPoint(new Point(100, 950));
            BezCurve.SetControlPoint(new Point(217, 1220));
            BezCurve.SetControlPoint(new Point(418, 1500));

            BezCurve.SetControlPoint(new Point(611, 1100));


            BezCurve.SetControlPoint(new Point(911, 1100));
            BezCurve.SetControlPoint(new Point(1242, 1100));
            BezCurve.SetControlPoint(new Point(1393, 813));
            BezCurve.SetControlPoint(new Point(1595, 884));
            BezCurve.SetControlPoint(new Point(1900, 1000));
            BezCurve.SetControlPoint(new Point(2500, 800));

            BezCurve.SetControlPoint(new Point(2000, 1000));
            BezCurve.SetControlPoint(new Point(1676, 702));

            BezCurve.SetControlPoint(new Point(1676, 702));
            BezCurve.SetControlPoint(new Point(1680, 621));
            BezCurve.SetControlPoint(new Point(1568, 646));
            BezCurve.SetControlPoint(new Point(1505, 577));
            BezCurve.SetControlPoint(new Point(1392, 572));

            BezCurve.SetControlPoint(new Point(1349, 639));
            BezCurve.SetControlPoint(new Point(1243, 695));
            BezCurve.SetControlPoint(new Point(1186, 838));
            BezCurve.SetControlPoint(new Point(1057, 1000));

            BezCurve.SetControlPoint(new Point(992, 1000));
            BezCurve.SetControlPoint(new Point(900, 877));
            BezCurve.SetControlPoint(new Point(815, 725));
            BezCurve.SetControlPoint(new Point(716, 900));
            BezCurve.SetControlPoint(new Point(592, 824));
            BezCurve.SetControlPoint(new Point(542, 694));
            BezCurve.SetControlPoint(new Point(471, 779));
            BezCurve.SetControlPoint(new Point(428, 705));
            BezCurve.SetControlPoint(new Point(345, 681));
            BezCurve.SetControlPoint(new Point(367, 607));


            /*Create_Ball_Bezier();*/

            for (float t = 0; t <= 1; t += 0.0001f)
            {
                CurvePoints.Add(BezCurve.CalculateCurvePointAtTimeT(t));
                /*Console.WriteLine(CurvePoints[mina].X);
                mina++;*/
               
            }
            /* for(int i = 0; i < 10; i++)
             {
                 Create_Ball_Bezier();
             }*/
        }
        private void Create_Skull(float Xf, float Yf)
        {

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

            Background pnn = new Background();
            pnn.x = Xf;
            pnn.y = Yf;

            Bitmap OG_BG = new Bitmap("./assets/map.png");

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
            Fball.x = Xf;
            Fball.y = Yf;
            Fball.type = Rand.Next(1, 8);
            Fball.ball_img = new Bitmap("./assets/" + Fball.type + ".png");
            Fball.ishot = false;
            Fball.Direction = LFrogs[0].Direction;
            LBalls.Add(Fball);

        }

        private void Create_Ball_Bezier()
        {
            /*if (LBalls.Count >= 100)
                 return;*/

            float startX = BezCurve.GetControlPoint(0).X;
            float startY = BezCurve.GetControlPoint(0).Y;
            frame = Rand.Next(1, 8);

            Ball BezoBall = new Ball();
            BezoBall.x = startX;
            BezoBall.y = startY;
            BezoBall.ball_img = new Bitmap("./assets/" + frame + ".png");
            BezoBall.type = frame;
            BezoBall.ishot = false;
            BezoBall.tpos = 0;

            BezoBall.HitBox = new RectangleF(BezoBall.x, BezoBall.y, BezoBall.ball_img.Width, BezoBall.ball_img.Height);

            LBezBalls.Add(BezoBall);

            /*PushBall += 1;*/

        }

        private void CheckCollision()
        {

            for (int i = 0; i < LBalls.Count; i++)
            {
                Ball ball = LBalls[i];
                if (!ball.ishot)
                    continue;

                for (int j = 0; j < LBezBalls.Count; j++)
                {
                    Ball bezBall = LBezBalls[j];
                    if (bezBall.type == ball.type && ball.HitBox.IntersectsWith(bezBall.HitBox))
                    {
                        LBalls.RemoveAt(i);
                        LBezBalls.RemoveAt(j);
                        i--; 
                        break;
                    }
                }
            }

        }

        private void GameOver()
        {
            Timer.Stop();
            MessageBox.Show("Game Over");
        }
        private void Animate_Ball_Bezier()
        {
            for (int i = 0; i < LBezBalls.Count; i++)
            {
                PointF BPos = BezCurve.CalculateCurvePointAtTimeT(LBezBalls[i].tpos);
                LBezBalls[i].tpos += 0.008f;
                LBezBalls[i].x = BPos.X;
                LBezBalls[i].y = BPos.Y;
            }
               

        }

        private void Timer_Tick(object sender, EventArgs e)
        {
           /* if (Ballshot)*/
            {
                for (int i = 0; i < LBalls.Count; i++)
                {

                    if (LBalls[i].ishot)
                    {
                        DX = LBalls[i].Xend - LBalls[i].Xstart;
                        DY = LBalls[i].Yend - LBalls[i].Ystart;
                        M = DY / DX;

                        if (Math.Abs(DX) > Math.Abs(DY))
                        {
                            if (LBalls[i].Xstart < LBalls[i].Xend)
                            {
                                LBalls[i].x += speed;
                                LBalls[i].y += M * speed;

                            }
                            else
                            {
                                LBalls[i].x -= speed;
                                LBalls[i].y -= M * speed;
                            }
                        }
                        else
                        {
                            if (LBalls[i].Ystart < LBalls[i].Yend)
                            {
                                LBalls[i].y += speed;
                                LBalls[i].x += 1 / M * speed;
                            }
                            else
                            {
                                LBalls[i].y -= speed;
                                LBalls[i].x -= 1 / M * speed;
                            }
                        }
                    }
                }
            }

            if (GiveTime > 2)
            {
                Create_Ball_Bezier();
                Animate_Ball_Bezier();

                GiveTime = 0;
            }
            /*if(GiveTimeanim  < 1)
            {
                Animate_Ball_Bezier();
                GiveTime = 0;
            }*/
            CheckCollision();

            if (LBezBalls.Count > 0 && LBezBalls[0].tpos >= 1.0f)
            {
                GameOver();
                return;
            }

            GiveTime++;
            GiveTimeanim++;
           /* this.Text = Ballshot + " Counter " + GiveTime;*/

            DrawDubb();
            
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            Text = e.Location.X + " : " + e.Location.Y + " ; " + Ballshot;
            
           
            DrawDubb();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)    
        {
            if (e.KeyCode == Keys.Right)
            {
                LFrogs[0].Direction += 5;
                LBalls[LBalls.Count - 1].Direction += 5;

            }
            if (e.KeyCode == Keys.Left)
            {
                LFrogs[0].Direction -= 5;
                LBalls[LBalls.Count - 1].Direction -= 5;
            }

            if (e.KeyCode == Keys.Space)
            {
                Ballshot = true;
                LBalls[ballshotct].ishot = true;

                float startX = LFrogs[0].x + LFrogs[0].Frog_img.Width / 2;
                float startY = LFrogs[0].y + LFrogs[0].Frog_img.Height / 2;

                LBalls[ballshotct].Xstart = startX;
                LBalls[ballshotct].Xend = LineXE;

                LBalls[ballshotct].Ystart = startY;
                LBalls[ballshotct].Yend = LineYE;

               /* ShootBall();*/

                Create_FrogBalls(LFrogs[0].x, LFrogs[0].y);
                ballshotct++;

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

            g.DrawImage(Lbgs[0].bg_img, Lbgs[0].x, Lbgs[0].y);

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


                if (LBalls[i].ishot)
                {
                    g.DrawImage(LBalls[i].ball_img, LBalls[i].x, LBalls[i].y);
                    /*Create_FrogBalls(-LBalls[i].ball_img.Width / 2 + 65, -LBalls[i].ball_img.Height / 2);*/
                }
                else
                {
                    g.TranslateTransform(LFrogs[0].x + LFrogs[0].Frog_img.Width / 2, LFrogs[0].y + LFrogs[0].Frog_img.Height / 2);
                    g.RotateTransform(LBalls[LBalls.Count-1].Direction);
                    g.DrawImage(LBalls[i].ball_img, -LBalls[i].ball_img.Width / 2 + 65, -LBalls[i].ball_img.Height / 2);
                    g.ResetTransform();
                }
            }

            for (int i = 0; i < LBezBalls.Count; i++)
            {
                g.DrawImage(LBezBalls[i].ball_img, LBezBalls[i].x, LBezBalls[i].y);
                /*g.DrawRectangle(LBezBalls[i].HitBox, LBezBalls[i].x, LBezBalls[i].y);*/
            }

           /* BezCurve.DrawCurve(g);*/
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
