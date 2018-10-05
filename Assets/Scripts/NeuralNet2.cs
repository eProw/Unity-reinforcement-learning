using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class NeuralNet2 : MonoBehaviour {

    public LayerMask whatToHit;

    [HideInInspector]
    public Perceptron p;

    int[] neuronsPerLayer;
    float[] inputs;
    public Transform left, right;


    Vector2 directionLeft;
    Vector2 directionRight;

    RaycastHit2D hitLeft;
    RaycastHit2D hitRight;

    Vector2 velC = Vector2.zero;
    public float rotC = 0;
    bool back = false;
    public bool control = true;

    public bool randomNet = true;

    void Start()
    {
        if (randomNet)
        {
            neuronsPerLayer = new int[] { 4, 4, 2 };
            inputs = new float[2];
            p = new Perceptron(neuronsPerLayer, inputs);
        }
    }

    void Update()
    {
        directionLeft = left.up - left.right / 8;
        directionRight = right.up + right.right / 8;

        hitLeft = Physics2D.Raycast(left.position, directionLeft, 10, whatToHit);
        hitRight = Physics2D.Raycast(right.transform.position, directionRight, 10, whatToHit);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            gameObject.GetComponent<Rigidbody2D>().inertia = 0;
            if (control)
            {
                control = false;
            }else
            {
                control = true;
            }
        }
        if (control)
        {
            gameObject.GetComponent<Rigidbody2D>().inertia = 0;
            if (Input.GetKey(KeyCode.W))
            {
                velC = transform.up * 6;
                back = false;
            }
            else

       if (Input.GetKey(KeyCode.S))
            {
                velC = transform.up * -6;
                back = true;
            }
            else
            {
                velC = Vector2.zero;
            }

            /////////////////

            if (Input.GetKey(KeyCode.A))
            {
                rotC = back ? -1.5f : 1.5f;
            }
            else
            if (Input.GetKey(KeyCode.D))
            {
                rotC = back ? 1.5f : -1.5f;
            }
            else
            {
                rotC = 0;
            }

            gameObject.GetComponent<Rigidbody2D>().velocity = velC;
            if (velC != Vector2.zero)
                gameObject.GetComponent<Rigidbody2D>().rotation = gameObject.GetComponent<Rigidbody2D>().rotation + rotC;
        }
        else
        {
            velC = Vector2.zero;
            rotC = 0;
            inputs = new float[] { hitLeft.distance, hitRight.distance };
            p.UpdateInputs(inputs);

            for (int i = 0; i < inputs.Length; i++)
            {
                if (inputs[i] == 0) inputs[i] = 10;
            }

            float[] outputs = p.Calculate();

            float vel = Mathf.Clamp((outputs[0] * 2 - 1) * 10, -20, 20);
            float rot = Mathf.Clamp((outputs[1] * 2 - 1) * 10, -10, 10);


            gameObject.GetComponent<Rigidbody2D>().velocity = transform.up * vel;

            gameObject.GetComponent<Rigidbody2D>().rotation = gameObject.GetComponent<Rigidbody2D>().rotation + rot;

            //Debug.Log(string.Format("VEL:{0},ROT:{1}||{2},{3}||{4}", vel,rot, hitFront.distance, hitLeft.distance, Random.seed));
        }
        if (Input.GetMouseButtonDown(0))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(left.position, hitLeft.collider != null ? hitLeft.point : (Vector2)transform.position);
        Gizmos.DrawLine(right.position, hitRight.collider != null ? hitRight.point : (Vector2)transform.position);

        Gizmos.DrawWireSphere(hitLeft.collider != null ? hitLeft.point : (Vector2)transform.position, .2f);
        Gizmos.DrawWireSphere(hitRight.collider != null ? hitRight.point : (Vector2)transform.position, .2f);
    }

    void DestroyObj()
    {
        gameObject.SetActive(false);
    }

    public void ForceInit()
    {
        neuronsPerLayer = new int[] { 4, 4, 2 };
        inputs = new float[2];
        p = new Perceptron(neuronsPerLayer, inputs);
    }
}
