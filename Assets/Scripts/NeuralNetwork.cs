using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class NeuralNetwork : MonoBehaviour {

    public LayerMask whatToHit;

    [HideInInspector]
    public Perceptron p;

    int[] neuronsPerLayer;
    float[] inputs;

    Vector2 directionFront;
    Vector2 directionLeft;
    Vector2 directionRight;
    Vector2 directionL;
    Vector2 directionR;

    RaycastHit2D hitFront;
    RaycastHit2D hitLeft;
    RaycastHit2D hitRight;
    RaycastHit2D hitL;
    RaycastHit2D hitR;

    public bool randomNet = true;

    void Start() {
        if (randomNet) {
            neuronsPerLayer = new int[] { 4, 5, 4, 2 };
            inputs = new float[6];
            p = new Perceptron(neuronsPerLayer, inputs);
        }
    }

	void Update () {
        directionFront = transform.up;
        directionLeft = transform.up - transform.right;
        directionRight = transform.up + transform.right;
        directionL = - transform.right;
        directionR = transform.right;

        hitFront = Physics2D.Raycast(gameObject.transform.position, directionFront,10,whatToHit);
        hitLeft = Physics2D.Raycast(gameObject.transform.position, directionLeft,10,whatToHit);
        hitRight = Physics2D.Raycast(gameObject.transform.position, directionRight,10, whatToHit);

        hitL = Physics2D.Raycast(gameObject.transform.position, directionL, 10, whatToHit);
        hitR = Physics2D.Raycast(gameObject.transform.position, directionR, 10, whatToHit);

        inputs = new float[] {hitFront.distance,hitLeft.distance, hitRight.distance, hitR.distance, hitL.distance, gameObject.GetComponent<Rigidbody2D>().velocity.sqrMagnitude};
        p.UpdateInputs(inputs);

        for(int i = 0; i < inputs.Length;i++)
        {
            if (inputs[i] == 0) inputs[i] = 10;
        }

        float[] outputs = p.Calculate();

        float vel = Mathf.Clamp((outputs[0] * 2 - 1) * 10,-20,20);
        float rot = Mathf.Clamp((outputs[1] * 2 - 1) * 10, -10, 10);


        gameObject.GetComponent<Rigidbody2D>().velocity = transform.up*vel;

        gameObject.GetComponent<Rigidbody2D>().rotation = gameObject.GetComponent<Rigidbody2D>().rotation+rot;

        //Debug.Log(string.Format("VEL:{0},ROT:{1}||{2},{3}||{4}", vel,rot, hitFront.distance, hitLeft.distance, Random.seed));

        if (Input.GetMouseButtonDown(0))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag != "Respawn" && other.tag != "Player")
            gameObject.SetActive(false);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Respawn")
            Invoke("DestroyObj",5);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Respawn")
            CancelInvoke("DestroyObj");
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, hitFront.collider!= null?hitFront.point:(Vector2)transform.position);
        Gizmos.DrawLine(transform.position, hitLeft.collider != null ? hitLeft.point : (Vector2)transform.position);
        Gizmos.DrawLine(transform.position, hitRight.collider != null ? hitRight.point : (Vector2)transform.position);
        Gizmos.DrawLine(transform.position, hitL.collider != null ? hitL.point : (Vector2)transform.position);
        Gizmos.DrawLine(transform.position, hitR.collider != null ? hitR.point : (Vector2)transform.position);

        Gizmos.DrawWireSphere(hitFront.collider != null ? hitFront.point : (Vector2)transform.position, .2f);
        Gizmos.DrawWireSphere(hitLeft.collider != null ? hitLeft.point : (Vector2)transform.position, .2f);
        Gizmos.DrawWireSphere(hitRight.collider != null ? hitRight.point : (Vector2)transform.position, .2f);
        Gizmos.DrawWireSphere(hitR.collider != null ? hitR.point : (Vector2)transform.position, .2f);
        Gizmos.DrawWireSphere(hitL.collider != null ? hitL.point : (Vector2)transform.position, .2f);
    }

    void DestroyObj()
    {
        gameObject.SetActive(false);
    }

    public void ForceInit()
    {
        neuronsPerLayer = new int[] { 4, 5, 4, 2 };
        inputs = new float[6];
        p = new Perceptron(neuronsPerLayer, inputs);
    }
}
