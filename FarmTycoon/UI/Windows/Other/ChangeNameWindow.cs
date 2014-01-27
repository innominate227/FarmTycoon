using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;
using System.Drawing;

namespace FarmTycoon
{
    public partial class ChangeNameWindow : TycoonWindow
    {   
        private GameObject _changeNameOf;

        public ChangeNameWindow(GameObject changeNameOf)
        {
            InitializeComponent();

            _changeNameOf = changeNameOf;
            nameTextbox.Text = _changeNameOf.Name;

            this.TitleText = "Change Name of " + changeNameOf.Name;

            okButton.Clicked += new Action<TycoonControl>(OkButton_Clicked);
            cancelButton.Clicked += new Action<TycoonControl>(delegate { CloseWindow(); });
            Program.UserInterface.Graphics.Events.MouseDown += new MouseEventHandler(Graphics_MouseDown);

            //add window to game
            Program.UserInterface.WindowManager.AddWindow(this);


            this.CloseClicked += new Action<TycoonWindow>(delegate
            {
                Program.UserInterface.Graphics.Events.MouseDown -= new MouseEventHandler(Graphics_MouseDown);
                Program.UserInterface.WindowManager.RemoveWindow(this); 	        
            });
        }

        private void Graphics_MouseDown(ClickInfo clickInfo)
        {
            //close window if something that is not in this window was clicked
            //dont close if event is from pie menu button clicked to create this window, or from control clicked in dropbox window
            if (clickInfo.ControlClicked == null || (clickInfo.ControlClicked.ParentWindow != this && clickInfo.ControlClicked.ParentWindow is PieMenuWindow == false && clickInfo.ControlClicked.ParentWindow.TitleText != "Dropbox"))
            {
                CloseWindow();
            }
        }
        
        private void OkButton_Clicked(TycoonControl obj)
        {
            _changeNameOf.Name = nameTextbox.Text;
            CloseWindow();
        }

    }
}
