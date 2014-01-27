using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;
using System.Drawing;

namespace FarmTycoon
{
    public partial class FieldWindow : TycoonWindow
    {
        
        /// <summary>
        /// Field being shown in the window
        /// </summary>
        private Field m_field;
        
        
        public FieldWindow(Field field)
        {
            m_field = field;  

            InitializeComponent();

            //pass field to panels            
            fieldViewPanel.SetField(field);
            fieldStatsPanel.SetField(field);
            fieldCalandarPanel.SetFilter(delegate(Task task)
            {
                return (task is FieldTask && (task as FieldTask).Field == m_field);
            });

            this.CloseClicked += new Action<TycoonWindow>(delegate
            {
                //m_field.NameChanged -= new Action(Field_NameChanged);                
                fieldCalandarPanel.Delete();
                fieldStatsPanel.Delete();
                this.Visible = false;
                Program.Graphics.RemoveWindow(this);
            });

            //set view to be the initly visible panel
            ShowView();

            //change visible panel when tab buttons are clicked
            viewTabButton.Clicked += new Action<TycoonControl>(delegate(TycoonControl control)
            {
                ShowView();
            });
            statusTabButton.Clicked += new Action<TycoonControl>(delegate
            {
                ShowStats();
            });
            scheduleTabButton.Clicked += new Action<TycoonControl>(delegate
            {
                ShowScheudle();
            });

            //set reasonable initial size
            this.Width = 400;
            this.Height = 400;

            //add window to graphics
            Program.Graphics.AddWindow(this); 
        }

        public void ShowView()
        {
            fieldViewPanel.Visible = true;
            fieldStatsPanel.Visible = false;
            fieldCalandarPanel.Visible = false;
        }
        public void ShowStats()
        {
            fieldViewPanel.Visible = false;
            fieldStatsPanel.Visible = true;
            fieldCalandarPanel.Visible = false;
        }
        public void ShowScheudle()
        {
            fieldViewPanel.Visible = false;
            fieldStatsPanel.Visible = false;
            fieldCalandarPanel.Visible = true;
        }

    }
}
