using UnityEngine;
using System.Collections;

public class camFollow : MonoBehaviour {

    public Transform target;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        GameObject obj = GameObject.FindGameObjectWithTag("Player");
        if (obj != null)
        {
            target = obj.GetComponent<Transform>();
            transform.position = Vector3.Lerp(transform.position, new Vector3(target.position.x, target.position.y, -10), Time.deltaTime * 5);
        }
    }
}
