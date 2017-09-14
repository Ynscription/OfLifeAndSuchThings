using UnityEngine;
using System.Collections;

public abstract class Entity : MonoBehaviour {
	
	public float moveSpeed = 0.1f;
	public float turnSpeed = 0.1f;
	public float maxAccel= 2f;
	public float minAccel = -2f;
	public float maxColorChange = 0.1f;

	public float accel = 0;
	public float rotation = 0;
	protected float red = 0;
	protected float green = 0.4f;
	protected float blue = 0;

	public abstract float Red { get; }
	
	public abstract float Green {get;}
	public abstract float Blue {get;}

	//private BoxCollider2D boxCollider;
	private Rigidbody2D rb2D;

	// Use this for initialization
	protected virtual void Start () {
		//boxCollider = GetComponent<BoxCollider2D>();
		rb2D = GetComponent<Rigidbody2D>();
	}

	protected void Rotate () {
		rb2D.MoveRotation(rb2D.rotation - rotation*turnSpeed);
	}

	protected void Move () {
		Vector3 start = transform.position;
		Vector3 move = new Vector3 (- Utils.CosDegToFloat(transform.rotation.eulerAngles.z), Utils.SinDegToToFloat(transform.rotation.eulerAngles.z), transform.position.z);
		Vector3 end = start + move * accel;
		
		Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, moveSpeed);
		rb2D.MovePosition(newPosition);

	}

	

	private void CheckVars() {
		if (red > 1) {
			red = 1;
		} else if (red < 0) {
			red = 0;
		}
		if (green > 1) {
			green = 1;
		} else if (green < 0) {
			green = 0;
		}
		if (blue > 1) {
			blue = 1;
		} else if (blue < 0) {
			blue = 0;
		}
	}


	void Update () {
		InnerUpdate();
		
	}

	

	
	// Update is called once per frame
	void FixedUpdate () {
		
		Rotate ();
		
		Move();

		InnerFixedUpdate();


	}

	protected abstract void InnerUpdate ();
	protected abstract void InnerFixedUpdate ();
}
