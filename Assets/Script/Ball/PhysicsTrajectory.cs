using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
public class PhysicsTrajectory : MonoBehaviour
{
    public UIControl UIControl;
    public GameObject statistics;
    public bool DrawTrajectory = false;
    public TextMeshProUGUI textMeshPro;
    public Material lineMaterial;
    List<LineRenderer>lines = new List<LineRenderer>();
    public GameObject Ball_point_blue;
    public GameObject Ball_point_red;
    public GameObject Ball_point_green;
    public GameObject Ball_point_yellow;
    public GameObject ball2D;
    Vector3 Point_position;
    int cnt = 0;
    int space = 500;    // shoot one ball every 700 frams
    public GameObject Baseball_Prefabs = null;
    public Transform pitchPoint;
    public GameObject flyball;
    Vector3 goodball;
    public float x_Pos, y_Pos = 0.7f, z_Pos;
    float x_Velocity, y_Velocity, z_Velocity;
    public float v0 = 10f; // mph
    public float theta;
    public float phi;
    public bool startDrawing = false;
    public int flag = 0;
    public int tablesize;
    public TextAsset textAssetData;
    [System.Serializable]
    public class Ball{
        public string batted_ball_type;
        public float x;
        public float y;
        public float launch_speed;
        public float launch_angle;
        public float launch_azimuth;
        public float cartesian_x;
        public float cartesian_y;
        public Vector3 batPoint;
        public Vector3 pitch_dist;
        public Vector3 landing_point;
        public GameObject flyball;
        public List<Vector3> pos;
        public int ball_exist;
        public bool isBat;
    }
    [System.Serializable]
    public class BallList{
        public Ball[] ball;
    }
    public BallList myPlayerList = new BallList();
    Vector3[] center = {new Vector3(-0.125f, 0.0f, 3.767f), new Vector3(-0.125f, 0.0f, -15.22f) };
    float[] radius = {99.0f, 29.187f};
    float[] angle = {90.0f, 144.0f};
    Vector3[] direction = {new Vector3(0.0f, 0.0f, -1.0f), new Vector3(0.0f, 0.0f, -1.0f)};
    int numberOfSectors = 6;
    public int[] landing = new int[6];
    public float infield = 0, outfield = 0, FOULBALL = 0;
    public float land = 0;
    public float launchangle;
    public int[] angleCounters = new int[9];
    public int camswitch = 0;
    public bool isswitch = false;
    public void Init() {

    }

    void Start() {
        statistics.gameObject.SetActive(true);
        ReadCSV();
        UIControl.HideDisplay();
    }

    void Update() {
        Analyze_ball_path();
    }

    public void ReadCSV(){
        string[] data = textAssetData.text.Split(new string[] {",","\n"}, System.StringSplitOptions.None);
        tablesize = data.Length/16-1;
        myPlayerList.ball = new Ball[tablesize];
        int tag = 0;
        for(int i = 0; i <= tablesize; i++){
            myPlayerList.ball[i] = new Ball();
            myPlayerList.ball[i].batted_ball_type = data[16 * (i + 1) + 7 + tag];
            myPlayerList.ball[i].x = float.Parse(data[16 * (i + 1) + 10 + tag]) * 0.3048f;
            myPlayerList.ball[i].y = float.Parse(data[16 * (i + 1) + 11 + tag]) * 0.3048f;
            myPlayerList.ball[i].launch_speed = float.Parse(data[16 * (i + 1) + 12 + tag]);
            myPlayerList.ball[i].launch_angle = float.Parse(data[16 * (i + 1) + 13 + tag]);
            myPlayerList.ball[i].launch_azimuth = float.Parse(data[16 * (i + 1) + 14 + tag]);
            myPlayerList.ball[i].cartesian_x = float.Parse(data[16 * (i + 1) + 15 + tag]) * 0.3048f;
            myPlayerList.ball[i].cartesian_y = float.Parse(data[16 * (i + 1) + 16 + tag]) * 0.3048f;
            
            myPlayerList.ball[i].batPoint = new Vector3(myPlayerList.ball[i].x, myPlayerList.ball[i].y, 3.1f);
            myPlayerList.ball[i].pitch_dist = myPlayerList.ball[i].batPoint-pitchPoint.position;
            myPlayerList.ball[i].flyball = null;
            myPlayerList.ball[i].isBat = false;
            myPlayerList.ball[i].pos = new List<Vector3>();
            myPlayerList.ball[i].ball_exist = 0;
            tag++;
        }
        
    }

    private void Analyze_ball_path()
    {
        if (flag == 0)
        {
            myPlayerList.ball[flag].ball_exist = 1;
            pitch_the_ball(myPlayerList.ball[flag]);
            camswitch = 0;
            flag++;
        }

        else if ( (myPlayerList.ball[flag-1].ball_exist == -2) ||  ( !isswitch && ( cnt > space )))
        {
            pitch_the_ball(myPlayerList.ball[flag]);
            camswitch = 0;
            myPlayerList.ball[flag].ball_exist = 1;
            flag++;
            cnt = 0;
        }
        else cnt++;

        for (int i = 0; i<tablesize; i++)
        {
            if (myPlayerList.ball[i].flyball == null) continue;

            Vector3 currentPosition = myPlayerList.ball[i].flyball.transform.position;
            if (myPlayerList.ball[i].ball_exist == 1)   // pitch
            {  
                if (arrived(myPlayerList.ball[i]))
                {
                    bat_the_ball(myPlayerList.ball[i]);
                    camswitch = 1;
                    myPlayerList.ball[i].ball_exist = -1;
                    show_AzimuthText(i);
                }
            }
            else if (myPlayerList.ball[i].ball_exist == -1)    // bat
            {
                if (currentPosition.y < 0)
                {
                    
                    myPlayerList.ball[i].landing_point = myPlayerList.ball[i].flyball.transform.position;
                    Point_position.x = myPlayerList.ball[i].landing_point.x;
                    Point_position.y = 0.1f;
                    Point_position.z = myPlayerList.ball[i].landing_point.z;
                    batted_ball_type(i);
                    Point_position.y = 0.0f;
                    LandingSector(Point_position);
                    Destroy(myPlayerList.ball[i].flyball);
                    myPlayerList.ball[i].ball_exist = -2;
                    myPlayerList.ball[i].flyball = null;
                    UIControl.HideDisplay();

                }
            }
            if (DrawTrajectory){
                LineRenderer line = lines[i];
                line.positionCount++;
                line.SetPosition(line.positionCount - 1, currentPosition);
            }
        }
    }

    private void pitch_the_ball(Ball ball)
    {  
        ball.flyball = Instantiate(Baseball_Prefabs, pitchPoint.position, pitchPoint.rotation);
        ball.flyball.GetComponent<Rigidbody>().velocity = ((ball.pitch_dist));
        GameObject lineObject = new GameObject("TrajectoryLine");
        LineRenderer line = lineObject.AddComponent<LineRenderer>();
        line.material = lineMaterial;
        line.startWidth = 0.1f;
        line.endWidth = 0.1f;

        if (ball.batted_ball_type == "fly_ball"){
            line.startColor = Color.blue;
            line.endColor = Color.blue;
        }
        else if (ball.batted_ball_type == "line_drive"){
            line.startColor = Color.red;
            line.endColor = Color.red;
        }
        else if (ball.batted_ball_type == "pop_up") {
            line.startColor = Color.yellow;
            line.endColor = Color.yellow;  
        }
        else if (ball.batted_ball_type == "ground_ball") {
            line.startColor = Color.green;
            line.endColor = Color.green;  
        }
        line.useWorldSpace = true;
        line.positionCount = 1;
        line.SetPosition(line.positionCount - 1, pitchPoint.position);
        lines.Add(line);
    }

    private bool arrived(Ball ball)
    {
        float real_dist = (ball.flyball.transform.position-pitchPoint.position).magnitude;
        float dist = (ball.pitch_dist).magnitude;
        if (real_dist>=dist)
            return true;
        else
            return false;
    }

    private void bat_the_ball(Ball ball)
    {
        Destroy(ball.flyball);
        ball.flyball = null;
        v0 = ball.launch_speed;
        /*
        theta = ball.launch_angle-90.0f;
        phi = ball.launch_azimuth-90.0f;*/
        launchangle = ball.launch_angle;
        angle_compare(launchangle);/*
        y_Velocity = v0 * 0.447f * Mathf.Cos(theta * Mathf.PI / 180f) * Mathf.Cos(phi * Mathf.PI / 180f);
        x_Velocity = v0 * 0.447f * Mathf.Cos(theta * Mathf.PI / 180f) * Mathf.Sin(phi * Mathf.PI / 180f);
        z_Velocity = v0 * 0.447f * Mathf.Sin(theta * Mathf.PI / 180f);*/
        float thetaRad = ball.launch_angle * Mathf.PI / 180f;
        float phiRad = (180 + ball.launch_azimuth) * Mathf.PI / 180f;
        float y_Velocity = v0 * 0.447f * Mathf.Sin(thetaRad);
        float x_Velocity = v0 * 0.447f * Mathf.Cos(thetaRad) * Mathf.Cos(phiRad);
        float z_Velocity = v0 * 0.447f * Mathf.Cos(thetaRad) * Mathf.Sin(phiRad);

        ball.flyball = Instantiate(flyball, ball.batPoint, Quaternion.identity);
        ball.flyball.GetComponent<Rigidbody>().velocity = new Vector3(x_Velocity, y_Velocity, z_Velocity);
        print(new Vector3(x_Velocity, y_Velocity, z_Velocity));
        goodball.x = ball.batPoint.x;
        goodball.y = ball.batPoint.y;
        goodball.z = 3.2f;
        GameObject ball2d = Instantiate(ball2D, goodball, Quaternion.identity);
        
    }

    private void batted_ball_type(int i)
    {
        if (myPlayerList.ball[i].batted_ball_type == "fly_ball"){
            GameObject point = Instantiate(Ball_point_blue, Point_position, Ball_point_blue.transform.rotation);
        }
        else if (myPlayerList.ball[i].batted_ball_type == "line_drive"){
            GameObject point = Instantiate(Ball_point_red, Point_position, Ball_point_red.transform.rotation);
        }
        else if (myPlayerList.ball[i].batted_ball_type == "pop_up"){
            GameObject point = Instantiate(Ball_point_yellow, Point_position, Ball_point_green.transform.rotation);
        }    
        else if (myPlayerList.ball[i].batted_ball_type == "ground_ball"){
            GameObject point = Instantiate(Ball_point_green, Point_position, Ball_point_green.transform.rotation);
        }     
    }

    private void angle_compare(float angle)
    {
        int index = Mathf.FloorToInt(angle / 10);
        if (index >= 0 && index < angleCounters.Length)
        {
            angleCounters[index]++;
        }
    }

    private void LandingSector(Vector3 Point_position)
    {
        if (IsPointInSector(Point_position, 0)){
            if (IsPointInSector(Point_position, 1) || (Point_position - center[0]).magnitude <= 40.0f)
                infield++;
            else
                outfield++;

            int sectorIndex = GetSectorIndex(Point_position);
            if (sectorIndex >= 0 && sectorIndex <= 5){
                landing[sectorIndex]++;
            }
            land++;
        }
        else{
            FOULBALL++;
            land++;
        }
    }
    public bool IsPointInSector(Vector3 point, int sectorIndex) //判斷內外野、出界
    {
        Vector3 dirToPoint = point - center[sectorIndex];
        if (dirToPoint.magnitude > radius[sectorIndex])
            return false;

        float angleToPoint = Vector3.Angle(direction[sectorIndex], dirToPoint);
        return angleToPoint <= angle[sectorIndex] / 2;
    }
    
    public int GetSectorIndex(Vector3 point) //落在哪個小善行
    {
        Vector3 toPoint = point - center[0];
        float angleToPoint = Vector3.SignedAngle(direction[0], toPoint, Vector3.up);
        float sectorAngle = angle[0] / numberOfSectors;
        float startAngleOffset = -angle[0] / 2;

        for (int i = 0; i < numberOfSectors; i++)
        {
            float sectorStartAngle = startAngleOffset + i * sectorAngle;
            float sectorEndAngle = sectorStartAngle + sectorAngle;

            if (angleToPoint >= sectorStartAngle && angleToPoint < sectorEndAngle)
            {
                return i;
            }
        }
        return -1;
    }
    private void show_AzimuthText(int idx)
    {
        string exit_vel = "Exit Velocity(m/s): " + (myPlayerList.ball[idx].launch_speed*0.447f).ToString("F1");
        string angle = "Angle(deg): " + myPlayerList.ball[idx].launch_angle.ToString();
        string azimuth = "Azimuth(deg): " + myPlayerList.ball[idx].launch_azimuth.ToString();
        string message = exit_vel + "\n" + angle + "\n" + azimuth;
        UIControl.OnBallHit(message);
        textMeshPro.text = "Bat Information" + "\n" + "\n" + message;
    }
}
