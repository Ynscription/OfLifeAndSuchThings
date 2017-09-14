using UnityEngine;
using System;
using Random = UnityEngine.Random;
using LibNoise.Generator;
using LibNoise;
using System.Collections.Generic;
using LibNoise.Operator;

public class TerrainGenerator {

	public int height;
	public int width;

	public bool randomSeed;


	public float sampleSize = 16;

	private float baseflatFrequency = 2.0f;

	private float flatScale = 0.125f;
	private float flatBias = -0.75f;

	//private float terraintypeFrequency = 0.5f;
	//private float terraintypePersistence = 0.25f;

	//private float landTerrainEdgeFalloff = 0.125f;
	private float oceanTerrainEdgeFalloff = 0.3f;

	private int seed = 0;
	private int landSeed = 50;


	private Gradient gradient;

	private List<GradientColorKey> terrainColorKeys = new List<GradientColorKey>
		{

				new GradientColorKey(new Color(0, 0, 0.5f), 0),//Deeps
				new GradientColorKey(new Color(0f, 0f, 1f), 0.2f),//Shallow
				new GradientColorKey(new Color(0f, 0.5f, 1f), 0.3f),//Shore
				new GradientColorKey(new Color(0.94f, 0.94f, 0.25f), 0.35f),//Sand
				//new GradientColorKey(new Color(0.125f, 0.625f, 0), 0.4f),//Grass1
				new GradientColorKey(new Color(0.100f, 0.55f, 0), 0.5f),//Grass2
				new GradientColorKey(new Color(0.39f, 0.25f, 0), 0.85f),//Dirt
				new GradientColorKey(new Color(0.5f, 0.5f, 0.5f), 0.95f),//Rock
				new GradientColorKey(Color.white, 1)//snow

		};
	private List<GradientAlphaKey> alphaKeys = new List<GradientAlphaKey> { new GradientAlphaKey(1, 0), new GradientAlphaKey(1, 1) };

	private Texture2D topoMap;
	private Texture2D dataMap;
	//private Texture2D grid;

	public Texture2D TopoMap {
		get {
			if (topoMap == null) {
				throw new NullReferenceException();
			}
			return topoMap;
		}
	}
	public Texture2D DataMap {
		get {
			if (dataMap == null) {
				throw new NullReferenceException();
			}
			return dataMap;
		}
	}
	

	private void generateSeed() {
		seed = Random.Range(0, Int32.MaxValue - 1);
		landSeed = Random.Range(0, Int32.MaxValue - 1);
		Debug.Log("Seed: " + seed);


	}

	public TerrainGenerator (int w, int h, bool r) {
		width = w;
		height = h;
		randomSeed = r;

	}


	// Use this for initialization
	public void Generate() {
		if (randomSeed) {
			generateSeed();
		}

		RidgedMultifractal baseMountainTerrain = new RidgedMultifractal();
		baseMountainTerrain.Seed = seed;
		baseMountainTerrain.Frequency = 0.4f;

		ScaleBias mountainTerrain = new ScaleBias(0.4f, 0.6f, baseMountainTerrain);

		Billow baseLandTerrain = new Billow();
		baseLandTerrain.Seed = seed;
		baseLandTerrain.Frequency = baseflatFrequency;

		ScaleBias landTerrain = new ScaleBias(0.3f, -0.1f, baseLandTerrain);

		Perlin landTerrainType = new Perlin();
		landTerrainType.Frequency = 0.5f;
		landTerrainType.Persistence = 0.5f;
		landTerrainType.Seed = landSeed;


		Select finalLandTerrain = new Select(landTerrain, mountainTerrain, landTerrainType);
		finalLandTerrain.SetBounds(0, 1000);
		finalLandTerrain.FallOff = 0.6f;

		Billow baseOceanTerrain = new Billow();
		baseOceanTerrain.Seed = seed;
		baseOceanTerrain.Frequency = baseflatFrequency;

		ScaleBias oceanTerrain = new ScaleBias(flatScale, flatBias, baseOceanTerrain);

		Perlin worlTerrainType = new Perlin();
		worlTerrainType.Frequency = 0.5f;
		worlTerrainType.Persistence = 0.5f;
		worlTerrainType.Seed = seed;

		Select finalTerrain = new Select(oceanTerrain, finalLandTerrain, worlTerrainType);
		finalTerrain.SetBounds(0, 1000);
		finalTerrain.FallOff = oceanTerrainEdgeFalloff;

		ScaleBias trueFinalTerrain = new ScaleBias(0.9f, 0.1f, finalTerrain);

		ModuleBase module = trueFinalTerrain;

		Noise2D heightMap = new Noise2D(width, height, module);

		//heightMap.GeneratePlanar(0, sampleSize * (1024 / height), 0, sampleSize * (1024 / width));
		heightMap.GeneratePlanar(0, sampleSize * ((float)width /1024), 0, sampleSize * ((float)height /1024));

		gradient = new Gradient();
		gradient.SetKeys(terrainColorKeys.ToArray(), alphaKeys.ToArray());

		dataMap= heightMap.GetTexture();
		dataMap.filterMode = FilterMode.Point;
		topoMap = heightMap.GetTexture(gradient);
		topoMap.filterMode = FilterMode.Point;
		
	}

	

	// Update is called once per frame
	void Update () {
	
	}
}
