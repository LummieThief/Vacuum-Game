using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirtController : MonoBehaviour
{
    public static DirtController instance;
    public const float pixelSize = 0.2f;
    private const int batchSize = 1000;
    [SerializeField] int numStartingParticles;
    [SerializeField] Vector2 startingParticleArea;
    [SerializeField] float mouseSuckSpeed;
    [SerializeField] float mouseSuckDistance;
    [SerializeField] float mouseDestroyDistance;

    // Material to use for drawing the meshes.
    public Material[] materials;

    private List<Matrix4x4[]>[] matrices;
    private int[] offsets;

    private int dynamicParticleIndex = 0;
    private const float dynamicZDepth = -0.1f;

    private Vector2 lastMousePos = Vector2.zero;
    private Vector2 lastLastMousePos = Vector2.zero;

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


        mesh = CreateQuad();

        matrices = new List<Matrix4x4[]>[materials.Length];
        for (int c = 0; c < matrices.Length; c++)
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
        for (int i = 0; i < numStartingParticles; i++)
		{
            AddParticle(new Vector2(Random.Range(-startingParticleArea.x / 2, startingParticleArea.x / 2), Random.Range(-startingParticleArea.y / 2, startingParticleArea.y / 2)));
		}
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
    public int AddParticle(Vector2 pos)
    {
        int c = Random.Range(0, matrices.Length);
        return AddParticle(pos, c);
    }

    // adds a particle of a specific material
    public int AddParticle(Vector2 pos, int c)
    {
        List<Matrix4x4[]> l = matrices[c];
        if (offsets[c] > 0 && offsets[c] % batchSize == 0)
        {
            l.Add(new Matrix4x4[batchSize]);
        }

        // Build matrix.
        Vector3 position = pos;
        Quaternion rotation = Quaternion.identity;
        Vector3 scale = Vector3.one;

        offsets[c]++;
        l[l.Count - 1][offsets[c] % batchSize] = Matrix4x4.TRS(position, rotation, scale);
        

        return c;
    }

    private void RemoveParticle(int c, int b, int i)
	{
        List<Matrix4x4[]> colorMatrix = matrices[c];
        
        int offsetInLastMatrix = offsets[c] % batchSize;

        int batchIndex = Mathf.FloorToInt(offsets[c] / batchSize);

        Matrix4x4 lastParticle = colorMatrix[batchIndex][offsetInLastMatrix];

        colorMatrix[b][i] = lastParticle;
        offsets[c]--;
        if (offsetInLastMatrix == 0 && colorMatrix.Count > 1)
		{
            colorMatrix.RemoveAt(colorMatrix.Count - 1);
		}
        
    }

    private void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetKey(KeyCode.Mouse0))
		{
            // An example of how to use SuckSlice
            SuckSlice(mousePos, (lastMousePos - lastLastMousePos).normalized * mouseSuckDistance, 90, mouseSuckSpeed, mouseDestroyDistance);
        }

        if (lastMousePos != mousePos)
		{
            lastLastMousePos = lastMousePos;
            lastMousePos = mousePos;
		}
        

        dynamicParticleIndex++;
        if (dynamicParticleIndex >= materials.Length)
		{
            dynamicParticleIndex = 0;
		}

        string s = "offsets: ";
        for (int i = 0; i < materials.Length; i++)
		{
            s += offsets[i] + ", ";
		}
        Debug.Log(s);

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
    public void SuckSlice(Vector2 center, Vector2 direction, float degrees, float suckSpeed, float destroyDistance)
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
        while (garbage.Count > 0)
		{
            g = garbage.Pop();
            RemoveParticle(dynamicParticleIndex, (int)g.x, (int)g.y);
        }
    }
}
