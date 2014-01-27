using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FarmTycoon
{
    public class Orchard : GameObject, IHasQuality, IEnclosedObject
    {
        /// <summary>
        /// The enclosure the orchard is in
        /// </summary>
        private Enclosure m_enclosure;
        
        /// <summary>
        /// Empty contrustor, use only when loading game
        /// </summary>
        public Orchard() : base() 
        {
        }
                
        /// <summary>
        /// Create a orchard
        /// </summary>
        public void Setup(Enclosure enclosure)
        {
            m_enclosure = enclosure;
            
            //update the locations we are on
            m_enclosure.LocationsChanged += new Action(UpdateLocationsOn);
            UpdateLocationsOn();

            //Orchard is immedantly considered done with placement
            this.DoneWithPlacement();    
        }
        
        private void UpdateLocationsOn()
        {
            //clear locations we used to be one
            ClearLocationsOn();

            //we are on the same locations as the enclosure
            AddLocationsOn(m_enclosure.AllLocationsOn);                        
        }
        
        public override void UpdateTiles()
        {
            //Orchard has no tiles.
        }

        /// <summary>
        /// The enclsure this orchard is in
        /// </summary>
        public Enclosure Enclosure
        {
            get { return m_enclosure; }
        }

        /// <summary>
        /// Called when the orchard is deleted
        /// </summary>
        protected override void DeleteInner()
        {
            m_enclosure.LocationsChanged -= new Action(UpdateLocationsOn);
            m_enclosure.Delete();
        }
        
        #region Trees

        /// <summary>
        /// Composite quality averaging the quality of all trees in the orchard.
        /// null if the orchard is empty.
        /// </summary>
        private CompositeQuality m_quality;

        /// <summary>
        /// Trees in the orchard
        /// </summary>
        private List<Tree> m_trees = new List<Tree>();

        /// <summary>
        /// Are the trees in the list ordered to corespond with the land in the enclosure
        /// </summary>
        private bool m_treesAreOrdered = false;

        /// <summary>
        /// All trees in the orchard
        /// </summary>
        public IList<Tree> Trees
        {
            get
            {
                if (m_treesAreOrdered == false) { OrderTrees(); } 
                return m_trees.AsReadOnly();
            }
        }

        /// <summary>
        /// Add a tree to the orchard
        /// </summary>
        public void AddTree(Tree tree)
        {
            if (m_quality == null)
            {
                m_quality = new CompositeQuality();                
            }
            m_quality.AddQuality((Quality)tree.Quality);
            m_trees.Add(tree);            
        }

        /// <summary>
        /// Remove a tree from the orchard
        /// </summary>
        public void RemoveTree(Tree tree)
        {
            m_quality.RemoveQuality((Quality)tree.Quality);
            m_trees.Remove(tree);
            if (m_trees.Count == 0)
            {
                m_quality = null;
            }
        }

        /// <summary>
        /// Order the trees to be in the same order as the land in the enclosure
        /// </summary>
        private void OrderTrees()
        {
            List<Tree> newTreeList = new List<Tree>();
            foreach (Land land in m_enclosure.OrderedLand)
            {
                Tree treeOnLand = land.LocationOn.Find<Tree>();
                Debug.Assert(m_trees.Contains(treeOnLand));
                newTreeList.Add(treeOnLand);
            }

            Debug.Assert(newTreeList.Count == m_trees.Count);
            m_trees = newTreeList;
            m_treesAreOrdered = true;
        }
        

        /// <summary>
        /// Return true or false depending on if the orchard has trees
        /// </summary>
        public bool HasTrees
        {
            get
            {
                return (m_trees.Count > 0);
            }
        }

        /// <summary>
        /// The name of trees planted in the orchard, or "None" if no trees are planted        
        /// </summary>
        public string TreesPlanted
        {
            get
            {
                if (m_trees.Count == 0) { return "None"; }
                return m_trees[0].TreeInfo.Name;
            }
        }

        /// <summary>
        /// Tree info for the trees planted in the field or null if no trees planted
        /// </summary>
        public TreeInfo TreeInfo
        {
            get
            {
                if (m_trees.Count == 0) { return null; }
                return m_trees[0].TreeInfo;
            }
        }

        /// <summary>
        /// Average Quality of all trees in the orchard, will be null if no trees are in the orchard
        /// </summary>
        public IQuality Quality
        {
            get { return m_quality; }
        }

        #endregion
                
        
        public override void WriteState(ObjectState state)
        {
            base.WriteState(state);

            state.SetValue("Enclosure", m_enclosure);
            state.SetListValues<Tree>("Trees", m_trees);
            state.SetValue("TreesAreOrdered", m_treesAreOrdered);       
            state.WriteSubState("Quality", m_quality);  
        }

        public override void ReadState(ObjectState state)
        {
            base.ReadState(state);

            m_enclosure = state.GetValue<Enclosure>("Enclosure");
            m_enclosure.LocationsChanged += new Action(UpdateLocationsOn);
            m_trees = state.GetListValues<Tree>("Trees");
            m_treesAreOrdered = state.GetValue<bool>("TreesAreOrdered");
            m_quality = state.ReadSubState<CompositeQuality>("Quality");  
        }

    }
}
