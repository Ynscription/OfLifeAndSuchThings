using UnityEngine;
using System.Collections;

public class ColorDetector : MonoBehaviour {

	public GameObject target;
	public GameObject distBorder;
	public LayerMask layer;

	private RaycastHit2D [] results;


	//private Collider2D self;
	private float red = 1;
	private float green = 0;
	private float blue = 1;
	private bool hit = false;

	private int doDetectStep = 0;
	
	//private bool colorChanged = false;
	

	private SpriteRenderer rend;

	public float Red {
		get { return red;}
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
	public bool Hit {
		get {
			return hit;
		}
	}


	/*public Collider2D Self {
		//set {self = value; }
	}*/

	// Use this for initialization
	void Start () {
		rend = GetComponent<SpriteRenderer>();
				
		results = new RaycastHit2D [1];


	}
	// Update is called once per frame
	void FixedUpdate () {
		//TODO RayCastHit2D Consumes a lot, maybe it can be optimized.
		Vector2 start = distBorder.transform.position;
		Vector2 end = target.transform.position;
		//self.enabled = false;
		//Debug.DrawLine(start, end, Color.red, Time.fixedDeltaTime);
		if (doDetectStep == 2) {
			doDetectStep = 0;

			int size = Physics2D.LinecastNonAlloc (start, end, results, layer);
			RaycastHit2D hitTg;
			//RaycastHit2D hitTg = Physics2D.Linecast(start, end, layer);

			if (size > 0) {
				hitTg = results [0];
			
				if (hitTg.transform.gameObject.tag == "Entity") {
					Entity e = hitTg.transform.gameObject.GetComponent<Entity>();
					if (e != null) {
						red = e.Red;
						green = e.Green;
						blue = e.Blue;
						hit = true;
						//colorChanged = true;
					}

				}
			}else {
				hit = false;
				byte r, g, b;
				GameManager.instance.GetTerrainColor(target.transform.position, out r, out g, out b);
				red = Utils.ColorByteToFloat (r);
				green = Utils.ColorByteToFloat(g);
				blue = Utils.ColorByteToFloat(b);

			}
		}
		doDetectStep++;
	}

	void Update () {
		rend.color = new Color (red, green, blue, 1);
		
	}
}
