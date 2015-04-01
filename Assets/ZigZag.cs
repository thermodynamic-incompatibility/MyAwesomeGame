using UnityEngine;
using System.Collections;

public class ZigZag : MonoBehaviour {

	public float rotSpeed;

	// Use this for initialization
	void Start () 
	{

	}

	void Update () 
	{
		Debug.Log ("The current total rotation is " + rotSpeed * Time.time);
		transform.rotation = Quaternion.Euler(90f, rotSpeed * Time.time, 0f);
	}
}
