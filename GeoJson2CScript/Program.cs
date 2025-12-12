using GeoJSON.Net;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using geoJsonMap;

try
{
    MapCSJson mapData = new MapCSJson();

    // 获取当前程序运行路径
    var currentPath = Directory.GetCurrentDirectory();
    Console.WriteLine($"当前路径：{currentPath}\n");

    // 筛选当前路径下的所有 .geojson 文件（不包含子文件夹）
    var geoJsonFiles = Directory.EnumerateFiles(currentPath, "*.geojson", SearchOption.TopDirectoryOnly);

    // 输出结果
    if (!geoJsonFiles.Any())
    {
        Console.WriteLine("当前路径下未找到 .geojson 文件");
    }
    else
    {
        foreach (var file in geoJsonFiles)
        {
            Console.WriteLine("尝试解析解析" + file);
            // 读取文件内容
            string jsonContent = File.ReadAllText(file);
            var geojo = JsonConvert.DeserializeObject<FeatureCollection>(jsonContent);
            foreach (var feature in geojo.Features)
            {
                MapItem item = new MapItem();
                Console.WriteLine("\tname=" + feature.Properties["name"]);
                item.name = feature.Properties["name"] as string;
                switch (feature.Geometry)
                {
                    //case Point p:
                    //    // Console.WriteLine($"({p.Coordinates.Longitude}, {p.Coordinates.Latitude})");
                    //    break;
                    case LineString ls:
                        Console.WriteLine("\t\tLineString顶点量" + ls.Coordinates.Count());
                        item.type = "road";
                        foreach (var point in ls.Coordinates)
                        {
                            item.Points.Add(new MapPoint() { X = point.Longitude, Y = point.Latitude });
                        }
                        mapData.Items.Add(item);
                        break;
                    //case Polygon pg:
                    //    // Console.WriteLine("\t\tPolygon包含 " + pg.Coordinates.First() as .Count() + "个顶点");
                    //    break;
                    case MultiPolygon mpg:
                        //foreach (var pg in mpg.Coordinates)
                        //{
                        //    Console.WriteLine("\t\tMultiPolygon " + pg.Coordinates.Count());
                        //}

                        Console.WriteLine("\t\tMultiPolygon " + mpg.Coordinates.Count());
                        Console.WriteLine("\t\tMultiPolygon " + mpg.Coordinates.First().Coordinates.Count());
                        Console.WriteLine("\t\tMultiPolygon " + mpg.Coordinates.First().Coordinates.First().Coordinates.Count());
                        Console.WriteLine("\t\tMultiPolygon " + mpg.Coordinates.First().Coordinates.First().Coordinates.First().Longitude);
                        Console.WriteLine("\t\tMultiPolygon " + mpg.Coordinates.First().Coordinates.First().Coordinates.First().Latitude);
                        item.type = "area";
                        foreach (var point in mpg.Coordinates.First().Coordinates.First().Coordinates)
                        {
                            item.Points.Add(new MapPoint() { X = point.Longitude, Y = point.Latitude });
                        }
                        mapData.Items.Add(item);
                        break;
                    default:
                        Console.WriteLine("\t\t类型坐标未能解析" + feature.Geometry.Type);
                        break;
                }
            }
        }
    }

    File.WriteAllText("Map.json", System.Text.Json.JsonSerializer.Serialize(mapData));
}
catch (Exception e)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("似乎出现了错误：" + e.Message);
}

Console.WriteLine("所有代码已经执行完毕，你可以关闭此窗口了");
while (true);