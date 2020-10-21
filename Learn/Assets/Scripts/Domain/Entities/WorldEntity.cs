using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Domain.Entities
{
    public sealed class WorldEntity : MonoBehaviour
    {
        #region Variables
        [SerializeField]
        private int seed = 1337;

        [Space]
        public Material atlasMaterial;

        public static int chunkSize = 32;
        public static int chunkHeight = 64;

        private static int columnHeight = 2;
        private static int worldSize = 4;

        public static Vector3[,,] allVertices = new Vector3[chunkSize + 1, chunkHeight + 1, chunkSize + 1];

        public static Dictionary<string, ChunkEntity> chunks = new Dictionary<string, ChunkEntity>();

        public static IReadOnlyDictionary<NDIR, Vector3> allNormals = new Dictionary<NDIR, Vector3>()
        {
            { NDIR.UP, Vector3.up },
            { NDIR.DOWN, Vector3.down },
            { NDIR.LEFT, Vector3.left },
            { NDIR.RIGHT, Vector3.right },
            { NDIR.FRONT, Vector3.forward },
            { NDIR.BACK, Vector3.back },
        };
        #endregion

        #region Custom Methods
        public static string BuildChunkName(Vector3 pos)
        {
            return
                (int)pos.x + "_" +
                (int)pos.y + "_" +
                (int)pos.z;
        }

        private void BuildNewChunkAt(Vector3 chunkPos)
        {
            var c = new ChunkEntity(chunkSize, chunkHeight, chunkPos, gameObject, atlasMaterial, seed);
            chunks.Add(c.chunk.name, c);
            c.DrawChunk(chunkSize, chunkHeight);
        }

        IEnumerator BuildChunksColumn()
        {
            for (int i = 0; i < columnHeight; i++)
            {
                Vector3 chunkPos = new Vector3
                    (transform.position.x, i * chunkHeight, transform.position.z);

                var c = new ChunkEntity(chunkSize, chunkHeight, chunkPos, gameObject, atlasMaterial, seed);
                chunks.Add(c.chunk.name, c);
            }

            // the foreach could be avoided by just drawing
            // each chunk as you made them. But for the
            // purpose of being able to see the inter chunk
            // optimization we draw them after they all exist.
            foreach (KeyValuePair<string, ChunkEntity> c in chunks)
            {
                c.Value.DrawChunk(chunkSize, chunkHeight);
                yield return null;
            }
        }

        IEnumerator BuildWorld()
        {
            for (int x = 0; x < worldSize; x++)
                for (int y = 0; y < columnHeight; y++)
                    for (int z = 0; z < worldSize; z++)
                    {
                        Vector3 chunkPos = new Vector3(x * chunkSize, y * chunkHeight, z * chunkSize);
                        var c = new ChunkEntity(chunkSize, chunkHeight, chunkPos, gameObject, atlasMaterial, seed);
                        chunks.Add(c.chunk.name, c);
                    }

            // the foreach could be avoided by just drawing
            // each chunk as you made them. But for the
            // purpose of being able to see the inter chunk
            // optimization we draw them after they all exist.
            foreach (KeyValuePair<string, ChunkEntity> c in chunks)
            {
                c.Value.DrawChunk(chunkSize, chunkHeight);
                yield return null;
            }

        }

        private void SetUp()
        {
            // make sure the World is centered
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;

            // if the chunk size is set lower by user then
            // optimize for smaller mesh data
            if (chunkSize * chunkHeight * chunkSize < 16384)
            {
                Debug.Log("Lower IndexFormat for chunk meshes");
            }
            // else set max mesh data higher
            else
            {
                Debug.Log("Higher IndexFormat for chunk meshes");
            }

            //generate all vertices
            for (int x = 0; x <= chunkSize; x++)
                for (int y = 0; y <= chunkHeight; y++)
                    for (int z = 0; z <= chunkSize; z++)
                    {
                        allVertices[x, y, z] = new Vector3(x, y, z);
                    }
        }

        public int GetSeed()
        {
            return seed;
        }
        #endregion

        #region Builtin Methods
        // Use this for initialization
        void Start()
        {
            SetUp();

            //build chunk here
            //GameObject c = Instantiate(chunkPrefab, this.transform.position, this.transform.rotation);
            //c.GetComponent<Chunk>().CreateChunk(cSize, cHeight, cSize);

            //BuildChunkAt(transform.position);

            //StartCoroutine(BuildChunksColumn());

            StartCoroutine(BuildWorld());
        }

        // Update is called once per frame
        void Update()
        {

        }
        #endregion
    }
}
