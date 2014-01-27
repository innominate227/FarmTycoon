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
    public partial class TycoonButton_Gen : Button
    {
        public TycoonButton_Gen()
        {
            InitializeComponent();

            this.Font = new Font("Segoe UI", 8, FontStyle.Regular);
            this.BackColor = Color.FromArgb(192, 64, 64);
            this.ForeColor = Color.White;
            this.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        }
        

        private string _toolTip = "";
        private double _toolTipTime = 1.0;
        private Color _shadowLightColor = Color.FromArgb(224, 128, 128);
        private Color _shadowDarkColor = Color.FromArgb(96, 32, 0);
        private string _iconTexture = "";
        private bool _depressed = false;
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
        /// The string on the button
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
        /// The font of the button text
        /// </summary>
        public Font Tycoon_TextFont
        {
            get { return this.Font; }
        }

        /// <summary>
        /// The alignment of the label text
        /// </summary>
        public StringAlignment Tycoon_TextAlignment
        {
            get
            {
                if (this.TextAlign == ContentAlignment.BottomLeft ||
                  this.TextAlign == ContentAlignment.MiddleLeft ||
                  this.TextAlign == ContentAlignment.TopLeft)
                {
                    return StringAlignment.Near;
                }
                if (this.TextAlign == ContentAlignment.BottomRight ||
                    this.TextAlign == ContentAlignment.MiddleRight ||
                    this.TextAlign == ContentAlignment.TopRight)
                {
                    return StringAlignment.Far;
                }
                else
                {
                    return StringAlignment.Center;
                }
            }
        }

        /// <summary>
        /// The verticle alignment of the label text
        /// </summary>
        public StringAlignment Tycoon_TextVerticelAlignment
        {
            get 
            {
                if (this.TextAlign == ContentAlignment.BottomCenter ||
                    this.TextAlign == ContentAlignment.BottomLeft ||
                    this.TextAlign == ContentAlignment.BottomRight)
                {
                    return StringAlignment.Far;
                }
                if (this.TextAlign == ContentAlignment.TopCenter ||
                    this.TextAlign == ContentAlignment.TopLeft ||
                    this.TextAlign == ContentAlignment.TopRight)
                {
                    return StringAlignment.Near;
                }
                else
                {
                    return StringAlignment.Center;
                }
            }            
        }

        /// <summary>
        /// Main color of the button
        /// </summary>
        public Color Tycoon_BackColor
        {
            get { return this.BackColor; }
        }

        /// <summary>
        /// Color of the drop shaodw above and to the left of the button
        /// </summary>
        public Color Tycoon_ShadowLightColor
        {
            get { return _shadowLightColor; }
            set { _shadowLightColor = value; }
        }

        /// <summary>
        /// Color of the drop shaodw to the right and below the button
        /// </summary>
        public Color Tycoon_ShadowDarkColor
        {
            get { return _shadowDarkColor; }
            set { _shadowDarkColor = value; }
        }

        /// <summary>
        /// The icon texture used for the button.
        /// </summary>
        public string Tycoon_IconTexture
        {
            get { return _iconTexture; }
            set { _iconTexture = value; }
        }

        /// <summary>
        /// is the button depressed
        /// </summary>
        public bool Tycoon_Depressed
        {
            get { return _depressed; }
            set { _depressed = value; }
        }




    }
}
