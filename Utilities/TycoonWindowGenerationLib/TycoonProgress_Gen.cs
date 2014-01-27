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
    public partial class TycoonProgress_Gen : ProgressBar
    {
        public TycoonProgress_Gen()
        {
            InitializeComponent();

            this.Font = new Font("Segoe UI", 8, FontStyle.Regular);
            this.BackColor = Color.FromArgb(192, 64, 64);
            this.ForeColor = Color.White;
        }



        private string _text = "";
        private string _toolTip = "";
        private double _toolTipTime = 1.0;
        private Color _borderColor = Color.FromArgb(224, 128, 128);        
        private Color _progressColor = Color.FromArgb(96, 32, 0);        
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
        /// The alignment of the label text
        /// </summary>
        public StringAlignment Tycoon_TextAlignment
        {
            get
            {                
                return StringAlignment.Center;                
            }
        }

        /// <summary>
        /// The verticle alignment of the label text
        /// </summary>
        public StringAlignment Tycoon_TextVerticelAlignment
        {
            get
            {                
                return StringAlignment.Center;                
            }
        }





        /// <summary>
        /// number between 0 and MaxValue that tells the progress
        /// </summary>
        public int Tycoon_Progress
        {
            get { return this.Value; }            
        }


        /// <summary>
        /// max value for the progress bar
        /// </summary>
        public int Tycoon_MaxValue
        {
            get { return this.Maximum; }
        }

        
        /// <summary>
        /// Progress color
        /// </summary>
        public Color Tycoon_ProgressColor
        {
            get { return _progressColor; }
            set { _progressColor = value; }
        }

        /// <summary>
        /// Color of the drop shaodw above and to the left of the button
        /// </summary>
        public Color Tycoon_BorderColor
        {
            get { return _borderColor; }
            set { _borderColor = value; }
        }

        /// <summary>
        /// Text for the tycoon progress
        /// </summary>
        public string Tycoon_Text
        {
            get { return _text; }
            set { _text = value; }
        }

    }
}
