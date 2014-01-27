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
    public partial class TycoonPanel_UserControl_Gen : UserControl
    {
        public TycoonPanel_UserControl_Gen()
        {
            InitializeComponent();

            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.BackColor = Color.FromArgb(192, 64, 64);
            this.ForeColor = Color.White;
        }



        private string _toolTip = "";
        private double _toolTipTime = 1.0;
        private Color _borderColor = Color.FromArgb(224, 128, 128);
        private Color _scrollColorLight = Color.FromArgb(224, 128, 128);
        private Color _scrollColorDark = Color.FromArgb(96, 32, 0);
        private bool _alwaysShowScroll = false;
        private int _scrollPosition = 0;
        private string _scrollUpTexture = "arrowup";
        private string _scrollDownTexture = "arrowdown";
        private bool _visible = true;
        private bool _border = true;


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
        /// Primary color.
        /// </summary>
        public Color Tycoon_BackColor
        {
            get { return this.BackColor; }
        }
        
        /// <summary>
        /// Color for the border
        /// </summary>
        public Color Tycoon_BorderColor
        {
            get { return _borderColor; }
            set { _borderColor = value; }            
        }

        /// <summary>
        /// Color for the light part of the scroll bar shadow
        /// </summary>
        public Color Tycoon_ScrollLightColor
        {
            get { return _scrollColorLight; }
            set { _scrollColorLight = value; }
        }

        /// <summary>
        /// Color for the dark part of the scroll bar shadow
        /// </summary>
        public Color Tycoon_ScrollDarkColor
        {
            get { return _scrollColorDark; }
            set { _scrollColorDark = value; }
        }

        /// <summary>
        /// Should the panel be scrollable (a scroll bar will always be shown)
        /// </summary>
        public bool Tycoon_Scrollable
        {
            get { return this.AutoScroll; }
        }

        /// <summary>
        /// Should the panel always show the scroll bar (even when theres nothing to scroll)
        /// </summary>
        public bool Tycoon_AlwaysShowScroll
        {
            get { return _alwaysShowScroll; }
            set { _alwaysShowScroll = value; }
        }

        /// <summary>
        /// Should the panel have a border
        /// </summary>
        public bool Tycoon_Border
        {
            get { return _border; }            
            set { _border = value; }            
        }
        

        /// <summary>
        /// The area of the scrollable panel being shown.  ( Controls with Top=ScrollPos will be seen at the very top of the window)
        /// </summary>
        public int Tycoon_ScrollPosition
        {
            get { return _scrollPosition; }
            set { _scrollPosition = value; }
        }

        /// <summary>
        /// The texture for the scroll up button
        /// </summary>
        public string Tycoon_ScrollUpTexture
        {
            get { return _scrollUpTexture; }
            set { _scrollUpTexture = value; }
        }

        /// <summary>
        /// The texture for the scroll down button
        /// </summary>
        public string Tycoon_ScrollDownTexture
        {
            get { return _scrollDownTexture; }
            set { _scrollDownTexture = value; }
        }


    }
}
