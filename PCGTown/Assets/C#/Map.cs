using System;
using System.Collections.Generic;

[Serializable]
public class MapPoint
{
    public double X;
    public double Y;
}
[Serializable]
public class MapItem
{
    public string type = "";
    public string name = string.Empty;
    public List<MapPoint> Points = new List<MapPoint>();
}
[Serializable]
public class MapCSJson
{
    public List<MapItem> Items = new List<MapItem> {};
}
