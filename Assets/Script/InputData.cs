using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class InputData : MonoBehaviour
{
    PhysicsTrajectory bal;
    public int ball_nums = 0;
    public int tablesize;
    public TextAsset textAssetData;
    [System.Serializable]
    public class Ball{
        public string name;
        public string game_date;
        public string stadium_name;
        public string batter_name_first;
        public string batter_name_last;
        public string batter_id;
        public string pitch_type;
        public string batted_ball_type;
        public string zone;
        public string stand;
        public float x;
        public float y;
        public float z;
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
        public int index;
        public bool isBat;
    }
    [System.Serializable]
    public class BallList{
        public Ball[] ball;
    }
    public BallList myPlayerList = new BallList();
    //float timePeriod = 0.02f;
    //float time = 0.0f;
    //float passedTime = 0.0f;
    private int flag = 0;
    
    void Start()
    {
        ReadCSV();
        bal = FindObjectOfType<PhysicsTrajectory>();
    }
    public void Init()
    {
        // Init time parameters
        //timePeriod = 0.02f;
        //time = 0.0f;
        //passedTime = 0.0f;
    }
    public void ReadCSV(){
        string[] data = textAssetData.text.Split(new string[] {",","\n"}, System.StringSplitOptions.None);
        tablesize = data.Length/16-1;
        myPlayerList.ball = new Ball[tablesize];
        int tag = 0;
        for(int i = 0; i <= tablesize; i++){
            myPlayerList.ball[i] = new Ball();
            myPlayerList.ball[i].name = data[16 * (i + 1) + tag];
            myPlayerList.ball[i].game_date = data[16 * (i + 1) + 1 + tag];
            myPlayerList.ball[i].stadium_name = data[16 * (i + 1) + 2] + tag;
            myPlayerList.ball[i].batter_name_first = data[16 * (i + 1) + 3 + tag];
            myPlayerList.ball[i].batter_name_last = data[16 * (i + 1) + 4 + tag];
            myPlayerList.ball[i].batter_id = data[16 * (i + 1) + 5 + tag];
            myPlayerList.ball[i].pitch_type = data[16 * (i + 1) + 6 + tag];
            myPlayerList.ball[i].batted_ball_type = data[16 * (i + 1) + 7 + tag];
            myPlayerList.ball[i].zone = data[16 * (i + 1) + 8 + tag];
            myPlayerList.ball[i].stand = data[16 * (i + 1) + 9 + tag];
            myPlayerList.ball[i].x = float.Parse(data[16 * (i + 1) + 10 + tag]) * 0.3048f;
            myPlayerList.ball[i].y = float.Parse(data[16 * (i + 1) + 11 + tag]) * 0.3048f;
            myPlayerList.ball[i].launch_speed = float.Parse(data[16 * (i + 1) + 12 + tag]);
            myPlayerList.ball[i].launch_angle = float.Parse(data[16 * (i + 1) + 13 + tag]);
            myPlayerList.ball[i].launch_azimuth = float.Parse(data[16 * (i + 1) + 14 + tag]);
            myPlayerList.ball[i].cartesian_x = float.Parse(data[16 * (i + 1) + 15 + tag]) * 0.3048f;
            myPlayerList.ball[i].cartesian_y = float.Parse(data[16 * (i + 1) + 16 + tag]) * 0.3048f;
            
            myPlayerList.ball[i].batPoint = new Vector3(myPlayerList.ball[i].x, myPlayerList.ball[i].y, 3.1f);
            myPlayerList.ball[i].pitch_dist = myPlayerList.ball[i].batPoint-bal.pitchPoint.position;
            myPlayerList.ball[i].flyball = null;
            myPlayerList.ball[i].isBat = false;

            myPlayerList.ball[i].pos = new List<Vector3>();
            myPlayerList.ball[i].ball_exist = 0;
            myPlayerList.ball[i].index = i;
            tag++;
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        /*if (startDrawing) {
            time += Time.deltaTime;
            flag++;
            
            time = 0.0f;
            passedTime += 0.02f;
            x_Pos = myPlayerList.ball[flag].x;
            y_Pos = myPlayerList.ball[flag].y;
            z_Pos = myPlayerList.ball[flag].z;
            Vector3 Coordinate = new Vector3(x_Pos, y_Pos, z_Pos);
            Debug.Log(Coordinate);

            // Original coordinate transformation
                
            //GameObject ball_instantiate = Instantiate(Baseball_Prefabs, new Vector3(x_Pos, y_Pos, z_Pos), Quaternion.identity);                
            
            //if (passedTime > 5.0f) {
                //startDrawing = false;
                //Init();
            //}
    
        }*/
    }
}
