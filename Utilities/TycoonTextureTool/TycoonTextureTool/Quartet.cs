using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TycoonTextureTool
{
    public class Quartet
    {

        /// <summary>
        /// Catagory string
        /// </summary>
        public string Catagory
        {
            get;
            set;
        }

        /// <summary>
        /// Name string
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Texture for viewing qurtet from the north
        /// </summary>
        public Texture North
        {
            get;
            set;
        }

        /// <summary>
        /// Texture for viewing qurtet from the south
        /// </summary>
        public Texture South
        {
            get;
            set;
        }

        /// <summary>
        /// Texture for viewing qurtet from the east
        /// </summary>
        public Texture East
        {
            get;
            set;
        }

        /// <summary>
        /// Texture for viewing qurtet from the nwest
        /// </summary>
        public Texture West
        {
            get;
            set;
        }



        public Quartet CloneKeepTextures()
        {
            Quartet clone = new Quartet();
            
            //clone the quartet
            clone.Catagory = this.Catagory;
            clone.North = this.North;
            clone.South = this.South;
            clone.East = this.East;
            clone.West = this.West;

            //find a unique name for the copy
            int copyNum = 1;
            string name = this.Name + " Copy" + copyNum.ToString();
            while (TextureTool.Instance.Quartets.ContainsKey(name))
            {
                name = this.Name + " Copy" + copyNum.ToString();
                copyNum++;
            }

            clone.Name = name;

            //give the clone to the main data strcuture
            TextureTool.Instance.AddQuartet(clone);

            return clone;
        }


        public Quartet Clone()
        {
            Quartet clone = new Quartet();

            //clone all the texture the quartet uses
            Dictionary<Texture, Texture> clonedTextures = new Dictionary<Texture, Texture>();
            foreach (Texture texture in new Texture[] { this.North, this.South, this.East, this.West })
            {
                if (clonedTextures.ContainsKey(texture) == false)
                {
                    clonedTextures.Add(texture, texture.Clone());
                }
            }

            //clone the quartet
            clone.Catagory = this.Catagory;
            clone.North = clonedTextures[this.North];
            clone.South = clonedTextures[this.South];
            clone.East = clonedTextures[this.East];
            clone.West = clonedTextures[this.West];
            
            //find a unique name for the copy
            int copyNum = 1;
            string name = this.Name + " Copy" + copyNum.ToString();
            while (TextureTool.Instance.Quartets.ContainsKey(name))
            {
                name = this.Name + " Copy" + copyNum.ToString();
                copyNum++;
            }

            clone.Name = name;

            //give the clone to the main data strcuture
            TextureTool.Instance.AddQuartet(clone);

            return clone;
        }

    }
}
