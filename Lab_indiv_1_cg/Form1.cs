using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab_indiv_1_cg
{
    public partial class Form1 : Form
    {
        Bitmap bmp;
        Queue<Point> Points = new Queue<Point>();
        List<Point> all_points = new List<Point>();
        Point curr_point;


        public Form1()
        {
            InitializeComponent();
            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            bmp = (Bitmap)pictureBox1.Image;
            pictureBox1.Image = bmp;
        }

        private void Add_point(object sender, MouseEventArgs e)
        {
            Point p = new Point(e.X, e.Y);
            all_points.Add(p);
            if (all_points.Count <= 3)
                Points.Enqueue(p);
            curr_point = p;
            ((Bitmap)pictureBox1.Image).SetPixel(p.X, p.Y, Color.Black);
            pictureBox1.Image = pictureBox1.Image;


            if (all_points.Count == 2 || all_points.Count == 3)
                Print_lines(Points);
            else if (all_points.Count >= 4)
            {
                clear_image();
                if (!check_point_inside_polygon(curr_point, Points))
                    Points = find_shell(Points, all_points, curr_point);
                Print_lines(Points);
                Print_points(all_points);
            }
        }

        private void Print_lines(Queue<Point> q)
        {
            Queue<Point> p = new Queue<Point>(q);
            var pen = new Pen(Color.Black, 1);
            var g = Graphics.FromImage(pictureBox1.Image);
            Point start = new Point(-5, -5);
            Point prev = p.Dequeue();
            Point start1 = prev;
            p.Enqueue(prev);
            Point curr = p.Dequeue();
            while (prev != start)
            {
                g.DrawLine(pen, prev, curr);
                prev = curr;
                p.Enqueue(prev);
                curr = p.Dequeue();
                start = start1;
            }
            p.Enqueue(curr);
            pictureBox1.Image = pictureBox1.Image;
        }

        private void Print_points(List<Point> l)
        {
            foreach (Point p in l)
            {
                ((Bitmap)pictureBox1.Image).SetPixel(p.X, p.Y, Color.Black);
                ((Bitmap)pictureBox1.Image).SetPixel(p.X+1, p.Y, Color.Black);
                ((Bitmap)pictureBox1.Image).SetPixel(p.X, p.Y+1, Color.Black);
                ((Bitmap)pictureBox1.Image).SetPixel(p.X+1, p.Y+1, Color.Black);
                pictureBox1.Image = pictureBox1.Image;
            }    
        }

        private double corner_cos(Point p1, Point p2, Point p3)
        {
            double a = make_lenght(p2, p3);
            double b = make_lenght(p1, p3);
            double c = make_lenght(p1, p2);
            return (b * b + c * c - a * a) / (2 * b * c);
        }

        private Queue<Point> find_shell(Queue<Point> q, List<Point> l, Point p)
        {
            Queue<Point> shell = new Queue<Point>();
            Point start_point = new Point(-1,-1);
            Point prev_point = q.Dequeue();
            q.Enqueue(prev_point);
            Point curr_p = q.Dequeue();
            bool adding_agree = false;
            bool point_added = false;
            while (curr_p != start_point)
            {
                if (check_leftline(p, curr_p, l) || check_rightline(p, curr_p, l))
                {
                    shell.Enqueue(curr_p);
                    
                    if (!point_added)
                    {
                        start_point = curr_p;
                        point_added = true;
                    }
                    if (corner_cos(curr_p, prev_point, p) > corner_cos(curr_p, q.Peek(), p))
                        adding_agree = true;
                    else
                    {
                        shell.Enqueue(p);
                        adding_agree = false;
                    }

                }

                else if (adding_agree)
                    shell.Enqueue(curr_p);

                q.Enqueue(curr_p);
                prev_point = curr_p;
                curr_p = q.Dequeue();
            }
            q.Enqueue(curr_p);
            return shell;
        }

        private double make_lenght(Point p1, Point p2)
        {
            return Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y));
        }

        private bool check_leftline(Point start, Point end, List<Point> l)
        {
            foreach (Point point in l)
            {
                double det = (end.X - start.X) * (point.Y - start.Y) -
                                (end.Y - start.Y) * (point.X - start.X);

                if (det >= 0)
                    continue;
                else 
                    return false;
                
            }
            return true;
        }

        private bool check_rightline(Point start, Point end, List<Point> l)
        {
            foreach (Point point in l)
            {
                double det = (end.X - start.X) * (point.Y - start.Y) -
                                (end.Y - start.Y) * (point.X - start.X);

                if (det <= 0)
                    continue;
                else
                    return false;
            }
            return true;
        }

        //поиск координаты y по координате х на прямой
        private double find_y(double x, Point start, Point last)
        {
            return (x - start.X) / (last.X - start.X) * (last.Y - start.Y) + start.Y;
        }

        private bool check_point_inside_polygon(Point p, Queue<Point> que)
        {
            Point prev = que.Dequeue();
            que.Enqueue(prev);
            Point curr = que.Dequeue();
            Point start = new Point(-5, -5);
            Point start1 = prev;
            int counter = 0;
            while (prev != start)
            {
                Point p_st;
                Point p_fin;
                start = start1;
                if (p.Y < Math.Min(prev.Y, curr.Y) || p.Y > Math.Max(prev.Y, curr.Y))
                {
                    que.Enqueue(curr);
                    prev = curr;
                    curr = que.Dequeue();
                    continue;
                }
                
                if (prev.X > curr.X)
                {
                    p_st = new Point(curr.X, curr.Y);
                    p_fin = new Point(prev.X, prev.Y);
                }
                else
                {
                    p_st = new Point(prev.X, prev.Y);
                    p_fin = new Point(curr.X, curr.Y);
                }

                if (p.X <= p_fin.X)
                {
                    double x_start = Math.Max(p_st.X, p.X);
                    if (p_st.X - p_fin.X != 0)
                    {
                        double h = (p_fin.X - x_start) / 2000;
                        double delta = Math.Abs(find_y(x_start, p_st, p_fin) - p.Y);
                        for (double i = x_start; i <= p_fin.X; i += h)
                        {
                            if (delta >= Math.Abs(find_y(i + h, p_st, p_fin) - p.Y))
                                delta = Math.Abs(find_y(i + h, p_st, p_fin) - p.Y);
                            else if (delta < 1)
                            {
                                counter += 1;
                                break;
                            }
                        }
                    }
                    else
                        counter += 1;
                }
                que.Enqueue(curr);
                prev = curr;
                curr = que.Dequeue();
            }
            que.Enqueue(curr);
            return counter == 1;
        }

        private void Button_clear_click(object sender, EventArgs e)
        {
            var g = Graphics.FromImage(pictureBox1.Image);
            g.Clear(pictureBox1.BackColor);
            pictureBox1.Image = pictureBox1.Image;
            Points.Clear();
            all_points.Clear();
            curr_point = new Point(0, 0);
        }

        private void clear_image()
        {
            var g = Graphics.FromImage(pictureBox1.Image);
            g.Clear(pictureBox1.BackColor);
            pictureBox1.Image = pictureBox1.Image;
        }

        private void Button_build_click(object sender, EventArgs e)
        {
        }

    }
}
