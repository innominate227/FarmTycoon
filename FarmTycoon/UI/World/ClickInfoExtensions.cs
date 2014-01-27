using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;

namespace FarmTycoon
{
    public static class ClickInfoExtensions
    {
        public static GameObject GetGameObjectClicked(this ClickInfo clickInfo)
        {
            if (clickInfo.TopMostTile == null)
            {
                return null;
            }
            else
            {
                return (clickInfo.TopMostTile.Tag as GameTile).GameObject;
            }
        }


        public static Land GetLandClicked(this ClickInfo clickInfo)
        {
            TileClickInfo landClickedInfo = clickInfo.GetLandClickedInfo();
            if (landClickedInfo != null)
            {
                return (landClickedInfo.Tile.Tag as GameTile).GameObject as Land;
            }
            return null;
        }


        public static TileClickInfo GetLandClickedInfo(this ClickInfo clickInfo)
        {
            foreach (TileClickInfo tileClicked in clickInfo.TilesClicked)
            {
                if ((tileClicked.Tile.Tag as GameTile).GameObject is Land)
                {
                    return tileClicked;
                }
            }
            return null;
        }

    }
}
