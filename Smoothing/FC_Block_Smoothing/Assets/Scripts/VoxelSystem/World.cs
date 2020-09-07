﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {

    #region Variables
    [Header("Options")]
    [SerializeField]
    private int seed = 1337;

    public Material atlasMaterial;

    [Header("Smoothing")]
    [SerializeField]
    private bool m_smooth = false;
    public static bool smoothing;

    [SerializeField] [Range(0f, 0.125f)]
    private float smoothAmount = 0.0625f;

    [SerializeField] [Range(-0.125f, 0f)]
    private float extrudeAmount = -0.0625f;

    public static int chunkSize = 16;
    public static int chunkHeight = 32;

    public int columnHeight = 1;
    public int worldSize = 1;

    public static Vector3[,,] allVertices = new Vector3[chunkSize + 1, chunkHeight + 1, chunkSize + 1];
	public static Vector3[] allNormals = new Vector3[6];
	public enum NDIR {UP, DOWN, LEFT, RIGHT, FRONT, BACK}

    public static Dictionary<string, Chunk> chunks = new Dictionary<string, Chunk>();
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
		Chunk c = new Chunk(chunkSize, chunkHeight, chunkPos, gameObject, atlasMaterial, seed);
		chunks.Add(c.chunk.name, c);
        c.DrawChunk(chunkSize, chunkHeight);
	}

    IEnumerator BuildChunksColumn()
    {
        for(int i = 0; i < columnHeight; i++)
        {
            Vector3 chunkPos = new Vector3
                (transform.position.x, i * chunkHeight, transform.position.z);

            Chunk c = new Chunk(chunkSize, chunkHeight, chunkPos, gameObject, atlasMaterial, seed);
            chunks.Add(c.chunk.name, c);
        }

        // the foreach could be avoided by just drawing
        // each chunk as you made them. But for the
        // purpose of being able to see the inter chunk
        // optimization we draw them after they all exist.
        foreach(KeyValuePair<string, Chunk> c in chunks)
        {
            c.Value.DrawChunk(chunkSize, chunkHeight);
            yield return null;
        }
    }

    IEnumerator BuildWorld()
    {
        for(int x = 0; x < worldSize; x++)
            for (int y = 0; y < columnHeight; y++)
                for (int z = 0; z < worldSize; z++)
                {
                    Vector3 chunkPos = new Vector3(x * chunkSize, y * chunkHeight, z * chunkSize);
                    Chunk c = new Chunk(chunkSize, chunkHeight, chunkPos, gameObject, atlasMaterial, seed);
                    chunks.Add(c.chunk.name, c);
                }

        if (smoothing)
            foreach (KeyValuePair<string, Chunk> c in chunks)
            {
                c.Value.SmoothChunk(chunkSize, chunkHeight, smoothAmount, extrudeAmount);
            }

        // the foreach could be avoided by just drawing
        // each chunk as you made them. But for the
        // purpose of being able to see the inter chunk
        // optimization we draw them after they all exist.
        foreach (KeyValuePair<string, Chunk> c in chunks)
        {
            c.Value.DrawChunk(chunkSize, chunkHeight);
            yield return null;
        }

    }

    private void SetUp()
    {
        // set up global variables
        smoothing = m_smooth;

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

        if(!smoothing)
            GenerateVertices();

        allNormals[(int)NDIR.UP] = Vector3.up;
        allNormals[(int)NDIR.DOWN] = Vector3.down;
        allNormals[(int)NDIR.LEFT] = Vector3.left;
        allNormals[(int)NDIR.RIGHT] = Vector3.right;
        allNormals[(int)NDIR.FRONT] = Vector3.forward;
        allNormals[(int)NDIR.BACK] = Vector3.back;
    }

    private static void GenerateVertices()
    {
        // generate all vertices
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
    void Start ()
    {
        SetUp();

        StartCoroutine(BuildWorld());
    }

    // Update is called once per frame
    void Update ()
	{

	}
    #endregion
}