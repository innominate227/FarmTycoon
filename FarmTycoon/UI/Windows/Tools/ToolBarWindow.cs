using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;
using System.Drawing;

namespace FarmTycoon
{
    public partial class ToolBarWindow : TycoonWindow
    {
        /// <summary>
        /// Event raised when a tool is clicked
        /// </summary>
        public event Action<string, int> ToolClicked;

        /// <summary>
        /// The butons showing in the toolbar
        /// </summary>
        private Dictionary<string, TycoonButton> _toolButtons = new Dictionary<string, TycoonButton>();

        /// <summary>
        /// Create a toolbar window
        /// </summary>
        public void Init(string[] tools, int arrowLocation)
        {
            InitializeComponent();
            this.Width = 31;

            int top = 0;
            int left = 0;
            if (arrowLocation != -1)
            {
                this.Width = 41;
                left = 10;
                if (arrowLocation - tools.Length + 1 > 0)
                {
                    top = 35 * (arrowLocation - tools.Length + 1);
                }
            }


            //create a button for each tool
            foreach (string tool in tools)
            {
                TycoonButton toolButton = new TycoonButton();
                toolButton.Width = 31;
                toolButton.Height = 31;
                toolButton.Left = left;
                toolButton.Top = top;
                toolButton.BackColor = Color.FromArgb(192, 64, 64);
                toolButton.ShadowDarkColor = Color.FromArgb(96, 32, 0);
                toolButton.ShadowLightColor = Color.FromArgb(224, 128, 128);
                toolButton.Text = "";
                toolButton.IconTexture = tool + "_tool";
                toolButton.Tag = tool;
                toolButton.TextColor = Color.White;
                toolButton.TextAlignment = StringAlignment.Center;
                toolButton.Visible = true;
                toolButton.Depressed = false;
                toolButton.Clicked += new Action<TycoonControl>(delegate(TycoonControl control)
                {
                    if (ToolClicked != null)
                    {
                        ToolClicked(control.Tag.ToString(), control.Top / 35);
                    }
                });

                this.AddChild(toolButton);
                _toolButtons.Add(tool, toolButton);

                top += 35;
            }

            this.Height = top - 3;

            if (arrowLocation != -1)
            {
                //create arrow
                TycoonLabel arrowLabel = new TycoonLabel();
                arrowLabel.Width = 6;
                arrowLabel.Height = 6;
                arrowLabel.Left = 2;
                arrowLabel.Top = arrowLocation * 35 + 12;
                arrowLabel.BackColor = Color.FromArgb(224, 128, 128);
                arrowLabel.BorderColor = Color.FromArgb(224, 128, 128);                                
                arrowLabel.Text = "";
                arrowLabel.Visible = true;
                this.AddChild(arrowLabel);
            }
                                  

            this.BackColor = Color.Transparent;            
            Program.UserInterface.WindowManager.AddWindow(this);
        }

        /// <summary>
        /// Determine if this is a main toolbar
        /// </summary>
        public bool IsMainToolbar()
        {
            //determine by checking size (sub toolbars are 40 pixels wide because they have to dot between them and the main toolbar or another sub toolbar)
            return (this.Width != 41);
        }

        public void SelectTool(string tool)
        {
            foreach (TycoonButton toolButton in _toolButtons.Values)
            {
                toolButton.Depressed = false;
            }
            if (tool != null && tool != "")
            {
                _toolButtons[tool].Depressed = true;
            }
        }

        public string ToolSelected
        {
            get
            {
                foreach (TycoonButton toolButton in _toolButtons.Values)
                {
                    if (toolButton.Depressed)
                    {
                        return toolButton.Tag.ToString();
                    }
                }
                return "";
            }
        }


    }
}
