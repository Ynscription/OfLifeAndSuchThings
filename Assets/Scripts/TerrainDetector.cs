using UnityEngine;
using System.Collections;

public class TerrainDetector : MonoBehaviour {

	private float red = 1;
	private float green = 0;
	private float blue = 1;

	public float Red {
		get {
			return red;
		}
	}
	public float Green {
		get {
			return green;
		}
	}
	public float Blue {
		get {
			return blue;
		}
	}


	//private SpriteRenderer rend;

	// Use this for initialization
	void Start () {
		//rend = GetComponent<SpriteRenderer>();
	}

	void FixedUpdate() {
		byte r, g ,b;
		GameManager.instance.GetTerrainColor(transform.position, out r, out g, out b);
		red = Utils.ColorByteToFloat(r);
		green = Utils.ColorByteToFloat(g);
		blue = Utils.ColorByteToFloat(b);
		
	}

	// Update is called once per frame
	void Update () {
		//rend.color = new Color(red, green, blue);
	}
}
