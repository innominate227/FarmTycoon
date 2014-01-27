using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL;

namespace TycoonGraphicsLib
{
    /// <summary>
    /// Contains information about an Open GL buffer of quads
    /// </summary>
    internal class QuadTextureBuffer : QuadBuffer
    {        
        /// <summary>
		/// Create a new quad buffer
		/// </summary>
        public QuadTextureBuffer() : this(1)
        {
        }
		    	
		/// <summary>
		/// Create a new quad buffer spcifing the intiaial buffer size
		/// </summary>
        public QuadTextureBuffer(int bufferSize)
        {
            _slotSize = 16;
            base.CreateBuffer(bufferSize);
        }


        /// <summary>
        /// The part of the buffer rendering that is unique to this class
        /// </summary>
        protected override void  RenderBufferAbstract()
        {
            GL.VertexPointer(2, VertexPointerType.Float, (int)(4 * sizeof(float)), (IntPtr)0);
            GL.TexCoordPointer(2, TexCoordPointerType.Float, (int)(4 * sizeof(float)), (IntPtr)(2 * sizeof(float)));            
                        

            GL.DrawArrays(BeginMode.Quads, 0, _nextIndex * 4);
        }


        /// <summary>
        /// set values of a slot
        /// </summary>
        public void SetSlotValues(int slot, float left, float top, float right, float bottom, float texLeft, float texTop, float texRight, float texBottom)
        {
            //add slot to the modified list
            _modifiedSlots.Add(slot);

            //set the values
            _buffer[slot * 16 + 0] = left;
            _buffer[slot * 16 + 1] = top;

            _buffer[slot * 16 + 2] = texLeft;
            _buffer[slot * 16 + 3] = texTop;

            _buffer[slot * 16 + 4] = right;
            _buffer[slot * 16 + 5] = top;

            _buffer[slot * 16 + 6] = texRight;
            _buffer[slot * 16 + 7] = texTop;

            _buffer[slot * 16 + 8] = right;
            _buffer[slot * 16 + 9] = bottom;

            _buffer[slot * 16 + 10] = texRight;
            _buffer[slot * 16 + 11] = texBottom;

            _buffer[slot * 16 + 12] = left;
            _buffer[slot * 16 + 13] = bottom;

            _buffer[slot * 16 + 14] = texLeft;
            _buffer[slot * 16 + 15] = texBottom;
        }



        /// <summary>
        /// set the texture values of a slot
        /// </summary>
        public void SetSlotTextureValues(int slot, float texLeft, float texTop, float texRight, float texBottom)
        {
            //add slot to the modified list
            _modifiedSlots.Add(slot);

            //set the values
            _buffer[slot * 16 + 2] = texLeft;
            _buffer[slot * 16 + 3] = texTop;
            
            _buffer[slot * 16 + 6] = texRight;
            _buffer[slot * 16 + 7] = texTop;
            
            _buffer[slot * 16 + 10] = texRight;
            _buffer[slot * 16 + 11] = texBottom;
            
            _buffer[slot * 16 + 14] = texLeft;
            _buffer[slot * 16 + 15] = texBottom;
        }

        /// <summary>
        /// set values of a slot
        /// </summary>
        public void SetSlotValues(int slot, float left, float top, float right, float bottom, Texture texture)
        {
            SetSlotValues(slot, left, top, right, bottom, texture.Left, texture.Top, texture.Right, texture.Bottom);
        }

        /// <summary>
        /// set the texture values of a slot
        /// </summary>
        public void SetSlotTextureValues(int slot, Texture texture)
        {
            SetSlotTextureValues(slot, texture.Left, texture.Top, texture.Right, texture.Bottom);
        }
        	
		/// <summary>
		/// Get the value in a slot
		/// </summary>
		public void GetSlotValues(int slot, out float left, out float top, out float right, out float bottom, out float texLeft, out float texTop, out float texRight, out float texBottom)
        {
            //get the values
            left = _buffer[slot * 16 + 0];
            top = _buffer[slot * 16 + 1];

            texLeft = _buffer[slot * 16 + 2];
            texTop = _buffer[slot * 16 + 3];

            right = _buffer[slot * 16 + 4];
            top = _buffer[slot * 16 + 5];

            texRight = _buffer[slot * 16 + 6];
            texTop = _buffer[slot * 16 + 7];

            right = _buffer[slot * 16 + 8];
            bottom = _buffer[slot * 16 + 9];

            texRight = _buffer[slot * 16 + 10];
            texBottom = _buffer[slot * 16 + 11];

            left = _buffer[slot * 16 + 12];
            bottom = _buffer[slot * 16 + 13];

            texLeft = _buffer[slot * 16 + 14];
            texBottom = _buffer[slot * 16 + 15];
        }


    }
}
