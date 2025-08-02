using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using System;


public class Ball : MonoBehaviour
{   
    InputData coor;
    PhysicsTrajectory pitch;
    [SerializeField] GameObject CanvasController;
    [SerializeField] GameObject PhysicsTrajectory;
    [SerializeField] GameObject Bat;
    public GameObject Baseball_Prefabs = null;
    public float x_Pos, y_Pos, z_Pos;
    public int flag = 0;
    public int ball_nums = 0;
    public GameObject ball_instantiate;

    void Start() {
        PhysicsTrajectory = GameObject.Find("TrajectorySimulator");
        CanvasController = GameObject.Find("Canvas");
        Bat = GameObject.Find("Bat");
        coor = FindObjectOfType<InputData>();
        pitch = FindObjectOfType<PhysicsTrajectory>();
    }

    void Update() {
        
    }

    private void OnCollisionEnter(Collision collision) {

        if (collision.gameObject.name == "Bat") {
            // CanvasController.GetComponent<CanvasController>().startSwinging = false;

            Vector3 direction = (transform.position - collision.contacts[0].point).normalized;

            Vector2 Vector_Positive = new Vector2(0f, 1f);
            Vector2 Vector_Negative = new Vector2(0f, -1f);
            Vector2 Current_Vector;
            if (Bat.transform.rotation.y < 0) Current_Vector = Vector_Negative;
            else Current_Vector = Vector_Positive;

            // Initial Position
            Instantiate(Baseball_Prefabs, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
            PhysicsTrajectory.GetComponent<PhysicsTrajectory>().x_Pos = transform.position.x;
            PhysicsTrajectory.GetComponent<PhysicsTrajectory>().y_Pos = transform.position.z;
            PhysicsTrajectory.GetComponent<PhysicsTrajectory>().z_Pos = transform.position.y;
            
            
            // Theta
            float theta = Vector3.Angle(new Vector2(direction.y, direction.z), Vector_Negative);
            print("--- Theta ---");
            print(new Vector2(direction.y, direction.z));
            print(theta);
            print("--- --- --- ---");
            if (direction.y < 0f) theta = -1f * theta;
            PhysicsTrajectory.GetComponent<PhysicsTrajectory>().theta = theta;

            // Phi
            float phi = Vector3.Angle(new Vector2(direction.x, direction.z), Current_Vector);
            print(phi);
            PhysicsTrajectory.GetComponent<PhysicsTrajectory>().phi = phi;

            //Thread.Sleep(100);

            PhysicsTrajectory.GetComponent<PhysicsTrajectory>().Init();
            PhysicsTrajectory.GetComponent<PhysicsTrajectory>().startDrawing = true;

            Destroy(gameObject);
        }

    }

    
    
}
