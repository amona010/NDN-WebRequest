﻿using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Linq;

public class NDNData : MonoBehaviour
{
    public string path = "C:/Users/Alex/Documents/GitHub/NDN WebRequest/Assets/Resources/newTest.json";
    public List<testbed_node> nodes;
    public GameObject node;
    public DataVisualizer visualizer;
    private string currentJSONNodes = "";
    private string nodeConnectionsJSON = "";
    private Dictionary<int, string> startAndFinishNodes = new Dictionary<int, string>();
    bool running = false; 

    void Start()
    {
        nodes = new List<testbed_node>();
        StartCoroutine(getJSONData(0));
    }

    private void Update()
    {
        if(!running)
        {
            StartCoroutine(ExecuteAfterTime(10));
        }
    }

    IEnumerator getJSONData(int action)
    {
        print("coroutine started");
        string propertyName = "";
        string tokenType = "";
        string nodeName = null;
        string shortName = null;
        string site = null;
        string https = null;
        bool backBone = false;
        bool wstls = false;
        string prefix = null;
        bool fchEnabled = false;
        bool ndnUp = false;
        float xVal = 0;
        float yVal = 0;
        UnityWebRequest www = UnityWebRequest.Get("http://ndndemo.arl.wustl.edu/testbed-nodes.json");
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            if (currentJSONNodes != www.downloadHandler.text)
            {
                currentJSONNodes = www.downloadHandler.text;

                JsonTextReader reader = new JsonTextReader(new StringReader(www.downloadHandler.text));

                while (reader.Read())
                {
                    if (reader.Value != null)
                    {
                        Debug.Log(reader.TokenType + " " + reader.Value);
                        if (reader.TokenType.ToString().Equals("PropertyName"))
                        {
                            Debug.Log("This is the name of a property");
                            propertyName = reader.Value.ToString();

                            if (propertyName.Equals("position"))
                            {
                                reader.Read();
                                Debug.Log(reader.TokenType);
                                reader.Read();
                                Debug.Log(reader.TokenType);
                                xVal = float.Parse(reader.Value.ToString());
                                reader.Read();
                                yVal = float.Parse(reader.Value.ToString());
                            }
                        }
                        else
                        {
                            switch (propertyName)
                            {
                                case "id":
                                    nodeName = (string)reader.Value;
                                    break;
                                case "shortname":
                                    shortName = (string)reader.Value;
                                    break;
                                case "site":
                                    site = (string)reader.Value;
                                    break;
                                case "https":
                                    https = (string)reader.Value;
                                    break;
                                case "backbone":
                                    backBone = (bool)reader.Value;
                                    break;
                                case "ws-tls":
                                    wstls = (bool)reader.Value;
                                    break;
                                case "prefix":
                                    prefix = (string)reader.Value;
                                    break;
                                case "fch-enabled":
                                    fchEnabled = (bool)reader.Value;
                                    break;
                                case "ndn-up":
                                    ndnUp = (bool)reader.Value;
                                    break;
                                default:
                                    Debug.Log("Error");
                                    break;

                            }
                        }
                    }
                    else
                    {
                        Debug.Log(reader.TokenType);

                        if (reader.TokenType.ToString().Equals("EndObject"))
                        {
                            if (tokenType.Equals("EndObject"))
                            {
                                Debug.Log("finished JSON");
                            }
                            else
                            {
                                tokenType = reader.TokenType.ToString();
                                Debug.Log(nodeName + " " + shortName + " " + site + " " + https + " " + xVal + " " + yVal + " " + backBone + " " + wstls + " " + fchEnabled + " " + ndnUp + " ");
                                nodes.Add(new testbed_node(nodeName, shortName, site, https, backBone, wstls, prefix, fchEnabled, ndnUp, xVal, yVal));
                            }
                        }
                        else
                        {
                            tokenType = reader.TokenType.ToString();
                        }
                    }
                }
                reader.Close();
                www.Dispose();

                if (action == 0)
                {
                    serializeNodes(nodes, 0);
                }
                else
                {
                    serializeNodes(nodes, 1);
                }
            }
        }

        www = UnityWebRequest.Get(/*"http://ndnmap.arl.wustl.edu/json/links/"*/"C:/Users/Alex/Downloads/download.JSON");
        yield return www.SendWebRequest();

        if(www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            int id = 0;
            string start = "";
            string end = "";

            if (nodeConnectionsJSON != www.downloadHandler.text)
            {
                nodeConnectionsJSON = www.downloadHandler.text;

                JsonTextReader reader = new JsonTextReader(new StringReader(www.downloadHandler.text));

                Debug.Log(nodeConnectionsJSON);

                while (reader.Read())
                {

                    if (reader.Value != null)
                    {
                        Debug.Log(reader.TokenType + " " + reader.Value);
                        if (reader.TokenType.ToString().Equals("PropertyName"))
                        {
                            Debug.Log("This is the name of a property");
                            propertyName = reader.Value.ToString();
                        }
                        else
                        {
                            switch (propertyName)
                            {
                                case "id":
                                    id = int.Parse(reader.Value.ToString());
                                    break;
                                case "start":
                                    start = (string)reader.Value;
                                    break;
                                case "end":
                                    end = (string)reader.Value;
                                    break;
                                default:
                                    Debug.Log("Error");
                                    break;
                            }
                        }
                    }
                    else
                    {
                        Debug.Log(reader.TokenType);

                        if (reader.TokenType.ToString().Equals("EndObject"))
                        {
                            if (tokenType.Equals("EndObject"))
                            {
                                Debug.Log("finished JSON");
                            }
                            else
                            {
                                startAndFinishNodes.Add(id, start + "," + end);
                            }
                        }
                        else
                        {
                            tokenType = reader.TokenType.ToString();
                        }
                    }
                }

                visualizer.createConnections(startAndFinishNodes);
            }
        }

        //var allKeys = startAndFinishNodes.Keys.ToArray();

        //foreach(int key in allKeys)
        //{
        //    string[] values = startAndFinishNodes[key].Split(',');

        //    GameObject startNode = GameObject.Find(values[0]);
        //    GameObject endNode = GameObject.Find(values[1]);

        //}

        www.Dispose();


        print("Coroutine Ended");
        yield break;
    }

    void createNodes(List<testbed_node> nodes)
    {
        foreach (testbed_node n in nodes)
        {
            float x = (0.5f * Mathf.Cos((n.xVal) * Mathf.Deg2Rad) * Mathf.Cos(n.yVal * Mathf.Deg2Rad)) * 100;
            float y = (0.5f * Mathf.Sin(n.yVal * Mathf.Deg2Rad)) * 100;
            float z = (0.5f * Mathf.Sin((n.xVal) * Mathf.Deg2Rad) * Mathf.Cos(n.yVal * Mathf.Deg2Rad)) * 100;
            Debug.Log(n.nodeName);
            GameObject newNode = Instantiate(node);
            newNode.GetComponent<Transform>().position = new Vector3(x, y, z);
            newNode.name = n.nodeName;
        }
    }

    void serializeNodes(List<testbed_node> nodes, int action)
    {
        SeriesData data = new SeriesData();
        SeriesArray array = new SeriesArray();
        List<string> nodeNames = new List<string>();
        data.Name = "Testbed_Nodes";
        List<float> values = new List<float>();
        List<SeriesData> arrayVals = new List<SeriesData>();
        string jsonString; 

        foreach (testbed_node n in nodes)
        {
            values.Add(n.xVal);
            values.Add(n.yVal);
            nodeNames.Add(n.shortName);
        }

        data.Data = values.ToArray();
        jsonString = JsonConvert.SerializeObject(data);
        arrayVals.Add(data);
        array.AllData = arrayVals.ToArray();

        print(array.AllData);

        if(action == 0)
        {
            visualizer.CreateMeshes(array.AllData, nodeNames);
        }
        else
        {
            visualizer.updateMeshes(array.AllData, nodeNames);
        }
        
    }

    IEnumerator ExecuteAfterTime(float time)
    {
        running = true; 
        yield return new WaitForSeconds(time);

        // Code to execute after the delay
        //SeriesData data = new SeriesData();
        //SeriesArray array = new SeriesArray();
        //data.Name = "Testbed_Nodes";
        //List<float> values = new List<float>();
        //List<SeriesData> arrayVals = new List<SeriesData>();
        //string jsonString;

        //foreach (testbed_node n in nodes)
        //{
        //    values.Add(n.xVal);
        //    values.Add(n.yVal);
        //    values.Add(Random.Range(.01f, .1f));
        //}

        //data.Data = values.ToArray();
        //jsonString = JsonConvert.SerializeObject(data);
        //arrayVals.Add(data);
        //array.AllData = arrayVals.ToArray();

        StartCoroutine(getJSONData(1));
        running = false; 
    }
}