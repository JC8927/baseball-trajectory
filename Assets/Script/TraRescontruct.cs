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

    [System.Serializable]
    public class Coor
    {
        public float x, y, z;
        public float dis, theta, phi, speed;
        public float vx, vy, vz;
        public float dirx, diry, dirz;
    }

    [System.Serializable]
    public class CoorList
    {
        public List<Coor> coor = new List<Coor>();
    }

    public CoorList[] coorLists;
    List<LineRenderer> lines = new List<LineRenderer>();
    public GameObject Baseball_Prefabs;
    GameObject[] predict_ball;
    LineRenderer[] predict_lines;
    int[] letsgo;
    public int[] StopAt;
    int currentIndex = -1;

    void Start()
    {
        if (video) videoplayer.gameObject.SetActive(true);

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
                Trajectory_Reconstruct(coorLists[i], colors[i % colors.Length], i);
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

                currentIndex = (currentIndex + 1) % textAssets.Length;

                coorLists[currentIndex] = new CoorList();
                ReadCoorCSV(textAssets[currentIndex], coorLists[currentIndex]);
                Trajectory_Reconstruct(coorLists[currentIndex], colors[currentIndex % colors.Length], currentIndex);
            }


            if (Input.GetKeyDown("a"))
            {
                ClearPreviousTrajectory();
                for (int i = 0; i < textAssets.Length; i++)
                {
                    coorLists[i] = new CoorList();
                    ReadCoorCSV(textAssets[i], coorLists[i]);
                    Trajectory_Reconstruct(coorLists[i], colors[i % colors.Length], i);
                }
                currentIndex = textAssets.Length - 1;
            }
            if (Input.GetKeyDown("c"))
            {
                ClearPreviousTrajectory();
            }

        }
    }

    void ReadCoorCSV(TextAsset textAssetData, CoorList coorList)
    {
        string[] data = textAssetData.text.Split(new string[] { ",", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        int numRows = data.Length / 3;

        for (int i = 1; i < numRows; i++)
        {
            try
            {
                float xx = float.Parse(data[3 * i]);
                if (xx != -1)
                {
                    Coor newCoor = new Coor
                    {
                        x = float.Parse(data[3 * i + 1]) * -1f,
                        y = float.Parse(data[3 * i + 2]),
                        z = xx + 3.8f
                    };
                    coorList.coor.Add(newCoor);
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"第 {i} 行解析失敗：{e.Message}");
            }
        }
    }

    void Trajectory_Reconstruct(CoorList coorList, Color color, int index)
    {
        GameObject lineObject = new GameObject("TrajectoryLine_" + index);
        LineRenderer newLine = lineObject.AddComponent<LineRenderer>();
        newLine.material = lineMaterial;
        newLine.startWidth = 0.08f;
        newLine.endWidth = 0.08f;
        newLine.useWorldSpace = true;
        newLine.startColor = color;
        newLine.endColor = color;

        StopAt[index] = coorList.coor.Count;

        for (int i = 0; i < coorList.coor.Count - 1; i++)
        {
            Vector3 v1 = new Vector3(coorList.coor[i].x, coorList.coor[i].y, coorList.coor[i].z);
            Vector3 v2 = new Vector3(coorList.coor[i + 1].x, coorList.coor[i + 1].y, coorList.coor[i + 1].z);
            Vector3 dir = v2 - v1;

            coorList.coor[i].dirx = dir.x;
            coorList.coor[i].diry = dir.y;
            coorList.coor[i].dirz = dir.z;

            if (dir.z < 0 && coorList.coor[i].z > -1 && StopAt[index] == coorList.coor.Count)
            {
                StopAt[index] = i;
                Debug.Log($"StopAt[{index}] = {StopAt[index]}");
            }
        }

        newLine.positionCount = coorList.coor.Count - StopAt[index];

        for (int i = 0; i < coorList.coor.Count; i++)
        {
            Vector3 pos = new Vector3(coorList.coor[i].x, coorList.coor[i].y, coorList.coor[i].z);
            if (i < StopAt[index])
            {
                GameObject ball = Instantiate(Baseball_Prefabs, pos, Quaternion.identity);
                Renderer renderer = ball.GetComponent<Renderer>();
                if (renderer != null)
                    renderer.material.color = color;
            }
            else
            {
                newLine.SetPosition(i - StopAt[index], pos);
            }
        }

        lines.Add(newLine);
        letsgo[index] = 0;
    }

    void ClearPreviousTrajectory()
    {
        // 清除所有 LineRenderer
        foreach (var line in lines)
        {
            if (line != null)
                Destroy(line.gameObject);
        }
        lines.Clear();

        // 清除所有球體
        GameObject[] balls = GameObject.FindGameObjectsWithTag("Baseball");
        foreach (var ball in balls)
        {
            Destroy(ball);
        }
    }
}
