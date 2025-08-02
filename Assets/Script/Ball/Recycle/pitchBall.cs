using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class pitchBall : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Transform t = GetComponent<Transform>();
        // v: velocity
        // w: angular velocity
        v0 = 95 * 0.44704f;
        V1 = 95;
        w  = 1800 * 2 * Mathf.PI / 60 ;
        W1 = 1800;
        theta = 1;
        ol = 225;
        movingvector.x = v0 * Mathf.Cos(theta * Mathf.PI / 180);
        movingvector.y = v0 * Mathf.Sin(theta * Mathf.PI / 180);
        movingvector.z = 0;
        v = Mathf.Sqrt(movingvector.x * movingvector.x + movingvector.y * movingvector.y + movingvector.z * movingvector.z);
        ScoreText.text = "V: " + V1 ;
        ScoreText2.text = "W: " + W1;
    }
	
	// Update is called once per frame
	void Update () {
        gameObject.transform.Rotate(new Vector3(0f, w*Mathf.Cos(ol), w*Mathf.Sin(ol)));
        transform.localPosition += Time.deltaTime * movingvector;
        movingvector.x = movingvector.x + (-1 * func(v) * v * movingvector.x + B * w * (movingvector.y * Mathf.Sin(ol * Mathf.PI / 180.0f) - movingvector.z * Mathf.Cos(ol * Mathf.PI / 180.0f))) * Time.deltaTime;
        movingvector.y = movingvector.y + (-1 * func(v) * v * movingvector.y - B * w * (movingvector.x * Mathf.Sin(ol * Mathf.PI / 180.0f)) - 9.8f) * Time.deltaTime;
        movingvector.z = movingvector.z + (-1 * func(v) * v * movingvector.z + B * w * (movingvector.x * Mathf.Cos(ol * Mathf.PI / 180.0f))) * Time.deltaTime;
        v = Mathf.Sqrt(movingvector.x * movingvector.x + movingvector.y * movingvector.y + movingvector.z * movingvector.z);
        if (Input.GetKey(KeyCode.W))
        {
            v0 += 0.5f* 0.44704f;
            V1 += 0.5f;
            AddV();
        }
        if (Input.GetKey(KeyCode.S))
        {
            v0 -= 0.5f * 0.44704f;
            V1 -= 0.5f;
            AddV();
        }
        if (Input.GetKey(KeyCode.D))
        {
            w += 10 * 2 * Mathf.PI / 60;
            W1 += 10;
            AddW();
        }
        if (Input.GetKey(KeyCode.A))
        {
            w -= 10 * 2 * Mathf.PI / 60;
            W1 -= 10;
            AddW();
        }
        while (transform.localPosition.x > 18.44)
        {
            transform.localPosition = new Vector3(0f,1.7f,0f);
            movingvector.x = v0 * Mathf.Cos(theta * Mathf.PI / 180);
            movingvector.y = v0 * Mathf.Sin(theta * Mathf.PI / 180);
            movingvector.z = 0;
            v = Mathf.Sqrt(movingvector.x * movingvector.x + movingvector.y * movingvector.y + movingvector.z * movingvector.z);
        }
    }
    public void AddV()
    {
        ScoreText.text = "V: " + V1 ;
    }
    public void AddW()
    {
        ScoreText2.text = "W: " + W1;
    }

    float func(float v) {
        return (0.0039f + 0.0058f / (1.0f + Mathf.Exp(v - 35.0f) / 5.0f));
    }
    public Vector3 movingvector;
    public float v0, V1, w, W1, theta, ol;
    public float v, B;
    public Text ScoreText;
    public Text ScoreText2;
}
