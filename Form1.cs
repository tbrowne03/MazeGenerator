using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Testing
{
    public partial class MazeGen : Form
    {
        SolidBrush brush_blue = new SolidBrush(Color.Blue);
        SolidBrush brush_black = new SolidBrush(Color.Black);
        SolidBrush brush_gold = new SolidBrush(Color.Gold);
        Pen pen = new Pen(Color.Black, 2);
        Font myFont = new Font("Arial", 14);

        static int spacing = 50, circleSize = 10;
        double offset_X, offset_Y, origin_X, origin_Y, mouseDown_deltaX, mouseDown_deltaY;
        bool mouseDown, showWeights = true;
        Point start, end;
        List<Point> visitedPoints;
        int[,] weightedGraph;

        int mazeSize = 6;

        public MazeGen()
        {
            InitializeComponent();
            offset_X = (pictureBox1.Size.Width / 2)  - ((mazeSize * spacing) / 2);
            offset_Y = (pictureBox1.Size.Height / 2) - ((mazeSize * spacing) / 2);
            mouseDown = false;

            NewMaze();
            Refresh();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            spacing = 50;
            circleSize = 10;
            offset_X = (pictureBox1.Size.Width / 2) - ((mazeSize * spacing) / 2);
            offset_Y = (pictureBox1.Size.Height / 2) - ((mazeSize * spacing) / 2);
            Refresh();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            mazeSize = Convert.ToInt16(numericUpDown1.Value);
            NewMaze();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            NewMaze();
            Refresh();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            showWeights = !showWeights;
            Refresh();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            //e.Graphics.ScaleTransform(1.0F, -1.0F);
            //e.Graphics.TranslateTransform(0.0F, -Height);

            int upperLimitX = pictureBox1.Size.Width  - ((mazeSize - 1) * spacing) - (circleSize / 2);
            int upperLimitY = pictureBox1.Size.Height - ((mazeSize - 1) * spacing) - (circleSize / 2);
            int lowerLimitX = 0 + (circleSize / 2);
            int lowerLimitY = 0 + (circleSize / 2);

            offset_X = (offset_X > upperLimitX) ? upperLimitX : offset_X;
            offset_X = (offset_X < lowerLimitX) ? lowerLimitX : offset_X;
            offset_Y = (offset_Y > upperLimitY) ? upperLimitY : offset_Y;
            offset_Y = (offset_Y < lowerLimitY) ? lowerLimitY : offset_Y;

            origin_X = offset_X + ((mazeSize - 1) * spacing / 2);
            origin_Y = offset_Y + ((mazeSize - 1) * spacing / 2);

            for (int i = 0; i < mazeSize; i++)
            {
                for (int j = 0; j < mazeSize; j++)
                {
                    if (i == start.Y && j == start.X)
                    {
                        e.Graphics.FillEllipse(
                            brush_black,
                            (j * spacing) - (circleSize / 2) + (float)offset_X,
                            (i * spacing) - (circleSize / 2) + (float)offset_Y,
                            circleSize,
                            circleSize
                            );

                        if (showWeights)
                        {
                            e.Graphics.DrawString(
                                weightedGraph[j, i].ToString(),
                                myFont,
                                Brushes.Black,
                                new PointF((j * spacing) + (float)offset_X, (i * spacing) + (float)offset_Y - 10)
                                );
                        }
                    }
                    else if(i == end.Y && j == end.X)
                    {
                        e.Graphics.FillEllipse(
                            brush_gold,
                            (j * spacing) - (circleSize / 2) + (float)offset_X,
                            (i * spacing) - (circleSize / 2) + (float)offset_Y,
                            circleSize,
                            circleSize
                            );

                        if (showWeights)
                        {
                            e.Graphics.DrawString(
                             weightedGraph[j, i].ToString(),
                             myFont,
                             Brushes.Black,
                             new PointF((j * spacing) + (float)offset_X, (i * spacing) + (float)offset_Y - 10)
                             );
                        }
                    }
                    else
                    {
                        e.Graphics.FillEllipse(
                            brush_blue,
                            (j * spacing) - (circleSize / 2) + (float)offset_X,
                            (i * spacing) - (circleSize / 2) + (float)offset_Y,
                            circleSize,
                            circleSize
                            );

                        if (showWeights)
                        {
                            e.Graphics.DrawString(
                             weightedGraph[j, i].ToString(),
                             myFont,
                             Brushes.Black,
                             new PointF((j * spacing) + (float)offset_X, (i * spacing) + (float)offset_Y - 10)
                             );
                        }
                    }
                }
            }

            List<PointF> adjustedPoints = new List<PointF>();

            for (int i = 0; i < visitedPoints.Count; i++)
            {
                float adjusted_X = (visitedPoints[i].X * spacing) + (float)offset_X;
                float adjusted_Y = (visitedPoints[i].Y * spacing) + (float)offset_Y;
                adjustedPoints.Add(new PointF(adjusted_X, adjusted_Y));
            }

            for (int i = 0; i < adjustedPoints.Count - 1; i++)
            {
                GraphicsPath mazePath = new GraphicsPath();
                mazePath.StartFigure();
                mazePath.AddLine(adjustedPoints[i], adjustedPoints[i + 1]);
                e.Graphics.DrawPath(pen, mazePath);
            }

        }
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            mouseDown_deltaX = e.X - offset_X;
            mouseDown_deltaY = e.Y - offset_Y;
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                offset_X = e.X - mouseDown_deltaX;
                offset_Y = e.Y - mouseDown_deltaY;

                Refresh();
            }
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 'w')
            {
                offset_Y += 30;
            }
            else if (e.KeyChar == 's')
            {
                offset_Y -= 30;
            }
            else if (e.KeyChar == 'a')
            {
                offset_X -= 30;
            }
            else if (e.KeyChar == 'd')
            {
                offset_X += 30;
            }
            else if (e.KeyChar == (char)Keys.Enter)
            {
                NewMaze();
            }

            Refresh();
        }

        private void Form1_MouseWheel(object sender, MouseEventArgs e)
        {
            int value = e.Delta * SystemInformation.MouseWheelScrollLines / 120;
            spacing += value;
            circleSize += value;

            if (e.Delta < 0)
            {
                offset_X += (e.X - origin_X) * 0.1;
                offset_Y += (e.Y - origin_Y) * 0.1;
            }
            else
            {
                offset_X -= (e.X - origin_X) * 0.1;
                offset_Y -= (e.Y - origin_Y) * 0.1;
            }

            Refresh();
        }

        private void NewMaze()
        {
            Maze maze = new Maze(mazeSize);
            maze.Run();
            start = maze.start;
            end = maze.end;
            visitedPoints = maze.visitedPoints;
            weightedGraph = maze.weightedGraph;
            Refresh();
        }
    }
}
