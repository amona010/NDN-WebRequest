using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public class NDNData : MonoBehaviour
{
    public string path = "C:/Users/Alex/Documents/GitHub/NDN WebRequest/Assets/Resources/newTest.json";
    public List<testbed_node> nodes;
    public GameObject node;

    void Start()
    {
        nodes = new List<testbed_node>();
        StartCoroutine(getJSONData());
    }

    IEnumerator getJSONData()
    {
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
        UnityWebRequest www = UnityWebRequest.Get("http://ndnmap.arl.wustl.edu/json/geocode/");
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {

            JsonTextReader reader = new JsonTextReader(new StringReader(www.downloadHandler.text));

            while(reader.Read())
            { 
                if (reader.Value != null)
                {
                   Debug.Log(reader.TokenType + " " + reader.Value);
                   if(reader.TokenType.ToString().Equals("PropertyName"))
                    {
                        Debug.Log("This is the name of a property");
                        propertyName = reader.Value.ToString();

                        if(propertyName.Equals("position"))
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
                        switch(propertyName)
                        {
                            case "name":
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

                    if(reader.TokenType.ToString().Equals("EndObject"))
                    {
                        if(tokenType.Equals("EndObject"))
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

            foreach(testbed_node n in nodes)
            {
                Debug.Log(n.nodeName);
                GameObject newNode = Instantiate(node);
                newNode.GetComponent<Transform>().position = new Vector3(n.xVal, n.yVal, 0);
                newNode.name = n.nodeName;
            }

            yield break;
        }
    }
}
