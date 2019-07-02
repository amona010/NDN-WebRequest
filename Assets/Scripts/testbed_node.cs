using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testbed_node
{
    public string nodeName = null;
    string shortName = null;
    string site = null;
    string https = null;
    bool backBone;
    bool wstls;
    string prefix = null;
    bool fchEnabled;
    bool ndnUp;
    public float xVal;
    public float yVal;

    public testbed_node(string name, string shortName, string site, string https, bool backbone, bool wstls, string prefix, bool fchEnabled, bool ndnUp, float x, float y)
    {
        nodeName = name;
        this.shortName = shortName;
        this.site = site;
        this.https = https;
        backBone = backbone;
        this.wstls = wstls;
        this.prefix = prefix;
        this.fchEnabled = fchEnabled;
        this.ndnUp = ndnUp;
        xVal = x;
        yVal = y;
    }

    public string toString()
    {
        return nodeName + " " + shortName + " " + site + " " + https + " " + backBone + " " + wstls + " " + prefix + " " + fchEnabled + " " + ndnUp + " " + xVal + " " + yVal;
    }
}
