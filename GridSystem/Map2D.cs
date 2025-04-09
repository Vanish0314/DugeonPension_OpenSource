using System;
using System.Numerics;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Dungeon.GridSystem
{

    public class Map2D<T>
    {
        private T[,] m_MapData;
        private int m_Width;
        private int m_Height;

        public Map2D(int width, int height)
        {
            m_Width = width;
            m_Height = height;
            m_MapData = new T[width, height];
        }

        public int Width => m_Width;
        public int Height => m_Height;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="MapX">X pos in map coord</param>
        /// <param name="MapY">Y pos in map coord</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public T Get(int MapX, int MapY)
        {
            if (!IsValidCoordinate(MapX, MapY))
                throw new ArgumentOutOfRangeException("[Map_2D.cs] Invalid coordinates.");
            return m_MapData[MapX, MapY];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="MapX">X pos in map coord</param>
        /// <param name="MapY">Y pos in map coord</param>
        /// <param name="value"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void Set(int MapX, int MapY, T value)
        {
            if (!IsValidCoordinate(MapX, MapY))
                throw new ArgumentOutOfRangeException("[Map_2D.cs] Invalid coordinates.");
            m_MapData[MapX, MapY] = value;
        }

        public void Set(Vector2Int pos, T value)
        {
            Set(pos.x, pos.y, value);
        }

        public bool IsValidCoordinate(int x, int y)
        {
            return x >= 0 && x < m_Width && y >= 0 && y < m_Height;
        }

        public void Clear()
        {
            Array.Clear(m_MapData, 0, m_MapData.Length);
        }

        public void ForEachRun(Action<int, int, T> action)
        {
            for (int x = 0; x < m_Width; x++)
            {
                for (int y = 0; y < m_Height; y++)
                {
                    action(x, y, m_MapData[x, y]);
                }
            }
        }

        public void FillAll(T value)
        {
            for (int x = 0; x < m_Width; x++)
            {
                for (int y = 0; y < m_Height; y++)
                {
                    m_MapData[x, y] = value;
                }
            }
        }

        public T[,] GetRawMapData()
        {
            return (T[,])m_MapData.Clone();
        }

        public void Resize(int newWidth, int newHeight)
        {
            T[,] newMapData = new T[newWidth, newHeight];
            int minWidth = Math.Min(m_Width, newWidth);
            int minHeight = Math.Min(m_Height, newHeight);

            for (int x = 0; x < minWidth; x++)
            {
                for (int y = 0; y < minHeight; y++)
                {
                    newMapData[x, y] = m_MapData[x, y];
                }
            }

            m_MapData = newMapData;
            m_Width = newWidth;
            m_Height = newHeight;
        }
    }

    public class Map2D_IJob<T> where T : struct
    {
        private NativeArray<T> m_MapData;
        private int m_Width;
        private int m_Height;

        public Map2D_IJob(int width, int height)
        {
            m_Width = width;
            m_Height = height;
            m_MapData = new NativeArray<T>(width * height, Allocator.Persistent);
        }

        public int Width => m_Width;
        public int Height => m_Height;

        public T Get(int x, int y)
        {
            if (!IsValidCoordinate(x, y))
                throw new ArgumentOutOfRangeException("[Map2D_IJob.cs] Invalid coordinates.");
            return m_MapData[x + y * m_Width];
        }

        public void Set(int x, int y, T value)
        {
            if (!IsValidCoordinate(x, y))
                throw new ArgumentOutOfRangeException("[Map2D_IJob.cs] Invalid coordinates.");
            m_MapData[x + y * m_Width] = value;
        }

        public bool IsValidCoordinate(int x, int y)
        {
            return x >= 0 && x < m_Width && y >= 0 && y < m_Height;
        }

        public void Clear()
        {
            NativeArray<T> newMapData = new(m_Width * m_Height, Allocator.Persistent);
            m_MapData.Dispose();
            m_MapData = newMapData;
        }

        public void ForEachRunParallel(Action<int, int, T> action)
        {
            NativeArray<int> xIndices = new(m_Width * m_Height, Allocator.TempJob);
            NativeArray<int> yIndices = new(m_Width * m_Height, Allocator.TempJob);
            NativeArray<T> values = m_MapData;

            for (int x = 0; x < m_Width; x++)
            {
                for (int y = 0; y < m_Height; y++)
                {
                    int index = x + y * m_Width;
                    xIndices[index] = x;
                    yIndices[index] = y;
                }
            }

            JobHandle jobHandle = new ForEachJob<T>
            {
                xIndices = xIndices,
                yIndices = yIndices,
                mapData = values,
                width = m_Width,
                action = action
            }.Schedule(m_Width * m_Height, 64);

            jobHandle.Complete();

            xIndices.Dispose();
            yIndices.Dispose();
        }

        public void Resize(int newWidth, int newHeight)
        {
            NativeArray<T> newMapData = new(newWidth * newHeight, Allocator.Persistent);
            int minWidth = Math.Min(m_Width, newWidth);
            int minHeight = Math.Min(m_Height, newHeight);

            for (int x = 0; x < minWidth; x++)
            {
                for (int y = 0; y < minHeight; y++)
                {
                    newMapData[x + y * newWidth] = m_MapData[x + y * m_Width];
                }
            }

            m_MapData.Dispose();
            m_MapData = newMapData;
            m_Width = newWidth;
            m_Height = newHeight;
        }

        private struct ForEachJob<Type> : IJobParallelFor where Type : struct
        {
            public NativeArray<int> xIndices;
            public NativeArray<int> yIndices;
            public NativeArray<Type> mapData;
            public int width;
            public Action<int, int, Type> action;

            public void Execute(int index)
            {
                int x = xIndices[index];
                int y = yIndices[index];
                Type value = mapData[x + y * width];
                action(x, y, value);
            }
        }

        public void Dispose()
        {
            if (m_MapData.IsCreated)
            {
                m_MapData.Dispose();
            }
        }
    }


}