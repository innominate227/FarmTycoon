using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;
using System.Drawing;

namespace FarmTycoon
{
    public partial class PreferredWorkersWindow : TycoonWindow
    {
        /// <summary>
        /// The task that preferred workers are being selected for
        /// </summary>
        private List<Worker> _preferredList;

        public PreferredWorkersWindow(List<Worker> preferredList)
        {
            InitializeComponent();
                        
            this.Top = (Program.UserInterface.Graphics.WindowHeight / 2) - (this.Height / 2);
            this.Left = (Program.UserInterface.Graphics.WindowWidth / 2) - (this.Width / 2);

            _preferredList = preferredList;

            CreateList();

            //this.OkButton.Clicked +=new Action<TycoonControl>(delegate{ CloseWindow(); });

            Program.UserInterface.Graphics.Events.MouseDown += new MouseEventHandler(Graphics_MouseDown);

            this.CloseClicked += new Action<TycoonWindow>(delegate
            {
                UpdateTask();
                Program.UserInterface.Graphics.Events.MouseDown -= new MouseEventHandler(Graphics_MouseDown);
                Program.UserInterface.WindowManager.RemoveWindow(this);                                
            });                 
            Program.UserInterface.WindowManager.AddWindow(this);
        }


        private void Graphics_MouseDown(ClickInfo clickInfo)
        {
            //close window if something that is not in this window was clicked            
            if (clickInfo.ControlClicked == null || (clickInfo.ControlClicked.ParentWindow != this && clickInfo.ControlClicked.Name != "PreferredWorkersButton"))
            {
                CloseWindow();
            }
        }
        
        private void CreateList()
        {
            int top = 0;
            foreach (Worker worker in GameState.Current.MasterObjectList.FindAll<Worker>())
            {
                PreferredWorkerPanel panel = new PreferredWorkerPanel();                
                panel.Top = top;
                panel.Left = 0;                
                panel.AnchorBottom = false;
                panel.AnchorTop = true;
                panel.AnchorLeft = true;
                panel.AnchorRight = false;
                panel.BorderColor = this.BackColor;
                panel.Worker = worker;
                panel.Selected = _preferredList.Contains(worker);
                panel.Visible = true;
                this.AddChild(panel);

                top += panel.Height;
            }
        }


        private void UpdateTask()
        {
            _preferredList.Clear();
            foreach (PreferredWorkerPanel panel in this.Children)
            {
                if (panel.Selected)
                {
                    _preferredList.Add(panel.Worker);
                }                
            }
        }
    

    }
}
