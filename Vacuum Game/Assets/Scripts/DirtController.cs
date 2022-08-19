using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirtController : MonoBehaviour
{
    public static DirtController instance;
    public const float pixelSize = 0.2f;
    private const int batchSize = 256;
    public int numStartingParticles;
    public int maxParticles;
    [SerializeField] bool spawnParticlesOnAwake;
    [SerializeField] Vector2 startingParticleArea;
    [SerializeField] float mouseSuckSpeed;
    [SerializeField] float mouseSuckDistance;
    [SerializeField] float mouseDestroyDistance;

    private Transform[] floorboards;

    // Material to use for drawing the meshes.
    public Material[] materials;

    private List<Matrix4x4[]>[] matrices;
    private int[] offsets;
    public int particleCount { get; private set; }
    private int dynamicParticleIndex = 0;
    private const float dynamicZDepth = -0.1f;
    public bool hasSpawnedStartParticles = false;

    private Mesh mesh;

    private void Setup()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        List<Transform> l = new List<Transform>();
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject.tag == "Floorboard" && transform.GetChild(i).gameObject.activeSelf)
            {
                l.Add(transform.GetChild(i));
            }
        }
        floorboards = l.ToArray();



        mesh = CreateQuad();

        matrices = new List<Matrix4x4[]>[materials.Length];
        for (int c = 0; c < materials.Length; c++)
        {
            matrices[c] = new List<Matrix4x4[]>();
            matrices[c].Add(new Matrix4x4[batchSize]);
        }

        offsets = new int[matrices.Length];
        for (int i = 0; i < offsets.Length; i++)
		{
            offsets[i] = -1;
		}
    }

    private Mesh CreateQuad(float width = pixelSize, float height = pixelSize)
    {
        Mesh mesh = new Mesh();
        Vector3[] normals = new Vector3[4];
        int[] triangles = new int[6];
        Vector2[] UVs = new Vector2[4];
        Vector3[] vertices = new Vector3[4];

        vertices[0] = new Vector3(-width / 2, height / 2f, 0);
        vertices[1] = new Vector3(width / 2, height / 2, 0);
        vertices[2] = new Vector3(width / 2, -height / 2f, 0);
        vertices[3] = new Vector3(-width / 2, -height / 2f, 0);

        normals[0] = Vector3.up;
        normals[1] = Vector3.up;
        normals[2] = Vector3.up;
        normals[3] = Vector3.up;

        UVs[0] = new Vector2(0, 1);
        UVs[1] = new Vector2(1, 1);
        UVs[2] = new Vector2(1, 0);
        UVs[3] = new Vector2(0, 0);

        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;
        triangles[3] = 0;
        triangles[4] = 2;
        triangles[5] = 3;

        mesh.vertices = vertices;
        mesh.uv = UVs;
        mesh.triangles = triangles;
        return mesh;
    }

    private void Start()
    {
        Setup();
        if (spawnParticlesOnAwake && !hasSpawnedStartParticles)
		{
            AddStartingParticles();
        }
        
    }

    public void AddStartingParticles()
	{
        hasSpawnedStartParticles = true;
        // gets the weights of each floorboard based on total area
		float[] floorWeights = new float[floorboards.Length];
        float totalArea = 0;
        for (int i = 0; i < floorWeights.Length; i++)
		{
            floorWeights[i] = floorboards[i].lossyScale.x * floorboards[i].lossyScale.y;
            totalArea += floorWeights[i];
		}
        for (int i = 0; i < floorWeights.Length; i++)
		{
            floorWeights[i] /= totalArea;
		}

        for (int i = 0; i < numStartingParticles; i++)
        {
            int c = Random.Range(0, materials.Length - 1);
            Transform floorboard = floorboards[WeightedRandom(floorWeights)];
            float particleX = Random.Range(-floorboard.lossyScale.x / 2, floorboard.lossyScale.x / 2);
            float particleY = Random.Range(-floorboard.lossyScale.y / 2, floorboard.lossyScale.y / 2);
            AddParticle((Vector2)floorboard.position + new Vector2(particleX, particleY), c);
        }
    }

    private int WeightedRandom(float[] weights)
	{
        float r = Random.Range(0, 1f);
        float sum = 0;
        for (int i = 0; i < weights.Length; i++)
		{
            sum += weights[i];
            if (r < sum)
			{
                return i;
			}
		}
        return weights.Length - 1;
	}

    private void Draw()
    {
        // Draw a bunch of meshes each frame.
        for (int c = 0; c < matrices.Length; c++)
        { 
            for (int b = 0; b < matrices[c].Count; b++)
            {
                Matrix4x4[] m = matrices[c][b];

                int particlesInBatch = batchSize;
				if (b == matrices[c].Count - 1)
				{
                    particlesInBatch = offsets[c] % batchSize + 1;
				}

                Graphics.DrawMeshInstanced(mesh, 0, materials[c], m, particlesInBatch);
            }
        }

    }

    // adds a particle of a random material
    public bool AddParticle(Vector2 pos)
    {
        int c = Random.Range(0, matrices.Length);
        return AddParticle(pos, c);
    }

    // adds a particle of a specific material
    public bool AddParticle(Vector2 pos, int c)
    {
        if (offsets[c] >= maxParticles / materials.Length)
		{
            return false;
		}

        List<Matrix4x4[]> l = matrices[c];
        if ((offsets[c] + 1) > 0 && (offsets[c] + 1) % batchSize == 0)
        {
            l.Add(new Matrix4x4[batchSize]);
        }

        // Build matrix.
        Vector3 position = pos;
        Quaternion rotation = Quaternion.identity;
        Vector3 scale = Vector3.one;

        offsets[c]++;
        particleCount++;
        l[l.Count - 1][offsets[c] % batchSize] = Matrix4x4.TRS(position, rotation, scale);

        
        return true;
    }

    private void RemoveParticle(int c, int b, int i)
	{
        List<Matrix4x4[]> colorMatrix = matrices[c];
        
        int offsetInLastMatrix = offsets[c] % batchSize;

        int batchIndex = Mathf.FloorToInt(offsets[c] / batchSize);

        Matrix4x4 lastParticle = colorMatrix[batchIndex][offsetInLastMatrix];

        colorMatrix[b][i] = lastParticle;
        offsets[c]--;
        particleCount--;
        if (offsetInLastMatrix == 0 && colorMatrix.Count > 1)
		{
            colorMatrix.RemoveAt(colorMatrix.Count - 1);
            
		}
        if(particleCount == 0) LevelLoader.instance.UpdateRoomClean(true);
    }

    private void Update()
    {
        
        if (Input.GetKey(KeyCode.Mouse0))
        {
            // An example of how to use SuckSlice
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            SuckSlice(mousePos, Vector2.up * mouseSuckDistance, 360, mouseSuckSpeed, mouseDestroyDistance);
            //SweepLine(mousePos, (Vector2.up).normalized, mouseSuckDistance);

        }
        

        dynamicParticleIndex++;
        if (dynamicParticleIndex >= materials.Length)
		{
            dynamicParticleIndex = 0;
		}
        
        /*
        string s = "offsets: ";
        for (int i = 0; i < materials.Length; i++)
		{
            s += offsets[i] + ", ";
		}
        Debug.Log(s);

        s = "batches: ";
        for (int i = 0; i < materials.Length; i++)
        {
            s += matrices[i].Count + ", ";
        }
        Debug.Log(s);
        */

        Draw();
    }

    /// <summary>
    /// Sucks a bunch of dirt particles contained within a section of a circle towards the center
    /// </summary>
    /// <param name="center">The center of the circle</param>
    /// <param name="direction">The direction of the slice from the center of the circle. The magnitude determines the radius of the circle</param>
    /// <param name="degrees">The number of degrees the slice of the circle will cover</param>
    /// <param name="suckSpeed">Affects how fast the particals will move towards the center of the circle</param>
    /// <param name="destroyDistance">How close a particle has to be to the center of the circle to be destroyed</param>
    /// <returns>Number of particles destroyed by the suck</returns>
    public int SuckSlice(Vector2 center, Vector2 direction, float degrees, float suckSpeed, float destroyDistance)
	{
        float radius = direction.magnitude;

        Stack<Vector2> garbage = new Stack<Vector2>();


        Matrix4x4 matrix;
        float distance;
        Vector4 column;
        for (int b = 0; b < matrices[dynamicParticleIndex].Count; b++)
        {
            int particlesInBatch = batchSize;
            if (b == matrices[dynamicParticleIndex].Count - 1)
            {
                particlesInBatch = offsets[dynamicParticleIndex] % batchSize + 1;
            }

            for (int i = 0; i < particlesInBatch; i++)
            {
                // runs for each particle of a certain color, with the color alternating each frame
                matrix = matrices[dynamicParticleIndex][b][i];
                column = matrix.GetColumn(3);
                distance = Vector2.Distance(column, center);


                if (distance < radius && (Vector2.Angle((Vector2)column - center, direction) < degrees / 2 || column.z == dynamicZDepth))
                {
                    distance /= radius;
                    distance = Mathf.Pow(distance, 2);
                    Vector2 newPos = Vector2.MoveTowards(column, center, (1 - distance) * Time.deltaTime * suckSpeed);
                    column.x = newPos.x;
                    column.y = newPos.y;
                    column.z = dynamicZDepth;
                    if (Vector2.Distance(column, center) < destroyDistance)
                    {
                        garbage.Push(new Vector2(b, i));
                    }
                    else
					{
                        matrices[dynamicParticleIndex][b][i].SetColumn(3, column);
                    }
                }
                else if (column.z == dynamicZDepth)
                {
                    column.z = 0;
                    matrices[dynamicParticleIndex][b][i].SetColumn(3, column);
                }
            }
        }

        Vector2 g;
        int d = 0;
        while (garbage.Count > 0)
		{
            g = garbage.Pop();
            RemoveParticle(dynamicParticleIndex, (int)g.x, (int)g.y);
            d++;
        }
        return d;
    }

    public void SweepLine(Vector2 center, Vector2 direction, float lineWidth)
	{
 
        const float inverseCurvature = 50;
        Matrix4x4 matrix;
        float orbitDistance = 0;
        float originDistance = 0;
        Vector4 column;
        Vector2 origin = center - direction.normalized * inverseCurvature;
        Vector2 orbit = center + direction / 2;
        for (int b = 0; b < matrices[dynamicParticleIndex].Count; b++)
        {
            int particlesInBatch = batchSize;
            if (b == matrices[dynamicParticleIndex].Count - 1)
            {
                particlesInBatch = offsets[dynamicParticleIndex] % batchSize + 1;
            }

            for (int i = 0; i < particlesInBatch; i++)
            {
                // runs for each particle of a certain color, with the color alternating each frame
                matrix = matrices[dynamicParticleIndex][b][i];
                column = matrix.GetColumn(3);
                orbitDistance = Vector2.Distance(column, orbit);

                bool inRect = false;
                if (orbitDistance < lineWidth)
                {
                    originDistance = Vector2.Distance(column, origin);
                    if (originDistance > inverseCurvature && originDistance < inverseCurvature + direction.magnitude)
					{
                        inRect = true;
					}
                }

                if (inRect)
				{
                    Vector2 newPos = Vector2.MoveTowards(column, origin, originDistance - inverseCurvature);
                    column.x = newPos.x;
                    column.y = newPos.y;
                    //column.z = dynamicZDepth;
                    matrices[dynamicParticleIndex][b][i].SetColumn(3, column);
                }
                else if (column.z == dynamicZDepth)
                {
                    column.z = 0;
                    matrices[dynamicParticleIndex][b][i].SetColumn(3, column);
                }
            }
        }
    }

    public Vector2 PollRandomSpawnLocation()
	{
        if (particleCount == 0)
        {
            return Vector2.zero;
        }

        // selects a uniformly random particle
        int particleIndex = Random.Range(0, particleCount);


        Vector3 particlePosition = GetParticlePositionFromIndex(particleIndex);

        int c = (int)particlePosition.x;
        int b = (int)particlePosition.y;
        int i = (int)particlePosition.z;

        //Debug.Log("particle index: " + particleIndex + " c: " + c + " b: " + b + " i: " + i);
        // finally we can find the particle
        Matrix4x4 particle = matrices[c][b][i];
        Vector4 column = particle.GetColumn(3);

        RaycastHit2D hit = Physics2D.Raycast(column, Vector2.zero, LayerMask.GetMask("Furniture"));
        
        float duration = 1f;
        if (hit.collider == null)
		{
            Debug.DrawRay(column, Vector2.up, Color.red, duration);
            Debug.DrawRay(column, (Vector2.up + Vector2.left).normalized * 0.2f, Color.red, duration);
            Debug.DrawRay(column, (Vector2.up + Vector2.right).normalized * 0.2f, Color.red, duration);
        }
        else
		{
            Debug.DrawRay(column, Vector2.up, Color.green, duration);
            Debug.DrawRay(column, (Vector2.up + Vector2.left).normalized * 0.2f, Color.green, duration);
            Debug.DrawRay(column, (Vector2.up + Vector2.right).normalized * 0.2f, Color.green, duration);
        }
        

        if (hit.collider != null)
        {
            return column;
        }
        else
        {
            return Vector2.zero;
        }
	}

    private Vector3 GetParticlePositionFromIndex(int index)
	{
        // gets the color of the particle
        int c = 0;
        while (true)
        {
            if (c == materials.Length)
			{
                Debug.LogError("no particle found");
                return new Vector3(-1, -1, -1);
			}
            index -= offsets[c] + 1;
            if (index < 0)
            {
                break;
            }
            c++;
        }

        // gets the index of the particle in the color
        index += offsets[c] + 1;

        // gets the batch of the particle
        int b = index / batchSize;

        // gets the index of the particle in the batch
        int i = index - b * batchSize;

        return new Vector3(c, b, i);
    }
}
