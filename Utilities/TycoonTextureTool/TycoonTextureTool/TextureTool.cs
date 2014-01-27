using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TycoonTextureTool
{
    /// <summary>
    /// Top level domain class for the tool
    /// </summary>
    public class TextureTool
    {
        private static TextureTool _instance = new TextureTool();
        public static TextureTool Instance
        {
            get { return _instance; }
        }


        private Dictionary<string, Texture> _windowTextures = new Dictionary<string, Texture>();
        /// <summary>
        /// All Window Textures keyed on their name
        /// </summary>
        public Dictionary<string, Texture> WindowTextures
        {
            get { return _windowTextures; }
        }


        private Dictionary<string, Texture> _textures = new Dictionary<string, Texture>();
        /// <summary>
        /// All Textures keyed on their name
        /// </summary>
        public Dictionary<string, Texture> Textures
        {
            get { return _textures; }
        }


        private Dictionary<string, Quartet> _quartets = new Dictionary<string, Quartet>();
        /// <summary>
        /// All Quartets keyed on their name
        /// </summary>
        public Dictionary<string, Quartet> Quartets
        {
            get { return _quartets; }
        }
        

        private Dictionary<string, HashSet<Texture>> _textureCatagories = new Dictionary<string, HashSet<Texture>>();
        /// <summary>
        /// All Textures by catagory
        /// </summary>
        public Dictionary<string, HashSet<Texture>> TextureCatagories
        {
            get { return _textureCatagories; }
        }


        private Dictionary<string, HashSet<Quartet>> _quartetCatagories = new Dictionary<string, HashSet<Quartet>>();
        /// <summary>
        /// All Quartets by catagory
        /// </summary>
        public Dictionary<string, HashSet<Quartet>> QuartetCatagories
        {
            get { return _quartetCatagories; }
        }




        /// <summary>
        /// Remove the Quartet passed from the Quartet and Catagories data structures                
        /// </summary>
        public void RemoveQuartet(Quartet quartet)
        {
            _quartets.Remove(quartet.Name);
            _quartetCatagories[quartet.Catagory].Remove(quartet);
            if (_quartetCatagories[quartet.Catagory].Count == 0)
            {
                _quartetCatagories.Remove(quartet.Catagory);
            }
        }


        /// <summary>
        /// Place the Quartet passed into the Quartet and Catagories data structures        
        /// Throws an expection if unable to because the Quartet name is already taken
        /// </summary>
        public void AddQuartet(Quartet quartet)
        {
            if (_quartets.ContainsKey(quartet.Name))
            {
                throw new Exception("Duplicate Name");
            }

            _quartets.Add(quartet.Name, quartet);
            if (_quartetCatagories.ContainsKey(quartet.Catagory) == false)
            {
                _quartetCatagories.Add(quartet.Catagory, new HashSet<Quartet>());
            }
            _quartetCatagories[quartet.Catagory].Add(quartet);
        }






        /// <summary>
        /// Remove the texture passed from the Textures and Catagories data structures                
        /// </summary>
        public void RemoveTexture(Texture texture)
        {
            _textures.Remove(texture.FullName);
            _textureCatagories[texture.Catagory].Remove(texture);
            if (_textureCatagories[texture.Catagory].Count == 0)
            {
                _textureCatagories.Remove(texture.Catagory);
            }            
        }


        /// <summary>
        /// Place the texture passed into the Textures and Catagories data structures        
        /// Throws an expection if unable to because the texture name is already taken
        /// </summary>
        public void AddTexture(Texture texture)
        {
            if (_textures.ContainsKey(texture.FullName))
            {
                throw new Exception("Duplicate Name");
            }

            _textures.Add(texture.FullName, texture);
            if (_textureCatagories.ContainsKey(texture.Catagory) == false)
            {
                _textureCatagories.Add(texture.Catagory, new HashSet<Texture>());
            }
            _textureCatagories[texture.Catagory].Add(texture);
            
        }



        public string WorkingDirectory
        {
            get;
            set;
        }

        public string TexturesDirectory
        {
            get { return WorkingDirectory + "Textures" + Path.DirectorySeparatorChar; }
        }

    }
}
