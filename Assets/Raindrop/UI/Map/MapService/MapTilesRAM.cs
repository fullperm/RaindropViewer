﻿using OpenMetaverse;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Raindrop.Map.Model
{
    /// <summary>
    /// API to retrieve, delete, modify, create MapTiles in the scene
    /// </summary>
    public class MapTilesRAM
    {
        //number of tiles that are in the scene.
        public int visibleCount => sceneTiles.Count;
        int poolSize => pool.Count;
        
        //tiles that are in the scene.
        private Dictionary<ulong, MapTile> sceneTiles = new Dictionary<ulong, MapTile>();

        private MapTilesPool pool;

        private ConcurrentQueue<MapService.MapData> readyForDecode_DataQueue;
        public MapTilesRAM(int poolSize, ConcurrentQueue<MapService.MapData> receivedDataQueue)
        {
            pool = new MapTilesPool(poolSize);
        }

        /// <summary>
        /// push a tile into visible scene, at a specific handle location.
        /// </summary>
        /// <param name="handle">location in grid coordinates * 256</param>
        /// <param name="tile">the map data</param>
        public void setTile(ulong handle, MapTile tile)
        {
            sceneTiles.Add(handle, tile);
        }

        /// <summary>
        /// gets a maptile, if it is present
        /// </summary>
        /// <param name="handle">the region handle of the tile to get ; gridCoords * 256 and pack X&Y together.</param>
        /// <returns> Tile </returns>
        public bool tryGetTile_RAM(ulong handle, out MapTile tile)
        {
            // MapTile tile = null;
            bool success = sceneTiles.TryGetValue(handle, out tile);
            return success;
        }
        
        /// <summary>
        /// creates a blank maptile at a grid handle.
        /// </summary>
        /// <param name="handle">the region handle of the tile to get ; gridCoords * 256 and pack X&Y together.</param>
        /// <returns> Tile </returns>
        public MapTile setEmptyTile(ulong handle)
        {
            MapTile tile = pool.acquireTile();
            sceneTiles.Add(handle, tile);
            return tile;
        }

        /// <summary>
        /// Returns a Region to the pool. -- EG not visible anymore
        /// </summary>
        /// <param name="handle"> the region handle </param>
        public void deprecateTile(ulong handle)
        {
            MapTile maptile;
            sceneTiles.TryGetValue(handle, out maptile);
            if (maptile == null)
            {
                throw new Exception("Tile not found when trying to deprecate tile.");
            } else
            {
                sceneTiles.Remove(handle);
                pool.releaseTile(maptile);
            }

        }



    }
}