using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TycoonTextureTool
{
    public class WindowManager
    {
        private static WindowManager _instance = new WindowManager();
        public static WindowManager Instance
        {
            get { return _instance; }
        }





        private Dictionary<Texture, TextureEditWindow> _textureWindows = new Dictionary<Texture, TextureEditWindow>();
        
        /// <summary>
        /// All open texture windows
        /// </summary>
        public List<TextureEditWindow> TextureWindows
        {
            get { return new List<TextureEditWindow>(_textureWindows.Values); }
        }

        public void ShowTextureWindow(Texture texture)
        {
            if (_textureWindows.ContainsKey(texture) == false)
            {            
                _textureWindows[texture] = new TextureEditWindow(texture);
                _textureWindows[texture].FormClosed += new System.Windows.Forms.FormClosedEventHandler(WindowManager_FormClosed);
            }
            _textureWindows[texture].Show();
            _textureWindows[texture].BringToFront();
        }
        





        private Dictionary<Quartet, QuartetEditWindow> _quartetWindows = new Dictionary<Quartet, QuartetEditWindow>();

        /// <summary>
        /// All open quartet windows
        /// </summary>
        public List<QuartetEditWindow> QuartetWindows
        {
            get { return new List<QuartetEditWindow>(_quartetWindows.Values); }
        }

        public void ShowQuartetWindow(Quartet quartet)
        {
            if (_quartetWindows.ContainsKey(quartet) == false)
            {
                _quartetWindows[quartet] = new QuartetEditWindow(quartet);
                _quartetWindows[quartet].FormClosed += new System.Windows.Forms.FormClosedEventHandler(WindowManager_FormClosed);
            }
            _quartetWindows[quartet].Show();
            _quartetWindows[quartet].BringToFront();
        }





        public void WindowManager_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            if (sender is TextureEditWindow)
            {
                _textureWindows.Remove((sender as TextureEditWindow).Texture);
            }
            else if (sender is QuartetEditWindow)
            {
                _quartetWindows.Remove((sender as QuartetEditWindow).Quartet);
            }
        }



        public void CloseAllWindows()
        {
            foreach (TextureEditWindow textEditWindow in _textureWindows.Values.ToArray())
            {
                textEditWindow.Close();
            }
            foreach (QuartetEditWindow quartetWindow in _quartetWindows.Values.ToArray())
            {
                quartetWindow.Close();
            }
        }

    }
}
