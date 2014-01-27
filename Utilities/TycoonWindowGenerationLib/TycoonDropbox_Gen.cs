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
    public partial class TycoonDropbox_Gen : ComboBox
    {
        public TycoonDropbox_Gen()
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
        private Color _selectionColor = Color.Blue;
        private Color _dropTextColor = Color.White;
        private string _dropArrowTexture = "arrowdown";
        private int _dropHeight = 50;
        private StringAlignment _align = StringAlignment.Near;
        private StringAlignment _alignVert = StringAlignment.Near;
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
            get { return _align; }            
            set { _align = value; }
        }

        /// <summary>
        /// The verticle alignment of the label text
        /// </summary>
        public StringAlignment Tycoon_TextVerticelAlignment
        {
            get { return _alignVert; }
            set { _alignVert = value; }
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
        /// Color of the item selected in the drop box
        /// </summary>
        public Color Tycoon_SelectionColor
        {
            get { return _selectionColor; }
            set { _selectionColor = value; }
        }


        /// <summary>
        /// Color of the text in the dropped portion of the dropbox
        /// </summary>
        public Color Tycoon_DropTextColor
        {
            get { return _dropTextColor; }
            set { _dropTextColor = value; }
        }

        /// <summary>
        /// The texture for the drop arrow
        /// </summary>
        public string Tycoon_DropArrowTexture
        {
            get { return _dropArrowTexture; }
            set { _dropArrowTexture = value; }
        }

        /// <summary>
        /// The height of the drop down region
        /// </summary>
        public int Tycoon_DropHeight
        {
            get { return _dropHeight; }
            set { _dropHeight = value; }
        }

        /// <summary>
        /// List of drop box items
        /// </summary>
        public List<string> Tycoon_Items
        {
            get 
            {
                List<string> toRet = new List<string>();
                foreach (string item in this.Items)
                {
                    toRet.Add(item);
                }
                return toRet;
            }
        }



    }
}
