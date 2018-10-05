using UnityEngine;
using System.Collections;

public class BackPropagation : MonoBehaviour {

    NeuralNet2 neuralNet;
    float[] outputs;

	// Use this for initialization
	void Start () {
        neuralNet = GetComponent<NeuralNet2>();
        outputs = new float[2];
	}
	
	// Update is called once per frame
	void Update () {
        if (neuralNet.control)
        {
            outputs[0] = ((gameObject.GetComponent<Rigidbody2D>().velocity.magnitude / transform.up.magnitude)+10)/20;
            outputs[1] = (neuralNet.rotC+10)/20;
            //Debug.Log(string.Format("{0},{1}",outputs[0],outputs[1]));

        }
	}
}
