using System;
using UnityEngine;

public class TerrainManager : MonoBehaviour {

	public GameObject terrainPrefab;
	public GameObject wallPrefab;

	public int width = 128;
	public int height = 128;
	public bool randomSeed = false;
	public float sampleSize = 16;

	public bool WallsKill = false;
		 
	private GameObject terrainInstance;
	private TerrainGenerator generator;

	public Texture2D TopoMap {
		get {
			if (generator.TopoMap == null) {
				throw new NullReferenceException();
			}
			return generator.TopoMap;
		}
	}
	public Texture2D DataMap {
		get {
			if (generator.DataMap == null) {
				throw new NullReferenceException();
			}
			return generator.DataMap;
		}
	}

	void Start () {

		generator = new TerrainGenerator (width, height, randomSeed);

		generator.sampleSize = sampleSize;

		generator.Generate();

		GameManager.instance.onTerrainGenerated();

		terrainInstance = Instantiate(terrainPrefab);

		SpriteRenderer renderer = terrainInstance.GetComponent<SpriteRenderer> ();

		renderer.sprite = Sprite.Create(generator.TopoMap, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 1f);

		CreateWalls();
	}

	private void CreateWalls () {	
		GameObject wall = Instantiate(wallPrefab);
		wall.GetComponent<BoxCollider2D>().isTrigger = WallsKill;
		wall.GetComponent<BoxCollider2D>().size = new Vector2(width, 1);
		wall.transform.position = new Vector3(0, 1f + height / 2, 0);

		wall = Instantiate(wallPrefab);
		wall.GetComponent<BoxCollider2D>().isTrigger = WallsKill;
		wall.GetComponent<BoxCollider2D>().size = new Vector2(width, 1);
		wall.transform.position = new Vector3(0, -1f - height / 2, 0);

		wall = Instantiate(wallPrefab);
		wall.GetComponent<BoxCollider2D>().isTrigger = WallsKill;
		wall.GetComponent<BoxCollider2D>().size = new Vector2(1, height);
		wall.transform.position = new Vector3(1f + width/2, 0, 0);

		wall = Instantiate(wallPrefab);
		wall.GetComponent<BoxCollider2D>().isTrigger = WallsKill;
		wall.GetComponent<BoxCollider2D>().size = new Vector2(1, height);
		wall.transform.position = new Vector3(-1f - width/2, 0, 0);
		
	}
	
	
	// Update is called once per frame
	void Update () {
	
	}
}
