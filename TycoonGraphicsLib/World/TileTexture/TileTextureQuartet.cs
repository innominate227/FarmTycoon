
using System;

namespace TycoonGraphicsLib
{

	/// <summary>
	/// Set of four textures one each for north south east and west
	/// </summary>
	internal class TileTextureQuartet
	{
		private string _name;		
		private TileTexture _north;
		private TileTexture _east;
		private TileTexture _south;
		private TileTexture _west;

        public TileTextureQuartet(string name, TileTexture north, TileTexture east, TileTexture south, TileTexture west)
		{
            _name = name;
			
			_north = north;
			_east = east;
			_south = south;
			_west = west;			
		}
		
		
		
		public string Name 
		{
			get { return _name; }
		}
		
		
		public TileTexture North 
		{
			get { return _north; }
		}
			
		
		public TileTexture East 
		{
			get { return _east; }
		}
			
		
		public TileTexture South 
		{
			get { return _south; }
		}
		
		
		public TileTexture West 
		{
			get { return _west; }
		}


        public TileTexture GetTextureForDirection(ViewDirection dir)
        {
            if (dir == ViewDirection.North)
            {
                return _north;
            }
            else if (dir == ViewDirection.East)
            {
                return _east;
            }
            else if (dir == ViewDirection.South)
            {
                return _south;
            }
            else //if (dir == ViewDirection.West)
            {
                return _west;
            }
		}
		
	}
}
