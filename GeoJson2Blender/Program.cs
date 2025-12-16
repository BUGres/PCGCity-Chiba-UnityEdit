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
//using geoJsonMap;
using System.Numerics;

try
{
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

            List<(string, List<Vector3>, List<(int, int)>)> objects = new List<(string, List<Vector3>, List<(int, int)>)>();

            foreach (var feature in geojo.Features)
            {
                Console.WriteLine("\tname=" + feature.Properties["name"]);
                // item.name = feature.Properties["name"] as string;
                switch (feature.Geometry)
                {
                    //case Point p:
                    //    // Console.WriteLine($"({p.Coordinates.Longitude}, {p.Coordinates.Latitude})");
                    //    break;
                    case LineString ls: // ls 路径
                        int indexoffset = 0;
                        foreach (var o in objects)
                        {
                            indexoffset += o.Item2.Count;
                        }
                        List<Vector3> points = new List<Vector3>();
                        List<(int, int)> lines = new List<(int, int)>();
                        foreach (var point in ls.Coordinates)
                        {
                            float mx = (float)(2 * MathF.PI * 6371 * (point.Longitude - 140.1107589));
                            float my = (float)(2 * MathF.PI * 6371 * (point.Latitude - 35.6104952));

                            points.Add(new Vector3(mx, my, 0));
                        }
                        for (int i = 0; i < points.Count; i++)
                        {
                            if (i != 0)
                            {
                                lines.Add((i + indexoffset, i + 1 + indexoffset));
                            }
                        }
                        objects.Add(("road" + objects.Count.ToString(), points, lines));
                        break;
                    //case Polygon pg:
                    //    // Console.WriteLine("\t\tPolygon包含 " + pg.Coordinates.First() as .Count() + "个顶点");
                    //    break;
                    case MultiPolygon mpg: // 区域
                        int indexoffset1 = 0;
                        foreach (var o in objects)
                        {
                            indexoffset1 += o.Item2.Count;
                        }
                        List<Vector3> points1 = new List<Vector3>();
                        List<(int, int)> lines1 = new List<(int, int)>();
                        foreach (var point in mpg.Coordinates.First().Coordinates.First().Coordinates)
                        {
                            float mx = (float)(2 * MathF.PI * 6371 * (point.Longitude - 140.1107589));
                            float my = (float)(2 * MathF.PI * 6371 * (point.Latitude - 35.6104952));

                            points1.Add(new Vector3(mx, my, 0));
                        }
                        for (int i = 0; i < points1.Count; i++)
                        {
                            if (i != 0)
                            {
                                if (i != points1.Count - 1)
                                {
                                    lines1.Add((i + indexoffset1, i + 1 + indexoffset1));
                                }
                                else 
                                {
                                    lines1.Add((i + indexoffset1, indexoffset1 + 1));
                                }
                                
                            }
                        }
                        objects.Add(("area" + objects.Count.ToString(), points1, lines1));
                        break;
                    //mapData.Items.Add(item);
                    //break;
                    default:
                        Console.WriteLine("\t\t类型坐标未能解析" + feature.Geometry.Type);
                        break;
                }
            }

            string str = "mtllib material.mtl\n";
            foreach (var o in objects)
            {
                str += "o " + o.Item1 + "\n";
                foreach (var point in o.Item2)
                {
                    str += "v " + point.X + " " + point.Y + " " + point.Z + "\n";
                }

                foreach (var line in o.Item3)
                {
                    str += "l " + line.Item1 + " " + line.Item2 + "\n";
                }
            }
            File.WriteAllText(file.Split('\\').ToList()[^1] + ".obj", str);
        }
    }
}
catch (Exception e)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("似乎出现了错误：" + e.Message);
}

Console.WriteLine("所有代码已经执行完毕，你可以关闭此窗口了");
while (true);