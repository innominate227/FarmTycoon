using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace TycoonGraphicsLib
{
    public class TycoonWorldViewPanel : TycoonPanel
    {
        #region GUI Properties

        /// <summary>
        /// X location to view in the panel
        /// </summary>
        private volatile float _viewX;

        /// <summary>
        /// Y location to view in the panel
        /// </summary>
        private volatile float _viewY;

        /// <summary>
        /// Z location to view in the panel
        /// </summary>
        private volatile float _viewZ;


        /// <summary>
        /// X location to view in the panel
        /// </summary>
        public float ViewX
        {
            get { return _viewX; }
            set
            {
                _viewX = value;
                if (_worldView != null)
                {
                    _worldView.X = _viewX;
                }
            }
        }

        /// <summary>
        /// Y location to view in the panel
        /// </summary>
        public float ViewY
        {
            get { return _viewY; }
            set
            {
                _viewY = value;
                if (_worldView != null)
                {
                    _worldView.Y = _viewY;
                }
            }
        }

        /// <summary>
        /// Z location to view in the panel
        /// </summary>
        public float ViewZ
        {
            get { return _viewZ; }
            set
            {
                _viewZ = value;
                if (_worldView != null)
                {
                    _worldView.Z = _viewZ;
                }
            }
        }
        #endregion

        #region Rendering World View

        /// <summary>
        /// World view to draw when the panel is rendered
        /// </summary>
        private WorldView _worldView;
       
        /// <summary>
        /// Preform the special world view rendering step
        /// </summary>
        internal override void DoSpecialPanelRender()
        {
            //create the world view it is the first time rendering
            if (_worldView == null)
            {
                _worldView = new WorldView(_parentWindow.World);
                _worldView.X = _viewX;
                _worldView.Y = _viewY;
                _worldView.Z = _viewZ;
                _worldView.Overdraw = 2;
            }

            int topAbsolute, leftAbsolute;
            base.GetPositionAbsolute(out leftAbsolute, out topAbsolute);

            _worldView.RenderLocX = leftAbsolute + 1;
            _worldView.RenderLocY = topAbsolute + 1;
            _worldView.RenderWidth = Width - 2;
            _worldView.RenderHeight = Height - 2;
            
            _worldView.RenderView();
        }

        #endregion
    }
}
