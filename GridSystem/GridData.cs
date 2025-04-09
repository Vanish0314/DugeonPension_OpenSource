using System;
using System.Collections;
using System.Collections.Generic;
using Dungeon.GridSystem;
using UnityEngine;

namespace Dungeon
{
    public class GridData
    {
        public GridProperties properties;
        public List<TileLayerData> layers = new();

        [Serializable]
        public class TileLayerData
        {
            public string layerName;
            public List<TileCellData> tiles = new(); // Json friendly
        }

        [Serializable]
        public class TileCellData
        {
            public int x, y;
            public string tilePath;
            public TilePathBlockType blockTypeType;
            public TileFunctionType funtionType;

            public IDungeonTile GetTile()
            {
                throw new NotImplementedException();//TODO: Load tile from path
            }
        }
    }
}
