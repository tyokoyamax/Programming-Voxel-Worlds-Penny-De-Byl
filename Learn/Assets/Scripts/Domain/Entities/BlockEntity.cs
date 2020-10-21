using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Domain.Entities
{
    public sealed class BlockEntity
    {
        /// <summary>
        /// ブロック面の種類
        /// </summary>
        private enum Cubeside { BOTTOM, TOP, LEFT, RIGHT, FRONT, BACK };

        /// <summary>
        /// UV座標の種類
        /// </summary>
        private enum UV { X0Y0, X1Y0, X0Y1, X1Y1 }

        /// <summary>
        /// ブロックテクスチャの種類
        /// </summary>
        private enum BlockTexture { GRASS_TOP, GRASS_SIDE, DIRT, STONE, SAND };

        /// <summary>
        /// ブロックUV
        /// </summary>
        private static readonly IReadOnlyDictionary<BlockTexture, IReadOnlyDictionary<UV, Vector2>> blockUVs = new Dictionary<BlockTexture, IReadOnlyDictionary<UV, Vector2>>
        {
            {
                BlockTexture.GRASS_TOP,
                new Dictionary<UV, Vector2>()
                {
                    { UV.X0Y0, new Vector2( 0.125f,  0.375f) },
                    { UV.X1Y0, new Vector2(0.1875f,  0.375f) },
                    { UV.X0Y1, new Vector2( 0.125f, 0.4375f) },
                    { UV.X1Y1, new Vector2(0.1875f, 0.4375f) }
                }
            },
            {
                BlockTexture.GRASS_SIDE,
                new Dictionary<UV, Vector2>()
                {
                    { UV.X0Y0, new Vector2(0.1875f, 0.9375f) },
                    { UV.X1Y0, new Vector2(  0.25f, 0.9375f) },
                    { UV.X0Y1, new Vector2(0.1875f,    1.0f) },
                    { UV.X1Y1, new Vector2(  0.25f,    1.0f) }
                }
            },
            {
                BlockTexture.DIRT,
                new Dictionary<UV, Vector2>()
                {
                    { UV.X0Y0, new Vector2(  0.125f, 0.9375f) },
                    { UV.X1Y0, new Vector2( 0.1875f, 0.9375f) },
                    { UV.X0Y1, new Vector2(  0.125f,    1.0f) },
                    { UV.X1Y1, new Vector2( 0.1875f,    1.0f) }
                }
            },
            {
                BlockTexture.STONE,
                new Dictionary<UV, Vector2>()
                {
                    { UV.X0Y0, new Vector2(      0f,  0.875f) },
                    { UV.X1Y0, new Vector2( 0.0625f,  0.875f) },
                    { UV.X0Y1, new Vector2(      0f, 0.9375f) },
                    { UV.X1Y1, new Vector2( 0.0625f, 0.9375f) }
                }
            },
            {
                BlockTexture.SAND,
                new Dictionary<UV, Vector2>()
                {
                    { UV.X0Y0, new Vector2(     0f,   0.25f) },
                    { UV.X1Y0, new Vector2(0.0625f,   0.25f) },
                    { UV.X0Y1, new Vector2(0.0625f, 0.3125f) },
                    { UV.X1Y1, new Vector2(     0f, 0.3125f) }
                }
            },
        };

        /// <summary>
        /// ブロックテクスチャ
        /// </summary>
        private static readonly IReadOnlyDictionary<BlockType, IReadOnlyDictionary<Cubeside, BlockTexture>> blockTextures = new Dictionary<BlockType, IReadOnlyDictionary<Cubeside, BlockTexture>>()
        {
            {
                BlockType.GRASS,
                new Dictionary<Cubeside, BlockTexture>()
                {
                    { Cubeside.BOTTOM, BlockTexture.DIRT },
                    { Cubeside.TOP,    BlockTexture.GRASS_TOP },
                    { Cubeside.LEFT,   BlockTexture.GRASS_SIDE },
                    { Cubeside.RIGHT,  BlockTexture.GRASS_SIDE },
                    { Cubeside.FRONT,  BlockTexture.GRASS_SIDE },
                    { Cubeside.BACK,   BlockTexture.GRASS_SIDE },
                }
            },
            {
                BlockType.DIRT,
                new Dictionary<Cubeside, BlockTexture>()
                {
                    { Cubeside.BOTTOM, BlockTexture.DIRT },
                    { Cubeside.TOP,    BlockTexture.DIRT },
                    { Cubeside.LEFT,   BlockTexture.DIRT },
                    { Cubeside.RIGHT,  BlockTexture.DIRT },
                    { Cubeside.FRONT,  BlockTexture.DIRT },
                    { Cubeside.BACK,   BlockTexture.DIRT },
                }
            },
            {
                BlockType.STONE,
                new Dictionary<Cubeside, BlockTexture>()
                {
                    { Cubeside.BOTTOM, BlockTexture.STONE },
                    { Cubeside.TOP,    BlockTexture.STONE },
                    { Cubeside.LEFT,   BlockTexture.STONE },
                    { Cubeside.RIGHT,  BlockTexture.STONE },
                    { Cubeside.FRONT,  BlockTexture.STONE },
                    { Cubeside.BACK,   BlockTexture.STONE },
                }
            },
            {
                BlockType.SAND,
                new Dictionary<Cubeside, BlockTexture>()
                {
                    { Cubeside.BOTTOM, BlockTexture.SAND },
                    { Cubeside.TOP,    BlockTexture.SAND },
                    { Cubeside.LEFT,   BlockTexture.SAND },
                    { Cubeside.RIGHT,  BlockTexture.SAND },
                    { Cubeside.FRONT,  BlockTexture.SAND },
                    { Cubeside.BACK,   BlockTexture.SAND },
                }
            },
        };

        /// <summary>
        /// ブロックタイプ
        /// </summary>
        public readonly BlockType blockType;

        /// <summary>
        /// ブロック座標
        /// </summary>
        public readonly Vector3 position;

        /// <summary>
        /// 
        /// </summary>
        public readonly GameObject parent;

        /// <summary>
        /// 
        /// </summary>
        public readonly ChunkEntity owner;

        /// <summary>
        /// 固体かを示す
        /// </summary>
        public readonly bool isSolid;

        /// <summary>
        /// ブロック座標値
        /// </summary>
        private readonly int x, y, z;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="bType">ブロックのタイプ</param>
        /// <param name="blockPos">ブロックの位置</param>
        /// <param name="blockParent">ブロックが含まれているチャンクのゲームオブジェクト</param>
        /// <param name="blockOwner">ブロックが含まれているチャンク</param>
        public BlockEntity(BlockType bType, Vector3 blockPos, GameObject blockParent, ChunkEntity blockOwner)
        {
            blockType = bType;
            position = blockPos;
            parent = blockParent;
            owner = blockOwner;

            isSolid = !(blockType == BlockType.AIR);

            x = (int)position.x;
            y = (int)position.y;
            z = (int)position.z;
        }


        void CreateQuad(Cubeside side, List<Vector3> v, List<Vector3> n, List<Vector2> u, List<int> t)
        {
            //all possible UVs
            BlockTexture blockTexture = blockTextures[blockType][side];
            Vector2 uv00 = blockUVs[blockTexture][UV.X0Y0];
            Vector2 uv10 = blockUVs[blockTexture][UV.X1Y0];
            Vector2 uv01 = blockUVs[blockTexture][UV.X0Y1];
            Vector2 uv11 = blockUVs[blockTexture][UV.X1Y1];

            //all possible vertices 
            Vector3 p0 = WorldEntity.allVertices[x, y, z + 1];
            Vector3 p1 = WorldEntity.allVertices[x + 1, y, z + 1];
            Vector3 p2 = WorldEntity.allVertices[x + 1, y, z];
            Vector3 p3 = WorldEntity.allVertices[x, y, z];
            Vector3 p4 = WorldEntity.allVertices[x, y + 1, z + 1];
            Vector3 p5 = WorldEntity.allVertices[x + 1, y + 1, z + 1];
            Vector3 p6 = WorldEntity.allVertices[x + 1, y + 1, z];
            Vector3 p7 = WorldEntity.allVertices[x, y + 1, z];

            int trioffset = v.Count;

            switch (side)
            {
                case Cubeside.BOTTOM:
                    v.Add(p0); v.Add(p1); v.Add(p2); v.Add(p3);
                    n.Add(WorldEntity.allNormals[NDIR.DOWN]);
                    n.Add(WorldEntity.allNormals[NDIR.DOWN]);
                    n.Add(WorldEntity.allNormals[NDIR.DOWN]);
                    n.Add(WorldEntity.allNormals[NDIR.DOWN]);
                    u.Add(uv11); u.Add(uv01); u.Add(uv00); u.Add(uv10);
                    t.Add(3 + trioffset); t.Add(1 + trioffset); t.Add(0 + trioffset); t.Add(3 + trioffset); t.Add(2 + trioffset); t.Add(1 + trioffset);

                    break;
                case Cubeside.TOP:
                    v.Add(p7); v.Add(p6); v.Add(p5); v.Add(p4);
                    n.Add(WorldEntity.allNormals[NDIR.UP]);
                    n.Add(WorldEntity.allNormals[NDIR.UP]);
                    n.Add(WorldEntity.allNormals[NDIR.UP]);
                    n.Add(WorldEntity.allNormals[NDIR.UP]);
                    u.Add(uv11); u.Add(uv01); u.Add(uv00); u.Add(uv10);
                    t.Add(3 + trioffset); t.Add(1 + trioffset); t.Add(0 + trioffset); t.Add(3 + trioffset); t.Add(2 + trioffset); t.Add(1 + trioffset);

                    break;
                case Cubeside.LEFT:
                    v.Add(p7); v.Add(p4); v.Add(p0); v.Add(p3);
                    n.Add(WorldEntity.allNormals[NDIR.LEFT]);
                    n.Add(WorldEntity.allNormals[NDIR.LEFT]);
                    n.Add(WorldEntity.allNormals[NDIR.LEFT]);
                    n.Add(WorldEntity.allNormals[NDIR.LEFT]);
                    u.Add(uv11); u.Add(uv01); u.Add(uv00); u.Add(uv10);
                    t.Add(3 + trioffset); t.Add(1 + trioffset); t.Add(0 + trioffset); t.Add(3 + trioffset); t.Add(2 + trioffset); t.Add(1 + trioffset);

                    break;
                case Cubeside.RIGHT:
                    v.Add(p5); v.Add(p6); v.Add(p2); v.Add(p1);
                    n.Add(WorldEntity.allNormals[NDIR.RIGHT]);
                    n.Add(WorldEntity.allNormals[NDIR.RIGHT]);
                    n.Add(WorldEntity.allNormals[NDIR.RIGHT]);
                    n.Add(WorldEntity.allNormals[NDIR.RIGHT]);
                    u.Add(uv11); u.Add(uv01); u.Add(uv00); u.Add(uv10);
                    t.Add(3 + trioffset); t.Add(1 + trioffset); t.Add(0 + trioffset); t.Add(3 + trioffset); t.Add(2 + trioffset); t.Add(1 + trioffset);

                    break;
                case Cubeside.FRONT:
                    v.Add(p4); v.Add(p5); v.Add(p1); v.Add(p0);
                    n.Add(WorldEntity.allNormals[NDIR.FRONT]);
                    n.Add(WorldEntity.allNormals[NDIR.FRONT]);
                    n.Add(WorldEntity.allNormals[NDIR.FRONT]);
                    n.Add(WorldEntity.allNormals[NDIR.FRONT]);
                    u.Add(uv11); u.Add(uv01); u.Add(uv00); u.Add(uv10);
                    t.Add(3 + trioffset); t.Add(1 + trioffset); t.Add(0 + trioffset); t.Add(3 + trioffset); t.Add(2 + trioffset); t.Add(1 + trioffset);

                    break;
                case Cubeside.BACK:
                    v.Add(p6); v.Add(p7); v.Add(p3); v.Add(p2);
                    n.Add(WorldEntity.allNormals[NDIR.BACK]);
                    n.Add(WorldEntity.allNormals[NDIR.BACK]);
                    n.Add(WorldEntity.allNormals[NDIR.BACK]);
                    n.Add(WorldEntity.allNormals[NDIR.BACK]);
                    u.Add(uv11); u.Add(uv01); u.Add(uv00); u.Add(uv10);
                    t.Add(3 + trioffset); t.Add(1 + trioffset); t.Add(0 + trioffset); t.Add(3 + trioffset); t.Add(2 + trioffset); t.Add(1 + trioffset);
                    break;
            }
        }

        int ConvertBlockIndexToLocal(int i)
        {
            if (i == -1)
                i = WorldEntity.chunkSize - 1;
            else if (i == WorldEntity.chunkSize)
                i = 0;
            return i;
        }

        int ConvertYBlockIndexToLocal(int i)
        {
            if (i == -1)
                i = WorldEntity.chunkHeight - 1;
            else if (i == WorldEntity.chunkHeight)
                i = 0;
            return i;
        }

        public bool HasSolidNeighbor(int x, int y, int z)
        {
            BlockEntity[,,] chunkInfo;

            if (x < 0 || x >= WorldEntity.chunkSize ||
               y < 0 || y >= WorldEntity.chunkHeight ||
               z < 0 || z >= WorldEntity.chunkSize)
            {  //block in a neighbouring chunk

                Vector3 neighbourChunkPos = this.parent.transform.position +
                                            new Vector3
                                            (
                                                (x - (int)position.x) * WorldEntity.chunkSize,
                                                (y - (int)position.y) * WorldEntity.chunkHeight,
                                                (z - (int)position.z) * WorldEntity.chunkSize
                                            );
                string neighborName = WorldEntity.BuildChunkName(neighbourChunkPos);

                x = ConvertBlockIndexToLocal(x);
                y = ConvertYBlockIndexToLocal(y);
                z = ConvertBlockIndexToLocal(z);

                ChunkEntity neighborChunk;
                if (WorldEntity.chunks.TryGetValue(neighborName, out neighborChunk))
                {
                    chunkInfo = neighborChunk.chunkData;
                }
                else
                    return false;
            }  //block in this chunk
            else
                chunkInfo = owner.chunkData;

            try
            {
                return chunkInfo[x, y, z].isSolid;
            }
            catch (System.IndexOutOfRangeException) { }

            return false;
        }

        public void Draw(List<Vector3> v, List<Vector3> n, List<Vector2> u, List<int> t)
        {
            if (blockType == BlockType.AIR) return;

            if (!HasSolidNeighbor((int)position.x, (int)position.y, (int)position.z + 1))
                CreateQuad(Cubeside.FRONT, v, n, u, t);
            if (!HasSolidNeighbor((int)position.x, (int)position.y, (int)position.z - 1))
                CreateQuad(Cubeside.BACK, v, n, u, t);
            if (!HasSolidNeighbor((int)position.x, (int)position.y + 1, (int)position.z))
                CreateQuad(Cubeside.TOP, v, n, u, t);
            if (!HasSolidNeighbor((int)position.x, (int)position.y - 1, (int)position.z))
                CreateQuad(Cubeside.BOTTOM, v, n, u, t);

            // the last two ifs must be inverted otherwise we get issues
            if (!HasSolidNeighbor((int)position.x - 1, (int)position.y, (int)position.z))
                CreateQuad(Cubeside.LEFT, v, n, u, t);
            if (!HasSolidNeighbor((int)position.x + 1, (int)position.y, (int)position.z))
                CreateQuad(Cubeside.RIGHT, v, n, u, t);
        }
    }
}
