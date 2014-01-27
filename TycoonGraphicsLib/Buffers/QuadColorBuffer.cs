using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace TycoonGraphicsLib
{
    /// <summary>
    /// Contains information about an Open GL buffer of quads
    /// </summary>
    internal class QuadColorBuffer : QuadBuffer
    {   
        /// <summary>
		/// Create a new quad buffer
		/// </summary>
        public QuadColorBuffer() : this(1)
        {
        }
		    	
		/// <summary>
		/// Create a new quad buffer  
		/// </summary>
        public QuadColorBuffer(int bufferSize)
        {
            _slotSize = 20;
            base.CreateBuffer(bufferSize);
        }
		
        /// <summary>
        /// The part of the buffer rendering that is unique to this class
        /// </summary>
        protected override void RenderBufferAbstract()
        {
            GL.VertexPointer(2, VertexPointerType.Float, (int)(5 * sizeof(float)), (IntPtr)0);
            GL.ColorPointer(3, ColorPointerType.Float, (int)(5 * sizeof(float)), (IntPtr)(2 * sizeof(float)));
            GL.DrawArrays(BeginMode.Quads, 0, _nextIndex * 4);
        }

        
        /// <summary>
        /// set values of a slot
        /// </summary>
        public void SetSlotValues(int slot, float left, float top, float right, float bottom, Color color)
        {
            if (color == Color.Transparent)
            {
                left = 0;
                top = 0;
                right = 0;
                bottom = 0;
            }


            //add slot to the modified list
            _modifiedSlots.Add(slot);
            
            float r = color.R / 255.0f;
            float g = color.G / 255.0f;
            float b = color.B / 255.0f;
            
            //set the values
            _buffer[slot * 20 + 0] = left;
            _buffer[slot * 20 + 1] = top;

            _buffer[slot * 20 + 2] = r;
            _buffer[slot * 20 + 3] = g;
            _buffer[slot * 20 + 4] = b;

            _buffer[slot * 20 + 5] = right;
            _buffer[slot * 20 + 6] = top;

            _buffer[slot * 20 + 7] = r;
            _buffer[slot * 20 + 8] = g;
            _buffer[slot * 20 + 9] = b;

            _buffer[slot * 20 + 10] = right;
            _buffer[slot * 20 + 11] = bottom;

            _buffer[slot * 20 + 12] = r;
            _buffer[slot * 20 + 13] = g;
            _buffer[slot * 20 + 14] = b;

            _buffer[slot * 20 + 15] = left;
            _buffer[slot * 20 + 16] = bottom;

            _buffer[slot * 20 + 17] = r;
            _buffer[slot * 20 + 18] = g;
            _buffer[slot * 20 + 19] = b;
        }
        

    }
}
