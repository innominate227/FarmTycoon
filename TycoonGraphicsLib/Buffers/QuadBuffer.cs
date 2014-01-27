using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.Security;

namespace TycoonGraphicsLib
{
    /// <summary>
    /// Contains information about an Open GL buffer of quads
    /// </summary>
    internal abstract class QuadBuffer
    {

        /// <summary>
        /// Open GL id of the buffer.
        /// </summary>
        protected uint _bufferId;

        /// <summary>
        /// The a system memory copy of the data stored in the buffer.
        /// </summary>
        protected float[] _buffer;

        /// <summary>
        /// The buffer is pinned in memory so that Marshal.UnsafeAddrOfPinnedArrayElement can be used
        /// </summary>
        protected GCHandle _bufferGarbageHandle;

        /// <summary>
        /// Size of the buffer in slots.
        /// </summary>
        protected int _bufferSize;

        /// <summary>
        /// Next index in the buffer that is free to be used
        /// </summary>
        protected int _nextIndex = 0;

        /// <summary>
        /// Does the buffer need to be fully updated next render, this becomes true when the buffer is resized
        /// </summary>
        protected bool _fullFill = true;

        /// <summary>
        /// What slots of the buffer need to be updated next render
        /// </summary>
        protected List<int> _modifiedSlots = new List<int>();

        /// <summary>
        /// What slots in the buffer are free
        /// </summary>
        protected Queue<int> _freeSlots = new Queue<int>();

        /// <summary>
        /// How many "floats" (4 bytes) long is each slot, needs to be set by the inheriting class
        /// </summary>
        protected int _slotSize = -1;

        /// <summary>
        /// Count of the number of slots in the buffer that are used.  If 0 the buffer does not need to be rendered.
        /// </summary>
        protected int _slotsUsed = 0;

		/// <summary>
		/// Initial creation of the buffer
        /// </summary>
        [SecuritySafeCritical]
        protected void CreateBuffer(int bufferSize)
        {
            //generate an id for the buffer
            GL.GenBuffers(1, out _bufferId);
            
            //allocate space for the buffer
            _bufferSize = bufferSize;
            _buffer = new float[bufferSize * _slotSize];
            
            //pin the buffer so we can use Marshal.UnsafeAddrOfPinnedArrayElement is called on it
            _bufferGarbageHandle = GCHandle.Alloc(_buffer, GCHandleType.Pinned);            

            //need to do a full fill before the first time the buffer is used
            _fullFill = true;
        }

        /// <summary>
        /// Doubles the size of the buffer
        /// </summary>
        [SecuritySafeCritical]
        protected void IncreaseBufferSize()
        {            
            //double the size of the buffer
            _bufferSize *= 2;

            //resize the buffer array (Note _buffer will point to a newly allocated buffer after this call)
            Array.Resize<float>(ref _buffer, _bufferSize * _slotSize);
            _fullFill = true;
            
            //allow the old buffer to be garbage collected
            _bufferGarbageHandle.Free();
            
            //pin the new buffer so we can use Marshal.UnsafeAddrOfPinnedArrayElement is called on it
            _bufferGarbageHandle = GCHandle.Alloc(_buffer, GCHandleType.Pinned);  
            
            //need to do a full fill becasue the buffer size changed
            _fullFill = true;
        }
        		
        /// <summary>
        /// Returns the next free slot in the buffer.
        /// Its is expected that the slot will be taken (in otherwords it will no longer be free once it is returned).        
        /// </summary>
        public int GetNextFreeSlot()
        {
            //one more slot being used now
            _slotsUsed++;
                        
            if (_freeSlots.Count > 0)
            {
                //if theres a slot in the free slots queue return it
                return _freeSlots.Dequeue();
            }
            else
            {
                //if the buffer is full we need to make it bigger
                if (_nextIndex == _bufferSize)
                {
                    IncreaseBufferSize();
                }

                //return the index to use, and increment it.
                _nextIndex++;
                return (_nextIndex - 1);
            }
        }
        
        /// <summary>
        /// Draw the tiles in the buffer to the screen
        /// </summary>
        [SecuritySafeCritical]
        public void Render()
        {
            //buffer is empty so no need to draw anything
            if (_slotsUsed == 0)
            {
                return;
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, _bufferId);
            if (_fullFill)
            {
                _fullFill = false;
                GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(sizeof(float) * _buffer.Length), _buffer, BufferUsageHint.StreamDraw);
            }
            else
            {
                foreach (int modifiedSlot in _modifiedSlots)
                {
                    GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)(modifiedSlot * sizeof(float) * _slotSize), (IntPtr)(sizeof(float) * _slotSize), Marshal.UnsafeAddrOfPinnedArrayElement(_buffer, modifiedSlot * _slotSize));
                }
            }
            _modifiedSlots.Clear();

            //do the class unique part of the rendering
            RenderBufferAbstract();
        }

        /// <summary>
        /// The final step of the rendering if different for each type of buffer.
        /// </summary>
        protected abstract void RenderBufferAbstract();
                              
        /// <summary>
        /// free a slot in the buffer
        /// </summary>
        public void FreeSlot(int slot)
        {
            //add slot to the free list
            _freeSlots.Enqueue(slot);

            //add slot to the modified list
            _modifiedSlots.Add(slot);

            //zero out the slot
            for (int i = 0; i < _slotSize; i++)
            {
                _buffer[slot * _slotSize + i] = 0;
            }

            //one less slot being used now
            _slotsUsed--;
        }

        /// <summary>
        /// frees all slots in the buffer
        /// </summary>
        public void Clear()
        {
            _freeSlots.Clear();
            _nextIndex = 0;
            _slotsUsed = 0;
        }

        /// <summary>
        /// Is the buffer not empty (at least one slots is being used)
        /// </summary>
        public bool NotEmpty
        {
            get
            {
                return (_slotsUsed != 0);
            }
        }

        /// <summary>
        /// Deletes the buffer
        /// </summary>
        [SecuritySafeCritical]
        public void Delete()
        {
            //allow the system memory buffer copy to be garbage collected
            _bufferGarbageHandle.Free();

            //delete the openGL buffer
            GL.DeleteBuffers(1, ref _bufferId);
        }
    }
}
