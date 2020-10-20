using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block
{
    enum Cubeside { BOTTOM, TOP, LEFT, RIGHT, FRONT, BACK };
    public enum BlockType { GRASS, DIRT, STONE, SAND, AIR };
    enum UV { UV00, UV10, UV01, UV11 }

    enum BlockUVType { GRASS_TOP, GRASS_SIDE, DIRT, STONE, SAND };


    BlockType blockType;
    public bool isSolid;

    Chunk owner;
    GameObject parent;
    Vector3 position;

    int x;
    int y;
    int z;

    static readonly IReadOnlyDictionary<BlockUVType, IReadOnlyDictionary<UV, Vector2>> blockUVs = new Dictionary<BlockUVType, IReadOnlyDictionary<UV, Vector2>>
    {
        {
            BlockUVType.GRASS_TOP,
            new Dictionary<UV, Vector2>()
            {
                { UV.UV00, new Vector2( 0.125f,  0.375f) },
                { UV.UV10, new Vector2(0.1875f,  0.375f) },
                { UV.UV01, new Vector2( 0.125f, 0.4375f) },
                { UV.UV11, new Vector2(0.1875f, 0.4375f) }
            }
        },
        {
            BlockUVType.GRASS_SIDE,
            new Dictionary<UV, Vector2>()
            {
                { UV.UV00, new Vector2(0.1875f, 0.9375f) },
                { UV.UV10, new Vector2(  0.25f, 0.9375f) },
                { UV.UV01, new Vector2(0.1875f,    1.0f) },
                { UV.UV11, new Vector2(  0.25f,    1.0f) }
            }
        },
        {
            BlockUVType.DIRT,
            new Dictionary<UV, Vector2>()
            {
                { UV.UV00, new Vector2(  0.125f, 0.9375f) },
                { UV.UV10, new Vector2( 0.1875f, 0.9375f) },
                { UV.UV01, new Vector2(  0.125f,    1.0f) },
                { UV.UV11, new Vector2( 0.1875f,    1.0f) }
            }
        },
        {
            BlockUVType.STONE,
            new Dictionary<UV, Vector2>()
            {
                { UV.UV00, new Vector2(      0f,  0.875f) },
                { UV.UV10, new Vector2( 0.0625f,  0.875f) },
                { UV.UV01, new Vector2(      0f, 0.9375f) },
                { UV.UV11, new Vector2( 0.0625f, 0.9375f) }
            }
        },
        {
            BlockUVType.SAND,
            new Dictionary<UV, Vector2>()
            {
                { UV.UV00, new Vector2(     0f,   0.25f) },
                { UV.UV10, new Vector2(0.0625f,   0.25f) },
                { UV.UV01, new Vector2(0.0625f, 0.3125f) },
                { UV.UV11, new Vector2(     0f, 0.3125f) }
            }
        },
    };

    static readonly IReadOnlyDictionary<BlockType, IReadOnlyDictionary<Cubeside, BlockUVType>> blockUVType = new Dictionary<BlockType, IReadOnlyDictionary<Cubeside, BlockUVType>>()
    {
        {
            BlockType.GRASS,
            new Dictionary<Cubeside, BlockUVType>()
            {
                { Cubeside.BOTTOM, BlockUVType.DIRT },
                { Cubeside.TOP,    BlockUVType.GRASS_TOP },
                { Cubeside.LEFT,   BlockUVType.GRASS_SIDE },
                { Cubeside.RIGHT,  BlockUVType.GRASS_SIDE },
                { Cubeside.FRONT,  BlockUVType.GRASS_SIDE },
                { Cubeside.BACK,   BlockUVType.GRASS_SIDE },
            }
        },
        { 
            BlockType.DIRT,
            new Dictionary<Cubeside, BlockUVType>()
            {
                { Cubeside.BOTTOM, BlockUVType.DIRT },
                { Cubeside.TOP,    BlockUVType.DIRT },
                { Cubeside.LEFT,   BlockUVType.DIRT },
                { Cubeside.RIGHT,  BlockUVType.DIRT },
                { Cubeside.FRONT,  BlockUVType.DIRT },
                { Cubeside.BACK,   BlockUVType.DIRT },
            }
        },
        { 
            BlockType.STONE,
            new Dictionary<Cubeside, BlockUVType>()
            {
                { Cubeside.BOTTOM, BlockUVType.STONE },
                { Cubeside.TOP,    BlockUVType.STONE },
                { Cubeside.LEFT,   BlockUVType.STONE },
                { Cubeside.RIGHT,  BlockUVType.STONE },
                { Cubeside.FRONT,  BlockUVType.STONE },
                { Cubeside.BACK,   BlockUVType.STONE },
            }
        },
        { 
            BlockType.SAND,
            new Dictionary<Cubeside, BlockUVType>()
            {
                { Cubeside.BOTTOM, BlockUVType.SAND },
                { Cubeside.TOP,    BlockUVType.SAND },
                { Cubeside.LEFT,   BlockUVType.SAND },
                { Cubeside.RIGHT,  BlockUVType.SAND },
                { Cubeside.FRONT,  BlockUVType.SAND },
                { Cubeside.BACK,   BlockUVType.SAND },
            }
        },
    };


    public Block(BlockType bType, Vector3 blockPos, GameObject blockParent, Chunk blockOwner)
    {
        blockType = bType;
        position = blockPos;
        parent = blockParent;
        owner = blockOwner;

        if (blockType == BlockType.AIR)
            isSolid = false;
        else
            isSolid = true;

        x = (int)blockPos.x;
        y = (int)blockPos.y;
        z = (int)blockPos.z;
    }

    void CreateQuad(Cubeside side, List<Vector3> v, List<Vector3> n, List<Vector2> u, List<int> t)
    {
        /*
        Mesh mesh = new Mesh();
        mesh.name = "ScriptedMesh" + side.ToString();
		*/

        //Vector3[] vertices = new Vector3[4];
        //Vector3[] normals = new Vector3[4];
        //Vector2[] uvs = new Vector2[4];
        //int[] triangles = new int[6];

        //all possible UVs
        BlockUVType uvType = blockUVType[blockType][side];
        Vector2 uv00 = blockUVs[uvType][UV.UV00];
        Vector2 uv10 = blockUVs[uvType][UV.UV10];
        Vector2 uv01 = blockUVs[uvType][UV.UV01];
        Vector2 uv11 = blockUVs[uvType][UV.UV11];

        //all possible vertices 
        Vector3 p0 = World.allVertices[x, y, z + 1];
        Vector3 p1 = World.allVertices[x + 1, y, z + 1];
        Vector3 p2 = World.allVertices[x + 1, y, z];
        Vector3 p3 = World.allVertices[x, y, z];
        Vector3 p4 = World.allVertices[x, y + 1, z + 1];
        Vector3 p5 = World.allVertices[x + 1, y + 1, z + 1];
        Vector3 p6 = World.allVertices[x + 1, y + 1, z];
        Vector3 p7 = World.allVertices[x, y + 1, z];

        int trioffset = 0;

        switch (side)
        {
            case Cubeside.BOTTOM:
                /*vertices = new Vector3[] {p0, p1, p2, p3};
				normals = new Vector3[] {Vector3.down, Vector3.down, 
											Vector3.down, Vector3.down};
				uvs = new Vector2[] {uv11, uv01, uv00, uv10};
				triangles = new int[] { 3, 1, 0, 3, 2, 1};*/
                trioffset = v.Count;
                v.Add(p0); v.Add(p1); v.Add(p2); v.Add(p3);
                n.Add(World.allNormals[World.NDIR.DOWN]);
                n.Add(World.allNormals[World.NDIR.DOWN]);
                n.Add(World.allNormals[World.NDIR.DOWN]);
                n.Add(World.allNormals[World.NDIR.DOWN]);
                u.Add(uv11); u.Add(uv01); u.Add(uv00); u.Add(uv10);
                t.Add(3 + trioffset); t.Add(1 + trioffset); t.Add(0 + trioffset); t.Add(3 + trioffset); t.Add(2 + trioffset); t.Add(1 + trioffset);

                break;
            case Cubeside.TOP:
                /*vertices = new Vector3[] {p7, p6, p5, p4};
				normals = new Vector3[] {Vector3.up, Vector3.up, 
											Vector3.up, Vector3.up};
				uvs = new Vector2[] {uv11, uv01, uv00, uv10};
				triangles = new int[] {3, 1, 0, 3, 2, 1};*/
                trioffset = v.Count;
                v.Add(p7); v.Add(p6); v.Add(p5); v.Add(p4);
                n.Add(World.allNormals[(int)World.NDIR.UP]);
                n.Add(World.allNormals[(int)World.NDIR.UP]);
                n.Add(World.allNormals[(int)World.NDIR.UP]);
                n.Add(World.allNormals[(int)World.NDIR.UP]);
                u.Add(uv11); u.Add(uv01); u.Add(uv00); u.Add(uv10);
                t.Add(3 + trioffset); t.Add(1 + trioffset); t.Add(0 + trioffset); t.Add(3 + trioffset); t.Add(2 + trioffset); t.Add(1 + trioffset);

                break;
            case Cubeside.LEFT:
                /*vertices = new Vector3[] {p7, p4, p0, p3};
				normals = new Vector3[] {Vector3.left, Vector3.left, 
											Vector3.left, Vector3.left};
				uvs = new Vector2[] {uv11, uv01, uv00, uv10};
				triangles = new int[] {3, 1, 0, 3, 2, 1};*/
                trioffset = v.Count;
                v.Add(p7); v.Add(p4); v.Add(p0); v.Add(p3);
                n.Add(World.allNormals[World.NDIR.LEFT]);
                n.Add(World.allNormals[World.NDIR.LEFT]);
                n.Add(World.allNormals[World.NDIR.LEFT]);
                n.Add(World.allNormals[World.NDIR.LEFT]);
                u.Add(uv11); u.Add(uv01); u.Add(uv00); u.Add(uv10);
                t.Add(3 + trioffset); t.Add(1 + trioffset); t.Add(0 + trioffset); t.Add(3 + trioffset); t.Add(2 + trioffset); t.Add(1 + trioffset);

                break;
            case Cubeside.RIGHT:
                /*vertices = new Vector3[] {p5, p6, p2, p1};
				normals = new Vector3[] {Vector3.right, Vector3.right, 
											Vector3.right, Vector3.right};
				uvs = new Vector2[] {uv11, uv01, uv00, uv10};
				triangles = new int[] {3, 1, 0, 3, 2, 1};*/
                trioffset = v.Count;
                v.Add(p5); v.Add(p6); v.Add(p2); v.Add(p1);
                n.Add(World.allNormals[World.NDIR.RIGHT]);
                n.Add(World.allNormals[World.NDIR.RIGHT]);
                n.Add(World.allNormals[World.NDIR.RIGHT]);
                n.Add(World.allNormals[World.NDIR.RIGHT]);
                u.Add(uv11); u.Add(uv01); u.Add(uv00); u.Add(uv10);
                t.Add(3 + trioffset); t.Add(1 + trioffset); t.Add(0 + trioffset); t.Add(3 + trioffset); t.Add(2 + trioffset); t.Add(1 + trioffset);

                break;
            case Cubeside.FRONT:
                /*vertices = new Vector3[] {p4, p5, p1, p0};
				normals = new Vector3[] {Vector3.forward, Vector3.forward, 
											Vector3.forward, Vector3.forward};
				uvs = new Vector2[] {uv11, uv01, uv00, uv10};
				triangles = new int[] {3, 1, 0, 3, 2, 1};*/
                trioffset = v.Count;
                v.Add(p4); v.Add(p5); v.Add(p1); v.Add(p0);
                n.Add(World.allNormals[World.NDIR.FRONT]);
                n.Add(World.allNormals[World.NDIR.FRONT]);
                n.Add(World.allNormals[World.NDIR.FRONT]);
                n.Add(World.allNormals[World.NDIR.FRONT]);
                u.Add(uv11); u.Add(uv01); u.Add(uv00); u.Add(uv10);
                t.Add(3 + trioffset); t.Add(1 + trioffset); t.Add(0 + trioffset); t.Add(3 + trioffset); t.Add(2 + trioffset); t.Add(1 + trioffset);

                break;
            case Cubeside.BACK:
                /*vertices = new Vector3[] {p6, p7, p3, p2};
				normals = new Vector3[] {Vector3.back, Vector3.back, 
											Vector3.back, Vector3.back};
				uvs = new Vector2[] {uv11, uv01, uv00, uv10};
				triangles = new int[] {3, 1, 0, 3, 2, 1};*/
                trioffset = v.Count;
                v.Add(p6); v.Add(p7); v.Add(p3); v.Add(p2);
                n.Add(World.allNormals[World.NDIR.BACK]);
                n.Add(World.allNormals[World.NDIR.BACK]);
                n.Add(World.allNormals[World.NDIR.BACK]);
                n.Add(World.allNormals[World.NDIR.BACK]);
                u.Add(uv11); u.Add(uv01); u.Add(uv00); u.Add(uv10);
                t.Add(3 + trioffset); t.Add(1 + trioffset); t.Add(0 + trioffset); t.Add(3 + trioffset); t.Add(2 + trioffset); t.Add(1 + trioffset);

                break;
        }

        /*mesh.vertices = vertices;
		mesh.normals = normals;
		mesh.uv = uvs;
		mesh.triangles = triangles;
		 
		mesh.RecalculateBounds();
		
		GameObject quad = new GameObject("Quad");
		quad.transform.position = position;
	    quad.transform.parent = parent.transform;

     	MeshFilter meshFilter = (MeshFilter) quad.AddComponent(typeof(MeshFilter));
		meshFilter.mesh = mesh;*/

        //MeshRenderer renderer = quad.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        //renderer.material = cubeMaterial;
    }

    int ConvertBlockIndexToLocal(int i)
    {
        if (i == -1)
            i = World.chunkSize - 1;
        else if (i == World.chunkSize)
            i = 0;
        return i;
    }

    int ConvertYBlockIndexToLocal(int i)
    {
        if (i == -1)
            i = World.chunkHeight - 1;
        else if (i == World.chunkHeight)
            i = 0;
        return i;
    }

    public bool HasSolidNeighbor(int x, int y, int z)
    {
        Block[,,] chunkInfo;

        if (x < 0 || x >= World.chunkSize ||
           y < 0 || y >= World.chunkHeight ||
           z < 0 || z >= World.chunkSize)
        {  //block in a neighbouring chunk

            Vector3 neighbourChunkPos = this.parent.transform.position +
                                        new Vector3
                                        (
                                            (x - (int)position.x) * World.chunkSize,
                                            (y - (int)position.y) * World.chunkHeight,
                                            (z - (int)position.z) * World.chunkSize
                                        );
            string neighborName = World.BuildChunkName(neighbourChunkPos);

            x = ConvertBlockIndexToLocal(x);
            y = ConvertYBlockIndexToLocal(y);
            z = ConvertBlockIndexToLocal(z);

            Chunk neighborChunk;
            if (World.chunks.TryGetValue(neighborName, out neighborChunk))
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
