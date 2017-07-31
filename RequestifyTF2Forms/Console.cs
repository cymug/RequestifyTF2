﻿using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using RequestifyTF2Forms.Properties;

namespace RequestifyTF2Forms
{
    public partial class Console : Form
    {
        public Console()
        {
            InitializeComponent();
            Icon = Resources._1481916367_letter_r_red;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
        }

        private void Thanks_Load(object sender, EventArgs e)
        {
            FormBorderStyle = FormBorderStyle.None;
            var xs = Main.instance.Location.X + 50 + Main.instance.Height;
            var ys = Main.instance.Location.Y + 1;
            ThreadHelperClass.Position(this, this, new Point(xs, ys));
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;

                while (true)
                {
                    if (ActiveForm == this) continue;
                    try
                    {
                        var x = Main.instance.Location.X + 50 + Main.instance.Height;
                        var y = Main.instance.Location.Y + 1;
                        ThreadHelperClass.Position(this, this, new Point(x, y));
                    }
                    catch (Exception)
                    {
                        //ignored
                    }
                }
            }).Start();
        }

        public static class ThreadHelperClass
        {
            /// <summary>
            ///     Set text property of various controls
            /// </summary>
            /// <param name="form">The calling form</param>
            /// <param name="ctrl"></param>
            /// <param name="pos"></param>
            public static void Position(Form form, Control ctrl, Point pos)
            {
                // InvokeRequired required compares the thread ID of the 
                // calling thread to the thread ID of the creating thread. 
                // If these threads are different, it returns true. 
                if (ctrl.InvokeRequired)
                {
                    SetPosCallback d = Position;
                    try
                    {
                        form.Invoke(d, form, ctrl, pos);
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }
                else
                {
                    ctrl.Location = pos;
                }
            }

            private delegate void SetPosCallback(Form f, Control ctrl, Point p);
        }
    }
}