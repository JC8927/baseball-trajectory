using System.Collections;    
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class TraRescontruct : MonoBehaviour
{
    public bool video;
    public bool test;
    public Material lineMaterial;
    public GameObject videoplayer;
    public bool startDrawing;
    public TextAsset[] textAssets;
    Color[] colors = { Color.blue, Color.green, Color.red, Color.yellow };
    int numRows;

    [System.Serializable]
    public class Coor
    {
        public float x;
        public float y;
        public float z;
        public float dis;
        public float theta;
        public float phi;
        public float speed;
        public float vx;
        public float vy;
        public float vz;
        public float dirx;
        public float diry;
        public float dirz;
    }

    [System.Serializable]
    public class CoorList
    {
        public List<Coor> coor = new List<Coor>();
    }
    public CoorList[] coorLists;
    List<LineRenderer> lines = new List<LineRenderer>();
    public GameObject Baseball_Prefabs;
    LineRenderer line;
    GameObject ball_instantiate;
    GameObject[] predict_ball;
    LineRenderer[] predict_lines;
    int[] letsgo;
    public int[] StopAt;
    float angleThreshold = 5.0f;
    int i = -1;

    void Start()
    {
        if (video)
        videoplayer.gameObject.SetActive(true);
        coorLists = new CoorList[textAssets.Length];
        StopAt = new int[textAssets.Length];
        predict_ball = new GameObject[textAssets.Length];
        letsgo = new int[textAssets.Length];
        predict_lines = new LineRenderer[textAssets.Length];
        
        if (!test)
        {
            for (int i = 0; i < textAssets.Length; i++)
            {
                coorLists[i] = new CoorList();
                ReadCoorCSV(textAssets[i], coorLists[i]);
                Trajectory_Reconstruct(coorLists[i], colors[i], i);
            }
        }
    }

    void Update() 
    {
        if (test)
        {
            if (Input.GetKeyDown("n"))
            {
                ClearPreviousTrajectory();
                i++;
                coorLists[i] = new CoorList();
                ReadCoorCSV(textAssets[i], coorLists[i]);
                Trajectory_Reconstruct(coorLists[i], colors[i], i);
                
            }

            if (Input.GetKeyDown("a"))
            {
                for (int i = 0; i < textAssets.Length; i++)
            {
                coorLists[i] = new CoorList();
                ReadCoorCSV(textAssets[i], coorLists[i]);
                Trajectory_Reconstruct(coorLists[i], colors[i], i);
            }
                
            }
        }
    }

    void ReadCoorCSV(TextAsset textAssetData, CoorList coorList)
    {
        string[] data = textAssetData.text.Split(new string[] { ",", "\n" }, System.StringSplitOptions.None);
        numRows = data.Length / 3 - 1;

        for (int i = 1; i < numRows; i++)
        {
            float xx = float.Parse(data[3 * i]);
            if (xx != -1)
            {
                Coor newCoor = new Coor
                {
                    x = float.Parse(data[3 * i + 1]) * -1f,
                    y = float.Parse(data[3 * i + 2]) * 1f,
                    z = xx * 1f + 3.8f
                    //z = x * 1f + 5.0f
                };
                coorList.coor.Add(newCoor);
            }
        }
    }
    void Trajectory_Reconstruct(CoorList coorList, Color color, int index)
    {
        GameObject lineObject = new GameObject("TrajectoryLine");
        LineRenderer newLine = lineObject.AddComponent<LineRenderer>();
        newLine.material = lineMaterial;
        newLine.startWidth = 0.08f;
        newLine.endWidth = 0.08f;
        newLine.useWorldSpace = true;
        newLine.startColor = color;
        newLine.endColor = color;

        StopAt[index] = coorList.coor.Count;

        for (int i = 0; i < coorList.coor.Count; i++)
        {
            Vector3 position = new Vector3(coorList.coor[i].x, coorList.coor[i].y, coorList.coor[i].z);

            if (i < coorList.coor.Count - 1)
            {
                Vector3 vector1 = position;
                Vector3 vector2 = new Vector3(coorList.coor[i + 1].x, coorList.coor[i + 1].y, coorList.coor[i + 1].z);
                Vector3 direction = vector2 - vector1;

                coorList.coor[i].dirx = direction.x;
                coorList.coor[i].diry = direction.y;
                coorList.coor[i].dirz = direction.z;

                if (coorList.coor[i].dirz < 0 && coorList.coor[i].z > -1 && StopAt[index] == coorList.coor.Count)
                {
                    StopAt[index] = i;
                    print("StopAt[" + index + "] = " + StopAt[index]);
                }
            }

            
            newLine.positionCount = coorList.coor.Count - StopAt[index];
            if (i < StopAt[index])
            {
                ball_instantiate = Instantiate(Baseball_Prefabs, position, Quaternion.identity);
                Renderer renderer = ball_instantiate.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = color;
                }
            }

            else
            {
                newLine.SetPosition(i - StopAt[index], position);
            }
        }

        lines.Add(newLine);
        letsgo[index] = 0;
    }
    void ClearPreviousTrajectory()
{
    // 刪除 LineRenderer
    if (lines.Count > 0)
    {
        Destroy(lines[i]);
    }

    // 刪除球物件
    GameObject[] existingBalls = GameObject.FindGameObjectsWithTag("Baseball");
    foreach (GameObject ball in existingBalls)
    {
        Destroy(ball);
    }
}
}
