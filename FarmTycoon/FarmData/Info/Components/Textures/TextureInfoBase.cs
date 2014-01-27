using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace FarmTycoon
{
    /// <summary>
    /// Info on a Texture that is shown temporaily during an action
    /// </summary>
    public abstract class TextureInfoBase
    {
        
        /// <summary>
        /// Name of the texture to apply
        /// </summary>
        protected string _texture = "";

        /// <summary>
        /// The number of frames in the texture
        /// </summary>
        protected int _frames = 1;

        /// <summary>
        /// The frame rate for the texture in days 
        /// </summary>
        protected double _frameRate = 1;

        


        /// <summary>
        /// Name of the texture to apply
        /// </summary>
        public string Texture
        {
            get { return _texture; }
        }


        /// <summary>
        /// The number of frames in the texture
        /// </summary>
        public int Frames
        {
            get { return _frames; }
        }

        /// <summary>
        /// The frame rate for the texture in days 
        /// </summary>
        public double FrameRate
        {
            get { return _frameRate; }
        }

    }
}
