using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TycoonWindowGenerationLib
{
    public partial class TycoonTextbox_Gen : TextBox
    {
        public TycoonTextbox_Gen()
        {
            InitializeComponent();

            this.Font = new Font("Segoe UI", 8, FontStyle.Regular);
            this.BackColor = Color.White;
            this.ForeColor = Color.Black;
            this.Multiline = true;
        }



        private string _toolTip = "";
        private double _toolTipTime = 1.0;
        private Color _borderColor = Color.FromArgb(224, 128, 128);
        private bool _numbersOnly = false;
        private int _maxLenght = int.MaxValue;
        private bool _visible = true;


        /// <summary>
        /// Name Tag for the control
        /// </summary>
        public string Tycoon_Name
        {
            get { return this.Name; }
        }

        /// <summary>
        /// Tag for the control, has no function.
        /// </summary>
        public object Tycoon_Tag
        {
            get { return this.Tag; }
        }

        /// <summary>
        /// Is the window visible
        /// </summary>
        public bool Tycoon_Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }

        /// <summary>
        /// Width of the control
        /// </summary>
        public int Tycoon_Width
        {
            get { return this.Width; }
        }

        /// <summary>
        /// Height of the control
        /// </summary>
        public virtual int Tycoon_Height
        {
            get { return this.Height; }
        }

        /// <summary>
        /// Top position of the control
        /// </summary>
        public virtual int Tycoon_Top
        {
            get { return this.Top; }
        }

        /// <summary>
        /// Left position of the control
        /// </summary>
        public virtual int Tycoon_Left
        {
            get { return this.Left; }
        }

        /// <summary>
        /// Anchor to the top of the window
        /// </summary>
        public bool Tycoon_AnchorTop
        {
            get { return ((int)this.Anchor & (int)AnchorStyles.Top) != 0; }
        }

        /// <summary>
        /// Anchor to the left of the window
        /// </summary>
        public bool Tycoon_AnchorLeft
        {
            get { return ((int)this.Anchor & (int)AnchorStyles.Left) != 0; }
        }

        /// <summary>
        /// Anchor to the right of the window
        /// </summary>
        public bool Tycoon_AnchorRight
        {
            get { return ((int)this.Anchor & (int)AnchorStyles.Right) != 0; }
        }

        /// <summary>
        /// Anchor to the bottom of the window
        /// </summary>
        public bool Tycoon_AnchorBottom
        {
            get { return ((int)this.Anchor & (int)AnchorStyles.Bottom) != 0; }
        }


        /// <summary>
        /// Tool tip for the control
        /// </summary>
        public string Tycoon_Tooltip
        {
            get { return _toolTip; }
            set { _toolTip = value; }
        }

        /// <summary>
        /// Time to wait until bringing up the tool tip
        /// </summary>
        public double Tycoon_TooltipTime
        {
            get { return _toolTipTime; }
            set { _toolTipTime = value; }
        }







        /// <summary>
        /// The string in the textbox
        /// </summary>
        public string Tycoon_Text
        {
            get { return this.Text; }
        }

        /// <summary>
        /// The color of the button text
        /// </summary>
        public Color Tycoon_TextColor
        {
            get { return this.ForeColor; }
        }

        /// <summary>
        /// The font of the textbox text
        /// </summary>
        public Font Tycoon_TextFont
        {
            get { return this.Font; }
        }

        /// <summary>
        /// Main color of the button
        /// </summary>
        public Color Tycoon_BackColor
        {
            get { return this.BackColor; }
        }

        /// <summary>
        /// Color of the border of the textbox
        /// </summary>
        public Color Tycoon_BorderColor
        {
            get { return _borderColor; }
            set { _borderColor = value; }
        }

        /// <summary>
        /// Should the box only allow numbers
        /// </summary>
        public bool Tycoon_NumbersOnly
        {
            get { return _numbersOnly; }
            set { _numbersOnly = value; }
        }

        /// <summary>
        /// the maximum number of characters in the box
        /// </summary>
        public int Tycoon_MaxLenght
        {
            get { return _maxLenght; }
            set { _maxLenght = value; }
        }



    }
}
