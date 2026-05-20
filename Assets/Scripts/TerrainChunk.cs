using UnityEngine;

public class TerrainChunk
{
    public GameObject chunkObject;
    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;
    Color[] colors;

    MapGenerator mapGenerator;
    Vector2 chunkPos;

    public TerrainChunk(Vector2 chunkPos, MapGenerator mapGenerator)
    {
        this.chunkPos = chunkPos;
        this.mapGenerator = mapGenerator;

        chunkObject = new GameObject($"Chunk_{chunkPos.x}_{chunkPos.y}");
        
        chunkObject.transform.position = new Vector3(
            chunkPos.x * mapGenerator.chunkSize * mapGenerator.vertexSpacing, 
            0, 
            chunkPos.y * mapGenerator.chunkSize * mapGenerator.vertexSpacing
        );
        chunkObject.transform.parent = mapGenerator.transform;

        MeshRenderer renderer = chunkObject.AddComponent<MeshRenderer>();
        MeshFilter filter = chunkObject.AddComponent<MeshFilter>();

        if (mapGenerator.terrainMaterial != null)
        {
            renderer.material = mapGenerator.terrainMaterial;
        }

        mesh = new Mesh();
        filter.mesh = mesh;

        GenerateChunk();
    }

    void GenerateChunk()
    {
        int size = mapGenerator.chunkSize;
        vertices = new Vector3[(size + 1) * (size + 1)];
        colors = new Color[(size + 1) * (size + 1)];

        int i = 0;
        for (int z = 0; z <= size; z++)
        {
            for (int x = 0; x <= size; x++)
            {
                float worldX = chunkObject.transform.position.x + (x * mapGenerator.vertexSpacing);
                float worldZ = chunkObject.transform.position.z + (z * mapGenerator.vertexSpacing);

                float elevation = mapGenerator.GetElevation(worldX, worldZ);
                float y = elevation * mapGenerator.heightMultiplier;
                
                vertices[i] = new Vector3(x * mapGenerator.vertexSpacing, y, z * mapGenerator.vertexSpacing);
                colors[i] = mapGenerator.GetBiomeColor(worldX, worldZ, elevation);
                i++;
            }
        }

        triangles = new int[size * size * 6];
        int vert = 0;
        int tris = 0;

        for (int z = 0; z < size; z++)
        {
            for (int x = 0; x < size; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + size + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + size + 1;
                triangles[tris + 5] = vert + size + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }

        if (mapGenerator.useFlatShading)
        {
            ApplyFlatShading();
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;
        mesh.RecalculateNormals();
    }

    void ApplyFlatShading()
    {
        Vector3[] flatVertices = new Vector3[triangles.Length];
        Color[] flatColors = new Color[triangles.Length];

        for (int i = 0; i < triangles.Length; i += 6)
        {
            Color quadColor = colors[triangles[i]];

            for (int j = 0; j < 6; j++)
            {
                flatVertices[i + j] = vertices[triangles[i + j]];
                flatColors[i + j] = quadColor;
                triangles[i + j] = i + j;
            }
        }
        
        vertices = flatVertices;
        colors = flatColors;
    }
}
