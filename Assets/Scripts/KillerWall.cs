using UnityEngine;
using System.Collections;

public class KillerWall : MonoBehaviour {

	void OnTriggerEnter2D (Collider2D other) {
		if (other.gameObject.tag == "Entity") {
			GameManager.instance.onCreatureDie(other.gameObject);
		}
	}
	
}
