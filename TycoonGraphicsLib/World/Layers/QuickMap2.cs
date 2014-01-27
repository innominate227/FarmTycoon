using System;
using System.Drawing;
using System.Collections.Generic;
using System.Diagnostics;

namespace TycoonGraphicsLib
{

    public class QuickMap
    {

        private List<MobileTile>[][] _mapPositions;
        private int _maxY;
        private int _maxX;

        public QuickMap(int maxX, int maxY)
        {
            _maxX = maxX;
            _maxY = maxY;
            _mapPositions = new List<MobileTile>[maxX + 1][];
            for (int x = 0; x <= maxX; x++)
            {
                _mapPositions[x] = new List<MobileTile>[maxY + 1];
                for (int y = 0; y <= maxY; y++)
                {
                    _mapPositions[x][y] = new List<MobileTile>();
                }
            }

        }

        
        public List<MobileTile> InsertAndQuery(MobileTile insertedTile)
        {
            int minX = (int)Math.Floor(insertedTile.WorldLeft);
            int maxX = (int)Math.Ceiling(insertedTile.WorldRight);
            int minY = (int)Math.Floor(insertedTile.WorldTop);
            int maxY = (int)Math.Ceiling(insertedTile.WorldBottom);

            Debug.Assert(minX <= maxX);
            Debug.Assert(minY <= maxY);

            List<MobileTile> queryResults = new List<MobileTile>();


            float myXmin = insertedTile.WorldLeft;
            float myXmax = insertedTile.WorldRight;
            float myYmin = insertedTile.WorldTop;
            float myYmax = insertedTile.WorldBottom;

            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    foreach (MobileTile otherTile in _mapPositions[x][y])
                    {
                        if (otherTile.SearchBool)
                        {
                            continue;
                        }

                        bool overlap = (myXmin < otherTile.WorldRight && myXmax > otherTile.WorldLeft && myYmin < otherTile.WorldBottom && myYmax > otherTile.WorldTop);
                        
                        if (overlap)                        
                        {
                            queryResults.Add(otherTile);
                            otherTile.SearchBool = true;
                        }
                    }

                    _mapPositions[x][y].Add(insertedTile);
                    insertedTile.QuickMapPositions.Add(new Point(x, y));
                }
            }

            foreach (MobileTile queryResultTile in queryResults)
            {
                queryResultTile.SearchBool = false;
            }


            return queryResults;
        }


        public List<MobileTile> MoveAndQuery(MobileTile item)
        {
            Remove(item);
            return InsertAndQuery(item);
        }

        public void Remove(MobileTile item)
        {
            foreach (Point quadMapPoint in item.QuickMapPositions)
            {
                _mapPositions[quadMapPoint.X][quadMapPoint.Y].Remove(item);
            }
            item.QuickMapPositions.Clear();
        }


    }

}
