using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Diagnostics;

namespace blobdraw
{
    public partial class Form1 : Form
    {
        private ArrayList circles;
        private Circle actual_circle;
        private bool buttondown;
        private System.Drawing.Graphics canvas;
        private Pen pen;
        private Bitmap backbuffer;
        private System.Drawing.Graphics g;
        private enum Modes {New, UpdateCenter, UpdateRadius};
        private Modes ModeSelect;
        const int ANCHORSIZE = 5;
        private double cutoff = 1.0;

        public Form1()
        {
            InitializeComponent();
            buttondown = false;
            circles = new ArrayList();
            canvas = this.CreateGraphics();
            pen = new Pen(System.Drawing.Color.Green, 2);
            backbuffer = new Bitmap(this.Width, this.Height);
            g = Graphics.FromImage(backbuffer);
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            ModeSelect = Modes.New;
            buttondown = true;

            foreach (Circle c in circles){
                if (e.X > (c.getX() - ANCHORSIZE) && e.X < (c.getX() + ANCHORSIZE) && e.Y > (c.getY() - ANCHORSIZE) && e.Y < (c.getY() + ANCHORSIZE))
                {
                    if (e.Button == System.Windows.Forms.MouseButtons.Right)
                    {
                        c.setR(c.getR() * -1);
                        buttondown = false;
                        return;
                    }
                    ModeSelect = Modes.UpdateCenter;
                    actual_circle = c;
                    return;
                }
                if (e.X > (c.getX() - ANCHORSIZE) && e.X < (c.getX() + ANCHORSIZE) && e.Y > (c.getY() - c.getR() - ANCHORSIZE) && e.Y < (c.getY() - c.getR() + ANCHORSIZE))
                {
                    ModeSelect = Modes.UpdateRadius;
                    actual_circle = c;
                    return;
                }
            }

            if (ModeSelect == Modes.New)
            {
                actual_circle = new Circle(e.X, e.Y);
            }

        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (ModeSelect == Modes.New)
            {
                circles.Add(actual_circle);
            }
            buttondown = false; 
            fill(false);
            DrawCircles();
            canvas.DrawImage(backbuffer, 0, 0);
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (buttondown)
            {
                g.Clear(System.Drawing.Color.White);
                fill(true);

                if (ModeSelect == Modes.New || ModeSelect == Modes.UpdateRadius)
                {
                    actual_circle.update(e.X, e.Y);
                    DrawOneCircle(actual_circle);
                }
                if (ModeSelect == Modes.UpdateCenter)
                {
                    actual_circle.setCenter(e.X, e.Y);
                }

                DrawCircles();
                canvas.DrawImage(backbuffer, 0, 0);
            }
        }

        private void DrawCircles()
        {
            foreach(Circle c in circles){
                DrawOneCircle(c);
            }
        }

        private void DrawOneCircle(Circle c)
        {
            int topX = c.getX() - c.getR();
            int topY = c.getY() - c.getR();
            g.DrawEllipse(pen, topX, topY, c.getR() * 2, c.getR() * 2);
            g.DrawRectangle(pen, c.getX() - ANCHORSIZE, c.getY() - ANCHORSIZE, ANCHORSIZE * 2, ANCHORSIZE * 2);
            g.DrawRectangle(pen, c.getX() - ANCHORSIZE, c.getY() - c.getR() - 5, ANCHORSIZE * 2, ANCHORSIZE * 2);
        }

        private void fill(bool fast)
        {
            int step;

            if (fast)
            {
                step = 5;
            }
            else
            {
                step = 1;
            }
            for (int x = 0; x < backbuffer.Width; x += step)
            {
                for (int y = 0; y < backbuffer.Height; y += step)
                {
                    double sum = 0.0;
                    foreach (Circle c in circles)
                    {
                        sum += (double)c.getR() / Math.Sqrt( (c.getX() - x) * (c.getX() - x) + (c.getY() - y) * (c.getY() - y));
                    }

                    if (sum > cutoff)
                    {
                        backbuffer.SetPixel(x, y, System.Drawing.Color.Black);
                    }
                }
            }
        }

        private void Redraw()
        {
            g.Clear(System.Drawing.Color.White);
            fill(false);
            DrawCircles();
            canvas.DrawImage(backbuffer, 0, 0);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();
            string filename = saveFileDialog1.FileName;
            using (System.IO.StreamWriter output = new System.IO.StreamWriter(filename))
            {
                output.WriteLine(cutoff);
                foreach (Circle c in circles)
                {
                    output.WriteLine(c.getX() + " " + c.getY() + " " + c.getR());
                }
            }
        }

        private void LimitScroll_ValueChanged(object sender, EventArgs e)
        {
            cutoff = LimitScroll.Value / 10.0;

            Redraw();
        }

        private void undoToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            circles.RemoveAt(circles.Count - 1);

            Redraw();
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            circles.Clear();

            Redraw();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            string filename = openFileDialog1.FileName;
            circles.Clear();
            using (System.IO.StreamReader input = new System.IO.StreamReader(filename))
            {
                cutoff = Convert.ToDouble(input.ReadLine());
                while (!input.EndOfStream)
                {
                    string[] fields = input.ReadLine().Split(' ');
                    circles.Add(new Circle(Convert.ToInt32(fields[0]), Convert.ToInt32(fields[1]), Convert.ToInt32(fields[2])));
                }
            }
        }
    }
}
