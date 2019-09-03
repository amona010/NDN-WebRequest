using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DataVisualizer : MonoBehaviour
{
    public GameObject Earth;
    public GameObject PointPrefab;
    public GameObject circle;
    public GameObject spawner; 
    public GameObject particlePrefab;
    public float ValueScaleMultiplier = 1;
    GameObject[] seriesObjects;
    int currentSeries = 0;
    public void CreateMeshes(SeriesData[] allSeries, List<string> nodeNames)
    {
        seriesObjects = new GameObject[allSeries.Length];
        string[] names = nodeNames.ToArray();
        int index;

        for (int i = 0; i < allSeries.Length; i++)
        {
            GameObject seriesObj = new GameObject(allSeries[i].Name);
            seriesObj.transform.parent = Earth.transform;
            seriesObjects[i] = seriesObj;
            SeriesData seriesData = allSeries[i];
            index = 0;
            for (int j = 0; j < seriesData.Data.Length; j += 2)
            {
                float lat = seriesData.Data[j];
                float lng = seriesData.Data[j + 1];
                AppendPointVertices(seriesObj, lng, lat, names[index]);
                index = index + 1;
            }
            //CreateObject(meshVertices, meshIndices, meshColors, seriesObj);
            seriesObjects[i].SetActive(false);
        }

        seriesObjects[currentSeries].SetActive(true);
    }
    private void AppendPointVertices(GameObject p, float lng, float lat, string name)
    {
        GameObject g = Instantiate<GameObject>(circle);
        g.name = name;
        Vector3 pos;
        pos.x = 0.5f * Mathf.Cos((lng) * Mathf.Deg2Rad) * Mathf.Cos(lat * Mathf.Deg2Rad);
        pos.y = 0.5f * Mathf.Sin(lat * Mathf.Deg2Rad);
        pos.z = 0.5f * Mathf.Sin((lng) * Mathf.Deg2Rad) * Mathf.Cos(lat * Mathf.Deg2Rad);
        g.transform.parent = p.transform;
        g.transform.position = pos;
        g.transform.localScale = new Vector3(.005f, .005f, .005f); //Value in which point size is made
        g.transform.LookAt(pos * 2);


    }

    //private void CreateObject(List<Vector3> meshertices, List<int> meshindecies, List<Color> meshColors, GameObject seriesObj)
    //{
    //    Mesh mesh = new Mesh();
    //    mesh.vertices = meshertices.ToArray();
    //    mesh.triangles = meshindecies.ToArray();
    //    mesh.colors = meshColors.ToArray();
    //    GameObject obj = new GameObject();
    //    obj.transform.parent = Earth.transform;
    //    obj.AddComponent<MeshFilter>().mesh = mesh;
    //    obj.AddComponent<MeshRenderer>().material = PointMaterial;
    //    obj.transform.parent = seriesObj.transform;
    //}

    public void ActivateSeries(int seriesIndex)
    {
        if (seriesIndex >= 0 && seriesIndex < seriesObjects.Length)
        {
            seriesObjects[currentSeries].SetActive(false);
            currentSeries = seriesIndex;
            seriesObjects[currentSeries].SetActive(true);

        }
    }
    //New function which should update point locations
    public void updateMeshes(SeriesData[] allSeries, List<string> nodeNames)
    {
        int index;
        string[] names = nodeNames.ToArray();

        foreach (Transform child in Earth.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < allSeries.Length; i++)
        {
            GameObject seriesObj = new GameObject(allSeries[i].Name);
            seriesObj.transform.parent = Earth.transform;
            seriesObjects[i] = seriesObj;
            SeriesData seriesData = allSeries[i];
            index = 0;
            for (int j = 0; j < seriesData.Data.Length; j += 2)
            {
                float lat = seriesData.Data[j];
                float lng = seriesData.Data[j + 1];
                AppendPointVertices(seriesObj, lng, lat, names[index]);
                index = index + 1;
            }
        }
    }

    public void createConnections(Dictionary<int, string> connections)
    {

        var allKeys = connections.Keys.ToArray();

        foreach (int key in allKeys)
        {
            string[] values = connections[key].Split(',');

            GameObject startNode = GameObject.Find(values[0]);
            GameObject endNode = GameObject.Find(values[1]);

            if (startNode != null && endNode != null)
            {
                GameObject newSpawner = Instantiate<GameObject>(spawner);
                newSpawner.GetComponent<ParticleSpawner>().particle = particlePrefab;
                newSpawner.transform.position = startNode.transform.position;
                newSpawner.GetComponent<ParticleSpawner>().source = startNode.transform.position;
                newSpawner.GetComponent<ParticleSpawner>().DestinationName = endNode.name;
                newSpawner.GetComponent<ParticleSpawner>().Destination = endNode.transform.position;
            }



        }
    }

}
[System.Serializable]
public class SeriesData
{
    public string Name;
    public float[] Data;
}
