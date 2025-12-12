using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace geoJsonMap;

[JsonSerializable(typeof(MapPoint))]
public class MapPoint
{
    public double X { get; set; }
    public double Y { get; set; }
}

[JsonSerializable(typeof(MapItem))]
public class MapItem
{
    public string type { get; set; } = "";
    public string name { get; set; } = string.Empty;
    public List<MapPoint> Points { get; set; } = new List<MapPoint>();
}

[JsonSerializable(typeof(MapCSJson))]
public class MapCSJson
{
    public List<MapItem> Items { get; set; } = new List<MapItem> {};
}
