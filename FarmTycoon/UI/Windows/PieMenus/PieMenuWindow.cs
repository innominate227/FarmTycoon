using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;
using System.Drawing;

namespace FarmTycoon
{
    public partial class PieMenuWindow : TycoonWindow
    {
        const int PIE_BUTTON_SIZE = 30;

        public event Action<string, Point> ToolClicked;
        private Dictionary<string, TycoonButton> _toolButtons = new Dictionary<string, TycoonButton>();

        public void Init(string[] tools, Point centerPoint)
        {
            InitializeComponent();
            
            //tool count
            int toolCount = tools.Length;

            //determine radius of the pie circle
            double distanceBetweenPieButtons = Math.Sqrt(Math.Pow(PIE_BUTTON_SIZE, 2) + Math.Pow(PIE_BUTTON_SIZE, 2));
            double pieCircleRadius = distanceBetweenPieButtons / (2.0 * Math.Sin(Math.PI / toolCount));

            //size of the window can not be more than double the window
            this.Width = (int)(pieCircleRadius * 4);
            this.Height = (int)(pieCircleRadius * 4);
            int centerX = this.Width/2;
            int centerY = this.Height/2;

            //determine the center point for all the tools
            Point[] toolCenterPoints = new Point[toolCount];
            for (int i = 0; i < toolCount; i++)
            {
                double angle = (2 * Math.PI) * (i / (double)toolCount);
                toolCenterPoints[i].X = centerX + (int)(Math.Sin(angle) * pieCircleRadius);
                toolCenterPoints[i].Y = centerY - (int)(Math.Cos(angle) * pieCircleRadius);
            }
            
            //create a button for each tool
            for (int i = 0; i < toolCount; i++)
            {
                string tool = tools[i];

                TycoonButton toolButton = new TycoonButton();
                toolButton.Width = 30;
                toolButton.Height = 30;
                toolButton.Left = toolCenterPoints[i].X - (PIE_BUTTON_SIZE / 2);
                toolButton.Top = toolCenterPoints[i].Y - (PIE_BUTTON_SIZE / 2);
                toolButton.BackColor = Color.FromArgb(192, 64, 64);
                toolButton.ShadowDarkColor = Color.FromArgb(96, 32, 0);
                toolButton.ShadowLightColor = Color.FromArgb(224, 128, 128);
                toolButton.Text = tool;
                toolButton.Tag = tool;
                toolButton.TextColor = Color.White;
                toolButton.TextAlignment = StringAlignment.Center;
                toolButton.Visible = true;
                toolButton.Depressed = false;
                toolButton.Clicked += new Action<TycoonControl>(delegate(TycoonControl control)
                {
                    if (ToolClicked != null)
                    {
                        int xPosAbsolute, yPosAbsolute;
                        control.GetPositionAbsolute(out xPosAbsolute, out yPosAbsolute);
                        xPosAbsolute += control.Width / 2;
                        yPosAbsolute += control.Height / 2;

                        ToolClicked(control.Tag.ToString(), new Point(xPosAbsolute, yPosAbsolute));
                    }                    
                });

                this.AddChild(toolButton);
                _toolButtons.Add(tool, toolButton);
            }

            this.Top = centerPoint.Y - (this.Width / 2);
            this.Left = centerPoint.X - (this.Height / 2);
            this.BackColor = Color.Transparent;            
            Program.UserInterface.WindowManager.AddWindow(this);
            
            Program.UserInterface.Graphics.Events.MouseDown += new MouseEventHandler(Graphics_MouseDown);            
        }

        private void Graphics_MouseDown(ClickInfo clickInfo)
        {
            //on any mouse click the pie menu window should go away
            Program.UserInterface.Graphics.Events.MouseDown -= new MouseEventHandler(Graphics_MouseDown);
            Program.UserInterface.WindowManager.RemoveWindow(this);
        }
            
    


    }
}
