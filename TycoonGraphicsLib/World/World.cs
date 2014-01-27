using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace TycoonGraphicsLib
{
    internal class World
    {		
		/// <summary>
		/// manages the tiles that make up the world 
		/// </summary>
		private TileManager _tileManger;
			
		/// <summary>
		/// manages the textrues applied to the tiles that make up the world 
		/// </summary>
		private TileTextureManager _tileTextureManager;
		
        /// <summary>
        /// The world settings that determined how many TileBuffers were created
        /// </summary>
        private WorldSettings _worldSettings;
                
        /// <summary>
        /// Some setting of the world view need to be shared across all views of the world.  This a reference to those settings.
        /// </summary>
        private SharedWorldViewSettings _sharedWorldViewSettings;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="worldSettings"></param>
        public World(WorldSettings worldSettings)
        {
            _worldSettings = worldSettings;
            _sharedWorldViewSettings = new SharedWorldViewSettings(this);
            _tileTextureManager = new TileTextureManager(this);
			_tileManger = new TileManager(this);
        }
        
        /// <summary>
        /// Some setting of the world view need to be shared across all views of the world.  This a reference to those settings.
        /// </summary>
        public SharedWorldViewSettings SharedWorldViewSettings
        {
            get { return _sharedWorldViewSettings; }
        }
                
		/// <summary>
        /// The world settings that determined how many TileBuffers were created
        /// </summary>
        public WorldSettings WorldSettings
        {
            get { return _worldSettings; }
        }
		
		/// <summary>
		/// manages the tiles that make up the world 
		/// </summary>
        public TileManager TileManger 
		{
    		get { return _tileManger; }
    	}
    	    	
    	/// <summary>
    	/// Manages the textures, and texture quartets for the tiles
    	/// </summary>
		public TileTextureManager TileTextureManager 
		{
			get { return _tileTextureManager; }
    	}

        /// <summary>
        /// Free all resources used by the world
        /// </summary>
        public void Delete()
        {
            _tileManger.Delete();
            _tileTextureManager.Delete();
        }


    }
}
