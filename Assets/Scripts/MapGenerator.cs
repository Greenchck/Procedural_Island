using UnityEngine;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour
{
    [Header("Chunk Settings")]
    public int chunkGridSizeX = 8;
    public int chunkGridSizeZ = 8;
    public int chunkSize = 20;
    [Range(1f, 10f)] public float vertexSpacing = 1f;

    [Header("Visual Settings")]
    public Material terrainMaterial;
    public bool useFlatShading = true;

    [Header("Noise Settings")]
    public float noiseScale = 0.05f; 
    public float heightMultiplier = 10f;
    public Vector2 noiseOffset;

    [Header("Falloff Settings")]
    public bool useFalloff = true;
    [Range(1f, 10f)] public float falloffA = 3f;
    [Range(1f, 10f)] public float falloffB = 2.2f;

    [System.Serializable]
    public struct RegionBiome
    {
        public string name;
        public Color color;
        [Range(1, 100)] public int spawnWeight;
    }

    [Header("Region Settings")]
    public int regionCount = 8;
    public float boundaryNoiseScale = 0.05f;
    public float boundaryNoiseAmount = 25f;

    [Header("Available Biomes")]
    public RegionBiome[] availableBiomes;

    TerrainChunk[,] chunks;
    
    Vector2[] regionCenters;
    RegionBiome[] regionBiomeAssignments;

    void Start()
    {
        if (availableBiomes == null || availableBiomes.Length == 0)
        {
            Debug.LogError("Add Biomes to Generate Map");
            return;
        }

        InitializeRegions();
        GenerateMap();
    }

    public void Button_RegenerateMap()
    {
        if (availableBiomes == null || availableBiomes.Length == 0) return;
        
        noiseOffset = new Vector2(Random.Range(-10000f, 10000f), Random.Range(-10000f, 10000f));
        
        InitializeRegions();
        GenerateMap();
    }

    void InitializeRegions()
    {
        float mapSizeX = chunkGridSizeX * chunkSize * vertexSpacing;
        float mapSizeZ = chunkGridSizeZ * chunkSize * vertexSpacing;

        regionCenters = new Vector2[regionCount];
        regionBiomeAssignments = new RegionBiome[regionCount];

        List<RegionBiome> deck = new List<RegionBiome>();
        
        int totalWeight = 0;
        foreach (var biome in availableBiomes) totalWeight += biome.spawnWeight;

        int remainingRegions = regionCount;

        foreach (var biome in availableBiomes)
        {
            if (remainingRegions > 0)
            {
                deck.Add(biome);
                remainingRegions--;
            }
        }

        if (remainingRegions > 0)
        {
            float[] fractionalCounts = new float[availableBiomes.Length];
            int[] extraCounts = new int[availableBiomes.Length];

            for (int i = 0; i < availableBiomes.Length; i++)
            {
                float exactCount = ((float)availableBiomes[i].spawnWeight / totalWeight) * remainingRegions;
                extraCounts[i] = Mathf.FloorToInt(exactCount);
                fractionalCounts[i] = exactCount - extraCounts[i];
                
                for (int k = 0; k < extraCounts[i]; k++)
                {
                    deck.Add(availableBiomes[i]);
                }
            }

            int unassigned = regionCount - deck.Count;
            while (unassigned > 0)
            {
                float maxFraction = -1f;
                int maxIndex = 0;
                for (int i = 0; i < availableBiomes.Length; i++)
                {
                    if (fractionalCounts[i] > maxFraction)
                    {
                        maxFraction = fractionalCounts[i];
                        maxIndex = i;
                    }
                }
                deck.Add(availableBiomes[maxIndex]);
                fractionalCounts[maxIndex] = -1f;
                unassigned--;
            }
        }

        for (int i = 0; i < deck.Count; i++)
        {
            RegionBiome temp = deck[i];
            int randomIndex = Random.Range(i, deck.Count);
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }

        for (int i = 0; i < regionCount; i++)
        {
            Vector2 randomPoint = Vector2.zero;
            bool foundLand = false;
            int maxAttempts = 100;

            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                float rx = Random.Range(0, mapSizeX);
                float rz = Random.Range(0, mapSizeZ);
                
                if (GetElevation(rx, rz) >= 0.2f) 
                {
                    randomPoint = new Vector2(rx, rz);
                    foundLand = true;
                    break;
                }
            }

            if (!foundLand)
            {
                randomPoint = new Vector2(Random.Range(0, mapSizeX), Random.Range(0, mapSizeZ));
            }

            regionCenters[i] = randomPoint;
            regionBiomeAssignments[i] = deck[i];
        }
    }

    public void GenerateMap()
    {
        if (chunks != null)
        {
            foreach (var chunk in chunks)
            {
                if (chunk != null) Destroy(chunk.chunkObject);
            }
        }

        chunks = new TerrainChunk[chunkGridSizeX, chunkGridSizeZ];

        for (int z = 0; z < chunkGridSizeZ; z++)
        {
            for (int x = 0; x < chunkGridSizeX; x++)
            {
                Vector2 chunkPosition = new Vector2(x, z);
                chunks[x, z] = new TerrainChunk(chunkPosition, this);
            }
        }
    }

    public float GetElevation(float worldX, float worldZ)
    {
        float mapSizeX = chunkGridSizeX * chunkSize * vertexSpacing;
        float mapSizeZ = chunkGridSizeZ * chunkSize * vertexSpacing;

        float normalizedX = (worldX / vertexSpacing) + noiseOffset.x;
        float normalizedZ = (worldZ / vertexSpacing) + noiseOffset.y;

        float noiseValue = Mathf.PerlinNoise(normalizedX * noiseScale, normalizedZ * noiseScale);

        if (useFalloff)
        {
            float nx = (worldX / mapSizeX) * 2 - 1;
            float nz = (worldZ / mapSizeZ) * 2 - 1;

            float distance = Mathf.Clamp01(new Vector2(nx, nz).magnitude);
            float falloffValue = EvaluateFalloff(distance);
            noiseValue = Mathf.Clamp01(noiseValue - falloffValue);
        }

        return noiseValue;
    }

    public Color GetBiomeColor(float worldX, float worldZ, float elevation)
    {
        if (elevation < 0.2f) return new Color(0.9f, 0.8f, 0.5f);

        float normalizedX = worldX / vertexSpacing;
        float normalizedZ = worldZ / vertexSpacing;

        float offsetX = (Mathf.PerlinNoise(normalizedX * boundaryNoiseScale, normalizedZ * boundaryNoiseScale) * 2 - 1) * boundaryNoiseAmount * vertexSpacing;
        float offsetZ = (Mathf.PerlinNoise(normalizedX * boundaryNoiseScale + 1000, normalizedZ * boundaryNoiseScale + 1000) * 2 - 1) * boundaryNoiseAmount * vertexSpacing;

        Vector2 distortedPos = new Vector2(worldX + offsetX, worldZ + offsetZ);

        float minDistance = float.MaxValue;
        int closestIndex = 0;

        for (int i = 0; i < regionCount; i++)
        {
            float dist = Vector2.SqrMagnitude(distortedPos - regionCenters[i]);
            if (dist < minDistance)
            {
                minDistance = dist;
                closestIndex = i;
            }
        }

        return regionBiomeAssignments[closestIndex].color;
    }

    float EvaluateFalloff(float value)
    {
        float a = falloffA;
        float b = falloffB;
        return Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow(b - b * value, a));
    }
}
