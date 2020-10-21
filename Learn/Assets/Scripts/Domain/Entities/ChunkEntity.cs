using Assets.Scripts.Domain.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Domain.Entities
{
    public sealed class ChunkEntity
    {
        public Material bMaterial;
        public BlockEntity[,,] chunkData;
        public GameObject chunk;
        public GameObject parent;

        List<Vector3> Verts = new List<Vector3>();
        List<Vector3> Norms = new List<Vector3>();
        List<Vector2> UVs = new List<Vector2>();
        List<int> Tris = new List<int>();

        public void CalculateChunkData(int chunkSize, int chunkHeight, int seed)
        {
            chunkData = new BlockEntity[chunkSize, chunkHeight, chunkSize];

            //create blocks
            for (int x = 0; x < chunkSize; x++)
                for (int y = 0; y < chunkHeight; y++)
                    for (int z = 0; z < chunkSize; z++)
                    {
                        Vector3 pos = new Vector3(x, y, z);

                        int worldX = x + (int)chunk.transform.position.x;
                        int worldY = y + (int)chunk.transform.position.y;
                        int worldZ = z + (int)chunk.transform.position.z;

                        int baseSurfaceHeight = NoiseHelper.GenerateHeight(worldX, worldZ, seed);
                        int stoneSurfaceHeight = NoiseHelper.GenerateStoneHeight(worldX, worldZ, seed);
                        float caves = NoiseHelper.fBM3D(worldX, worldY, worldZ, 0.1f, 3, seed);

                        // generate surface terrain
                        if (caves < 0.42f)
                            chunkData[x, y, z] = new BlockEntity(BlockType.AIR, pos, chunk, this);
                        else if (worldY <= stoneSurfaceHeight)
                        {
                            float diamondLump = NoiseHelper.fBM3D(worldX, worldY, worldZ, 0.01f, 2, seed);
                            if (diamondLump < 0.4f && worldY < 40)
                                chunkData[x, y, z] = new BlockEntity(BlockType.DIAMOND, pos, chunk, this);
                            else
                                chunkData[x, y, z] = new BlockEntity(BlockType.STONE, pos, chunk, this);
                        }
                        else if (worldY == baseSurfaceHeight)
                            chunkData[x, y, z] = new BlockEntity(BlockType.GRASS, pos, chunk, this);
                        else if (worldY < baseSurfaceHeight)
                            chunkData[x, y, z] = new BlockEntity(BlockType.DIRT, pos, chunk, this);
                        else
                            chunkData[x, y, z] = new BlockEntity(BlockType.AIR, pos, chunk, this);
                    }
        }

        public void DrawChunk(int chunkSize, int chunkHeight)
        {
            //draw blocks
            Verts.Clear();
            Norms.Clear();
            UVs.Clear();
            Tris.Clear();
            for (int x = 0; x < chunkSize; x++)
                for (int y = 0; y < chunkHeight; y++)
                    for (int z = 0; z < chunkSize; z++)
                    {
                        chunkData[x, y, z].Draw(Verts, Norms, UVs, Tris);
                    }

            Mesh mesh = new Mesh();
            mesh.name = "ScriptedMesh";

            // if the chunk size is set higher by user then
            // set the mesh index higher
            if (chunkSize * chunkHeight * chunkSize > 16384)
            {
                mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            }

            mesh.vertices = Verts.ToArray();
            mesh.normals = Norms.ToArray();
            mesh.uv = UVs.ToArray();
            mesh.triangles = Tris.ToArray();

            mesh.RecalculateBounds();

            MeshFilter meshFilter = chunk.gameObject.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;

            MeshRenderer renderer = chunk.gameObject.AddComponent<MeshRenderer>();
            renderer.material = bMaterial;

            chunk.gameObject.AddComponent<MeshCollider>();
        }

        /// <summary>
        /// The ChunkSomething look up listen to penny called this
        /// </summary>
        /// <param name="sizeX"></param>
        /// <param name="sizeY"></param>
        /// <param name="sizeZ"></param>
        /// <param name="chunkPos"></param>
        /// <param name="chunkParent">The parent of the chunk GameObject</param>
        /// <param name="atlasMat">The Material WIth the Texure Atlas</param>
        public ChunkEntity(int chunkSize, int chunkHeight, Vector3 chunkPos, GameObject chunkParent, Material atlasMat, int seed)
        {
            chunk = new GameObject(WorldEntity.BuildChunkName(chunkPos));
            chunk.transform.parent = chunkParent.transform;
            chunk.transform.position = chunkPos;
            bMaterial = atlasMat;
            CalculateChunkData(chunkSize, chunkHeight, seed);
        }

        /// <summary>
        /// Calculate the chunks data, then draw it
        /// </summary>
        /// <param name="sizeX"></param>
        /// <param name="sizeY"></param>
        /// <param name="sizeZ"></param>
        public void BuildChunk(int chunkSize, int chunkHeight, int seed)
        {
            CalculateChunkData(chunkSize, chunkHeight, seed);

            // remove later for optimization
            DrawChunk(chunkSize, chunkHeight);
        }

        /*
          Inter Chunk Optimization => Buffer Mode
         */
        // TODO make method for checking for chunk neighbors
        // if all chunk neigbors are calculated then draw this chunk

        /*
          Inter Chunk Optimization => Redraw Mode
         */
        // IF there is no adjacent chunks then continue
        // and optimize the sides with adjacent chunks.
        // Once all adjacent chunks have been loaded then
        // redraw this chunk with optimizations

        /*
          Regular Optimization
         */
        // Whenever the chunk is drawn optimize with
        // currently loaded adjacent chunks

        // make conditional setting set by designer ^

        bool ChunkExists(Vector3 pos)
        {
            string neighborName = WorldEntity.BuildChunkName(pos);

            if (WorldEntity.chunks.ContainsKey(neighborName))
                return true;
            else
                return false;
        }

    }
}
