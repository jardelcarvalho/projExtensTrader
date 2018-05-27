using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ajusta : MonoBehaviour {

	[SerializeField] private GameObject palito;
	// Use this for initialization

	public bool Ajuste(float pX0, float px, string str) {
		if(palito.transform.position.x >= pX0 && palito.transform.position.x <= px
			&& (transform.position.x <= pX0 || transform.position.x >= px)) {
			Debug.Log("Call");
			float lP = palito.transform.position.x;
			GetComponent<UnityEngine.UI.Text>().text = str;
			float p = 0f;
			if(transform.position.x <= pX0) {
				p = transform.position.x + (transform.position.x - pX0);
			} else {
				p = transform.position.x - (px - transform.position.x);
			}
			transform.position = new Vector3(
				p,
				transform.position.y,
				transform.position.z
			);
			palito.transform.position = new Vector3(
				lP,
				palito.transform.position.y,
				palito.transform.position.z
			);
			return true;
		}
		return false;
	}
}
