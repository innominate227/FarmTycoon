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
    public partial class TycoonGauge_Gen : Label
    {
        public TycoonGauge_Gen()
        {
            InitializeComponent();

            this.Font = new Font("Segoe UI", 6, FontStyle.Regular);
            this.BackColor = Color.FromArgb(192, 64, 64);
            this.ForeColor = Color.Black;
            this.AutoSize = false;
        }



        private string _toolTip = "";
        private double _toolTipTime = 1.0;
        private Color _borderColor = Color.FromArgb(0, 0, 0);
        private Color _goodColor = Color.FromArgb(181, 230, 29);
        private Color _midColor = Color.FromArgb(255, 242, 0);
        private Color _badColor = Color.FromArgb(241, 86, 91);

        private int _quality = 50;
        private int _value = 50;
        private int _maxValue = 100;
        private int _minValue = 0;

        private int _midColorStart= 25;
        private int _midColorEnd = 75;
        private int _goodColorStart = 40;
        private int _goodColorEnd = 60;

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
        /// Quality level to show in the gauge
        /// </summary>
        public int Tycoon_Quality
        {
            get { return _quality; }
            set { _quality = value; }
        }
        


        public Color Tycoon_BorderColor
        {
            get { return _borderColor; }
            set { _borderColor = value; }
        }


        public Color Tycoon_GoodColor
        {
            get { return _goodColor; }
            set { _goodColor = value; }
        }
        public Color Tycoon_MidColor
        {
            get { return _midColor; }
            set { _midColor = value; }
        }
        public Color Tycoon_BadColor
        {
            get { return _badColor; }
            set { _badColor = value; }
        }






        public int Tycoon_Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public int Tycoon_MinValue
        {
            get { return _minValue; }
            set { _minValue = value; }
        }

        public int Tycoon_MaxValue
        {
            get { return _maxValue; }
            set { _maxValue = value; }
        }


        public int Tycoon_MidColorStart
        {
            get { return _midColorStart; }
            set { _midColorStart = value; }
        }
        public int Tycoon_MidColorEnd
        {
            get { return _midColorEnd; }
            set { _midColorEnd = value; }
        }
        public int Tycoon_GoodColorStart
        {
            get { return _goodColorStart; }
            set { _goodColorStart = value; }
        }
        public int Tycoon_GoodColorEnd
        {
            get { return _goodColorEnd; }
            set { _goodColorEnd = value; }
        }


    }
}
