using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;
using System.Drawing;

namespace FarmTycoon
{
    public partial class DebugToolWindow: TycoonWindow
    {
        private static DebugToolWindow _window;
        

        public static string VisibleLayers = "All Layers";
        public static string SoloNotifications = "";
        public static string DesiredRate = "";
        public static string ActualRate = "";
        public static int CacheHits = 0;
        public static int CacheMiss = 0;


        public DebugToolWindow()
        {
            InitializeComponent();

            this.Top = 375;
            this.Left = 0;


            Program.UserInterface.WindowManager.AddWindow(this);
            

            TycoonGraphicsLib.GraphicsDebug.ValueChanged += new Action(Refresh);

            GameButton.Clicked += new Action<TycoonControl>(delegate
            {
                GameButton.Depressed = true;
                GraphicsButton.Depressed = false;
                Refresh();
            });

            GraphicsButton.Clicked += new Action<TycoonControl>(delegate
            {
                GameButton.Depressed = false;
                GraphicsButton.Depressed = true;
                Refresh();
            });

            DoorsButton.Clicked += new Action<TycoonControl>(delegate
            {
                DoorsButton.Depressed = !DoorsButton.Depressed;

                if (DoorsButton.Depressed)
                {
                    Program.Game.PathFinder.Debug_MarkDoors();
                }
                else
                {
                    foreach (Land land in GameState.Current.MasterObjectList.FindAll<Land>())
                    {
                        land.CornerToHighlight = LandCorner.None;
                    }
                }

            });


            GameButton.Depressed = true;
            GraphicsButton.Depressed = false;
            Refresh();


            Program.GameThread.RefreshTimePassed += new Action(Refresh);

            this.CloseClicked += new Action<TycoonWindow>(delegate
            {
                Program.UserInterface.WindowManager.RemoveWindow(this);
                Program.GameThread.RefreshTimePassed -= new Action(Refresh);
            });

            _window = this;
        }

               


        public void Refresh()
        {
            if (GameButton.Depressed)
            {
                debugLabel1.Text = DesiredRate;
                debugLabel2.Text = ActualRate;
                debugLabel3.Text = "";
                debugLabel4.Text = "";
                debugLabel5.Text = SoloNotifications;
                debugLabel6.Text = "";
                debugLabel7.Text = "";
                debugLabel8.Text = "";
                debugLabel9.Text = "Hit: " + CacheHits.ToString() + " Mis:" + CacheMiss.ToString();
                debugLabel10.Text = "";
                debugLabel11.Text = "";
                debugLabel12.Text = VisibleLayers;
            }
            else if (GraphicsButton.Depressed)
            {
                debugLabel1.Text = TycoonGraphicsLib.GraphicsDebug.Debug1;
                debugLabel2.Text = TycoonGraphicsLib.GraphicsDebug.Debug2;
                debugLabel3.Text = TycoonGraphicsLib.GraphicsDebug.Debug3;
                debugLabel4.Text = TycoonGraphicsLib.GraphicsDebug.Debug4;
                debugLabel5.Text = TycoonGraphicsLib.GraphicsDebug.Debug5;
                debugLabel6.Text = TycoonGraphicsLib.GraphicsDebug.Debug6;
                debugLabel7.Text = TycoonGraphicsLib.GraphicsDebug.Debug7;
                debugLabel8.Text = TycoonGraphicsLib.GraphicsDebug.Debug8;
                debugLabel9.Text = TycoonGraphicsLib.GraphicsDebug.Debug9;
                debugLabel10.Text = TycoonGraphicsLib.GraphicsDebug.Debug10;
                debugLabel11.Text = TycoonGraphicsLib.GraphicsDebug.Debug11;
                debugLabel12.Text = TycoonGraphicsLib.GraphicsDebug.Debug12;
            }
        }
        
    }
}
