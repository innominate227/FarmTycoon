using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;
using System.Drawing;

namespace FarmTycoon
{
    public class CostWindow : TycoonWindow
    {
        private int _cost;

        private TycoonLabel _costLabel;

        /// <summary>
        /// Player tresurey so we know if to make the label red or black
        /// </summary>
        private Treasury _treasury;

        public CostWindow(Treasury treasury)
        {
            _treasury = treasury;

            //make sure time window always stays in botom left
            Program.UserInterface.Graphics.Events.MouseMoved += new MouseEventHandler(Graphics_MouseMoved);
            _treasury.MoneyChanged += new Action(Treasury_MoneyChanged);

            //Window
            this.Width = 40;
            this.Height = 15;
            this.Left = 0;
            this.Top = 0;
            this.BackColor = Color.White;
            this.BorderColor = Color.Black;
            this.Resizable = false;
            this.Scrollable = false;
            this.TitleBar = false;
            this.Visible = true;

            //cost Label
            _costLabel = new TycoonLabel();
            _costLabel.Width = this.Width-2;
            _costLabel.Height = this.Height-2;
            _costLabel.Left = 0;
            _costLabel.Top = 0;
            _costLabel.BackColor = Color.White;
            _costLabel.BorderColor = Color.Transparent;            
            _costLabel.Text = "$10";
            _costLabel.TextColor = Color.Black;
            _costLabel.TextAlignment = StringAlignment.Center;
            _costLabel.TextVerticelAlignment = StringAlignment.Center;
            _costLabel.Visible = true;
            this.AddChild(_costLabel);
            
            Program.UserInterface.WindowManager.AddWindow(this);

            this.CloseClicked += new Action<TycoonWindow>(delegate
            {                
                Program.UserInterface.WindowManager.RemoveWindow(this);
            });
        }
        
        private void Treasury_MoneyChanged()
        {
            RefreshCost();
        }

        private void Graphics_MouseMoved(ClickInfo clickInfo)
        {
            this.Top = clickInfo.Y;
            this.Left = clickInfo.X + 20;
        }

        /// <summary>
        /// Get or set the cost 
        /// </summary>
        public int Cost
        {
            get { return _cost; }
            set 
            {
                _cost = value;
                RefreshCost();
            }
        }

        private void RefreshCost()
        {
            _costLabel.Text = "$" + _cost.ToString();
            if (_treasury.CurrentMoney - _cost >= 0)
            {
                _costLabel.TextColor = Color.Black;
            }
            else
            {
                _costLabel.TextColor = Color.Red;
            }
        }


    }
}
