using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class Creature : Entity {
	public GameObject[] colorDetectors;
	public GameObject goRend;

	private ColorDetector [] colDets;

	private SpriteRenderer rend;

	public override float Red {
		get {
			return rend.color.r;
		}
	}

	public override float Green {
		get {
			return rend.color.g;
		}
	}

	public override float Blue {
		get {
			return rend.color.b;
		}
	}
	

	protected override void Start() {
		base.Start();
		colDets = new ColorDetector [colorDetectors.Length];
		for (int i = 0; i < colorDetectors.Length; i++) {
			colDets [i] = colorDetectors[i].GetComponent<ColorDetector> ();
			//det.Self = GetComponent<Collider2D>();
		}
		red = Random.value;
		green = Random.value;
		blue = Random.value;
		accel = Random.value*2 - 1.0f;
		rotation = Random.value * 2 - 1.0f;
		rend = goRend.GetComponent<SpriteRenderer>();
		rend.color = new Color(red, green, blue);

	}
	

	protected override void InnerFixedUpdate() {
		if (GameManager.instance.getTerrHeight(transform.position) < 68.85f) {
			LoseHealth (DamageType.Drowning);
		}
	}

	private void LoseHealth(DamageType dt) {
		if (dt == DamageType.Drowning) {
			GameManager.instance.onCreatureDie(this.gameObject);
		}
	}

	protected override void InnerUpdate() {

		ChangeColor();
	}


	private void ChangeColor() {
		Color target = new Color(red, green, blue);
		float distance = Mathf.Abs(target.grayscale - rend.color.grayscale);
		rend.color = Color.Lerp(rend.color, target, maxColorChange / distance);
	}


}
