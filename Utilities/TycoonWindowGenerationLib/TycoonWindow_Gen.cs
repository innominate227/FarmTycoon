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
    public partial class TycoonWindow_Gen : Panel
    {
        public TycoonWindow_Gen()
        {
            InitializeComponent();
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.BackColor = Color.FromArgb(192, 64, 64);
            this.ForeColor = Color.White;
        }



        private string m_toolTip = "";
        private double m_toolTipTime = 1.0;
        private Color m_borderColor = Color.FromArgb(224, 128, 128);
        private Color m_scrollColorLight = Color.FromArgb(224, 128, 128);
        private Color m_scrollColorDark = Color.FromArgb(96, 32, 0);
        private bool m_alwaysShowScroll = false;
        private int m_scrollPosition = 0;
        private string m_scrollUpTexture = "arrowup";
        private string m_scrollDownTexture = "arrowdown";
        private string m_title = "Window";
        private Color m_titleTextColor = Color.White;
        private Font m_titleFont = new Font("Segoe UI", 8, FontStyle.Regular);
        private StringAlignment m_titleAlign = StringAlignment.Near;
        private Color m_titleBarColor = Color.FromArgb(96, 32, 0);
        private bool m_titlebar = true;
        private int m_minimumWidth = 0;
        private int m_minimumHeight = 0;
        private int m_maximumWidth = int.MaxValue;
        private int m_maximumHeight = int.MaxValue;
        private bool m_resizable = true;
        private bool m_visible = true;


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
            get { return m_visible; }
            set { m_visible = value; }
        }

        /// <summary>
        /// Width of the control
        /// </summary>
        public int Tycoon_Width
        {
            get 
            {
                return this.Width; 
            }
        }

        /// <summary>
        /// Height of the control
        /// </summary>
        public virtual int Tycoon_Height
        {
            get
            {
                if (m_titlebar)
                {
                    return this.Height + 12;
                }
                return this.Height; 
            }
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
            get { return m_toolTip; }
            set { m_toolTip = value; }
        }
        
        /// <summary>
        /// Time to wait until bringing up the tool tip
        /// </summary>
        public double Tycoon_TooltipTime
        {
            get { return m_toolTipTime; }
            set { m_toolTipTime = value; }
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
            get { return m_borderColor; }
            set { m_borderColor = value; }            
        }

        /// <summary>
        /// Color for the light part of the scroll bar shadow
        /// </summary>
        public Color Tycoon_ScrollLightColor
        {
            get { return m_scrollColorLight; }
            set { m_scrollColorLight = value; }
        }

        /// <summary>
        /// Color for the dark part of the scroll bar shadow
        /// </summary>
        public Color Tycoon_ScrollDarkColor
        {
            get { return m_scrollColorDark; }
            set { m_scrollColorDark = value; }
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
            get { return m_alwaysShowScroll; }
            set { m_alwaysShowScroll = value; }
        }

        /// <summary>
        /// Should the panel have a border
        /// </summary>
        public bool Tycoon_Border
        {
            get { return this.BorderStyle != System.Windows.Forms.BorderStyle.None; }            
        }
        

        /// <summary>
        /// The area of the scrollable panel being shown.  ( Controls with Top=ScrollPos will be seen at the very top of the window)
        /// </summary>
        public int Tycoon_ScrollPosition
        {
            get { return m_scrollPosition; }
            set { m_scrollPosition = value; }
        }

        /// <summary>
        /// The texture for the scroll up button
        /// </summary>
        public string Tycoon_ScrollUpTexture
        {
            get { return m_scrollUpTexture; }
            set { m_scrollUpTexture = value; }
        }

        /// <summary>
        /// The texture for the scroll down button
        /// </summary>
        public string Tycoon_ScrollDownTexture
        {
            get { return m_scrollDownTexture; }
            set { m_scrollDownTexture = value; }
        }















        /// <summary>
        /// String for the title text of the window
        /// </summary>
        public string Tycoon_TitleText
        {
            get { return m_title; }
        }

        /// <summary>
        /// Color for the title text of the window
        /// </summary>
        public Color Tycoon_TitleTextColor
        {
            get { return m_titleTextColor; }
            set { m_titleTextColor = value; }
        }

        /// <summary>
        /// Font for the title text of the window
        /// </summary>
        public Font Tycoon_TitleTextFont
        {
            get { return m_titleFont; }
            set { m_titleFont = value; }
        }

        /// <summary>
        /// The alignment of the label text
        /// </summary>
        public StringAlignment Tycoon_TitleTextAlignment
        {
            get { return m_titleAlign; }
            set { m_titleAlign = value; }
        }

        /// <summary>
        /// Color of the windows title bar
        /// </summary>
        public Color Tycoon_TitleBarColor
        {
            get { return m_titleBarColor; }
            set { m_titleBarColor = value;  }
        }

        /// <summary>
        /// True if the windows title bar should be drawn
        /// </summary>
        public bool Tycoon_TitleBar
        {
            get { return m_titlebar; }
            set { m_titlebar = value; }
        }

        /// <summary>
        /// The minimum width for the window
        /// </summary>
        public int Tycoon_MinimumWidth
        {
            get { return m_minimumWidth; }
            set
            {
                m_minimumWidth = value;
            }
        }

        /// <summary>
        /// The minimum height for the window
        /// </summary>
        public int Tycoon_MinimumHeight
        {
            get { return m_minimumHeight; }
            set
            {
                m_minimumHeight = value;
            }
        }

        /// <summary>
        /// The maximum width for the window
        /// </summary>
        public int Tycoon_MaximumWidth
        {
            get { return m_maximumWidth; }
            set
            {
                m_maximumWidth = value;
            }
        }

        /// <summary>
        /// The maximum height for the window
        /// </summary>
        public int Tycoon_MaximumHeight
        {
            get { return m_maximumHeight; }
            set
            {
                m_maximumHeight = value;
            }
        }

        /// <summary>
        /// Is the window resizable
        /// </summary>
        public bool Tycoon_Resizable
        {
            get { return m_resizable; }
            set { m_resizable = value; }
        }


    }
}
