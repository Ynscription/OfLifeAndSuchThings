using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {


	public float speed;
	public Texture texture;

	private int terrainWidth;
	private int terrainHeight;
	private Vector2 camSize;
	private int halfTerrW;
	private int halfTerrH;


	private RectTransform miniMapTip;
	private Vector2 miniMapSize = Vector2.zero;

	

	// Use this for initialization
	void Start() {
		Loader l = GetComponent<Loader>();
		TerrainManager t = l.gameManager.GetComponent<TerrainManager>();
		terrainWidth = t.width;
		terrainHeight = t.height;
		halfTerrW = terrainWidth/2;
		halfTerrH = terrainHeight / 2;

		miniMapTip = GameObject.Find("MinimapTip").GetComponent<RectTransform>();


	}

	private void tryGetMiniMap() {
		Vector2 tryGet = GameManager.instance.MiniMap.sizeDelta;
		if (!(tryGet.x <= 1 && tryGet.y <= 1)) {
			miniMapSize = tryGet;
		}
	}
	
	// Update is called once per frame
	void Update () {
		float moveHorizontal = Input.GetAxis("Horizontal");
		float moveVertical = Input.GetAxis("Vertical");
		
		float zoom = Input.GetAxis("Mouse ScrollWheel");

		if (moveHorizontal != 0f || moveVertical != 0f) {
			Vector3 movement = new Vector3(moveHorizontal, moveVertical, 0.0f);
		

			transform.Translate (movement*Camera.main.orthographicSize * speed);
			
		}


		
		camSize.x = Camera.main.orthographicSize*Camera.main.aspect;
		camSize.y = Camera.main.orthographicSize;
		
		Vector2 newCamSize = new Vector2();
		if (Camera.main.aspect <= 1) {
			if (camSize.y - zoom * camSize.y > halfTerrH) {
				newCamSize.y = halfTerrH;
			}else {
				newCamSize.y = Mathf.Max((Camera.main.orthographicSize - zoom * Camera.main.orthographicSize), 1);
			}
		}else {
			if (camSize.x - zoom * camSize.x > halfTerrW) {
				float aspect = 1 / Camera.main.aspect;
				newCamSize.y = (halfTerrW) * aspect;
			}else if (camSize.y - zoom * camSize.y > halfTerrH) {
				newCamSize.y = halfTerrH;
			} else {
				newCamSize.y = Mathf.Max((Camera.main.orthographicSize - zoom * Camera.main.orthographicSize), 1);
			}
		}
		if (newCamSize.y > 40) {
			newCamSize.y = 40;
		}

		newCamSize.x = newCamSize.y*Camera.main.aspect;

		Vector3 pos = transform.position;
		if (pos.x > halfTerrW - newCamSize.x) {
			transform.position = new Vector3(halfTerrW - newCamSize.x, transform.position.y, transform.position.z);
		} else if (pos.x < (-halfTerrW) + newCamSize.x) {
			transform.position = new Vector3((-halfTerrW) + newCamSize.x, transform.position.y, transform.position.z);
		}
		if (pos.y > halfTerrH - newCamSize.y) {
			transform.position = new Vector3(transform.position.x, halfTerrH - newCamSize.y, transform.position.z);
		} else if (pos.y < (-halfTerrH) + newCamSize.y) {
			transform.position = new Vector3(transform.position.x, (-halfTerrH) + newCamSize.y, transform.position.z);
		}

		if (miniMapSize != Vector2.zero) {

			int addx = 0;
			int addy = 0;
			if (terrainWidth % 2 != 0) {
				addx++;
			}
			if (terrainHeight % 2 != 0) {
				addy++;
			}

			float ratiox = terrainWidth / newCamSize.x;
			float ratioy = terrainHeight / newCamSize.y;
			float x = (miniMapSize.x + addx) / ratiox;
			float y = (miniMapSize.y + addy) / ratioy;			
			miniMapTip.sizeDelta = new Vector2 (x*2,y*2);
			

			

			float posRatiox = (float)(miniMapSize.x / terrainWidth);
			float posRatioy = (float)(miniMapSize.y / terrainHeight);
			miniMapTip.anchoredPosition = new Vector2 (transform.position.x*posRatiox, transform.position.y*posRatioy);

		} else {
			tryGetMiniMap();
		}

			


		Camera.main.orthographicSize = newCamSize.y;
		

		

	}
}
