using UnityEngine;
using System;
using Random = UnityEngine.Random;
using UnityEngine.UI;


public class GameManager : MonoBehaviour {

	public static GameManager instance = null;
	public GameObject creature;
	
	public bool populationLimit = true;
	public int maxPopulation = 100;
	public float spawnChance = 0.01f;

	private int population;

	private Text terrText;
	private Text popText;
	private Text fpsText;
	private Text pfpsText;

	private GameObject miniMap;
	public RectTransform MiniMap {
		get { return miniMap.GetComponent<RectTransform>();}
	}
	//private GameObject miniMapBorder;

	private TerrainManager terrainManager;

	private byte[,] terrLookUp;
	private byte [,] redLookUp;
	private byte[,] greenLookUp;
	private byte[,] blueLookUp;

	private float halfTerrH;
	private float halfTerrW;

	private int frameCount = 0;
	private float dt = 0.0f;
	private float fps = 0.0f;
	private float updateRate = 0.25f;  // 4 updates per sec.

	private bool kOS = false;

	private GameObject units;


	void Awake () {
		if (instance == null) {
			instance = this;
		}else if (instance != this) {
			Destroy(gameObject);
		}
		DontDestroyOnLoad(gameObject);
		terrainManager = GetComponent<TerrainManager>();
		population = 0;
		Random.seed = DateTime.Now.Millisecond;

		terrText = GameObject.Find ("TerrainText").GetComponent<Text>();
		popText = GameObject.Find("PopulationText").GetComponent<Text>();
		fpsText = GameObject.Find ("FPSText").GetComponent<Text>();

		miniMap = GameObject.Find ("MinimapImage");
		//miniMapBorder = GameObject.Find ("MinimapBorder");
		

units = new GameObject ("Units");
		
		
	}

	// Use this for initialization
	void Start () {
		
	}
	


	// Update is called once per frame
	void Update () {
		Populate ();
		if (Input.GetMouseButton(0)) {
			Vector2 pz = Camera.main.ScreenToWorldPoint(Input.mousePosition);

			float terr = GetTerrainInfo(pz);
			int x = Mathf.FloorToInt(pz.x);
			int y = Mathf.FloorToInt(pz.y);

			terrText.text = "Position: " + x + "," + y + "\nTerrain: " + terr.ToString("0.00000");
		}

		if (Input.GetMouseButtonUp(1)) {
			kOS = !kOS;
		}


		frameCount++;
		dt += Time.deltaTime;
		if (dt > updateRate) {
			fps = frameCount / dt;
			frameCount = 0;
			dt -= updateRate;

			fpsText.text = "fps: " + fps.ToString("00.00");
		}


	}

	public byte getTerrHeight(Vector2 v) {
		if (kOS) {
			return 0;
		}

		int posx = Mathf.FloorToInt(v.x) + (int)halfTerrW;
		int posy = Mathf.FloorToInt(v.y) + (int)halfTerrH;

		if (posx < 0) {
			posx = 0;
		} else if (posx >= terrainManager.width) {
			posx = terrainManager.width - 1;
		}
		if (posy < 0) {
			posy = 0;
		} else if (posy >= terrainManager.height) {
			posy = terrainManager.height - 1;
		}

		return terrLookUp[posx, posy];
	}

	public void GetTerrainColor(Vector2 v, out byte r, out byte g, out byte b) {
		int posx = Mathf.FloorToInt(v.x) + (int)halfTerrW;
		int posy = Mathf.FloorToInt(v.y) + (int)halfTerrH;

		if (posx < 0) {
			posx = 0;
		}else if (posx >= terrainManager.width) {
			posx = terrainManager.width -1;
		}
		if (posy < 0) {
			posy = 0;
		}else if (posy >= terrainManager.height) {
			posy = terrainManager.height - 1;
		}

		r = redLookUp[posx, posy];
		g = greenLookUp[posx, posy];
		b = blueLookUp[posx, posy];

	}


	public void onTerrainGenerated() {
		terrLookUp = new byte[terrainManager.width, terrainManager.height];
		redLookUp = new byte[terrainManager.width, terrainManager.height];
		greenLookUp = new byte[terrainManager.width, terrainManager.height];
		blueLookUp = new byte[terrainManager.width, terrainManager.height];
		for (int i = 0; i < terrainManager.width; i++) {
			for (int j = 0; j < terrainManager.height; j++) {
				terrLookUp[i, j] = (byte)(GetTerrainInfo(i, j) * 255);
				Color c = GetTerrainColor(i, j);
				redLookUp[i, j] = (byte)(c.r * 255);
				greenLookUp[i, j] = (byte)(c.g * 255);
				blueLookUp[i, j] = (byte)(c.b * 255);
			}
		}

		halfTerrH = terrainManager.height / 2;
		halfTerrW = terrainManager.width / 2;

		miniMap.GetComponent<Image>().sprite = Sprite.Create(terrainManager.TopoMap, new Rect(0, 0, terrainManager.width, terrainManager.height), new Vector2(0.5f, 0.5f));
		RectTransform rt = miniMap.GetComponent<RectTransform>();
		float ratio = ((float)terrainManager.width/(float)terrainManager.height);
		if (ratio < 3f/2f) {
			rt.sizeDelta = new Vector2(200f * ratio, 200f);
		} else {
			rt.sizeDelta = new Vector2(300f, 200f/ratio);
		}

	}


	public void onCreatureDie (GameObject c) {
		Destroy(c);
		population--;
		popText.text = "Population: " + population.ToString("0,000");
	}

	

	private float GetTerrainInfo(int posx, int posy) {
		return terrainManager.DataMap.GetPixel(posx, posy).grayscale;
	}

	private float GetTerrainInfo(Vector2 v) {
		int posx = Mathf.FloorToInt(v.x) + (int)halfTerrW;
		int posy = Mathf.FloorToInt(v.y) + (int)halfTerrH;

		return terrainManager.DataMap.GetPixel(posx, posy).grayscale;		
	}

	private Color GetTerrainColor(int posx, int posy) {
		return terrainManager.TopoMap.GetPixel(posx, posy);
	}

	private	void BatchSprites () {
		units.SetActive(!units.activeSelf);
	}
	


	private void Populate () {
		if (population < maxPopulation || !populationLimit) {
			if (Random.value <= spawnChance) {
				float x = Random.Range (-halfTerrW, halfTerrW);
				float y = Random.Range(-halfTerrH, halfTerrH);
				float angle = Random.Range(0, 360);
				population++;
				GameObject go =(GameObject) Instantiate (creature, new Vector3 (x, y, 0), Quaternion.AngleAxis( angle, new Vector3(0, 0, 1)));
				//Instantiate(creature, new Vector3(x, y, 0), Quaternion.AngleAxis(angle, new Vector3(0, 0, 1)));
				go.transform.parent = units.transform;
				popText.text = "Population: " + population.ToString("0,000");

			}
		}
	}
}
