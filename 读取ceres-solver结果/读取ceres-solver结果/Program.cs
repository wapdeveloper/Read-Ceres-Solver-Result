using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
namespace 读取ceres_solver结果
{
    class Program
    {
        public static double initial = 0;  //初始误差
        static void Main(string[] args)
        {
            string[] dense = File.ReadAllLines("dense");
            string[] sparse = File.ReadAllLines("sparse");
            string[] pcg_b = File.ReadAllLines("pcg-b");
            string[] pcg_s = File.ReadAllLines("pcg-s");
            Solver(dense, "dense");
            Solver(sparse, "sparse");
            Solver(pcg_b, "pcg-b");
            Solver(pcg_s, "pcg-s");
        }

        static void Solver(string[] input, string name)
        {
            StringBuilder time = new StringBuilder();
            StringBuilder value = new StringBuilder();
            List<Figure> list = new List<Figure>();
            list = GetResult(input);
            double ratio = initial;
            list[0].time = 0.1;  //重设初值
            list[0].value = 1;   //重设初值
            for (int i = 1; i < list.Count; i++)
            {
                list[i].value /= ratio;  //获取y轴数据
            }
            //Figure f = new Figure();
            //f.time = 0;
            //f.value = 0;
            //list.Insert(0, f);
            for (int i = 0; i < list.Count; i++)
            {
                time.AppendLine(list[i].time.ToString());
                value.AppendLine(list[i].value.ToString());
            }
            File.WriteAllText(name + "-time", time.ToString());
            File.WriteAllText(name + "-value", value.ToString());
        }

        static List<Figure> GetResult(string[] input)
        {

            List<Figure> list = new List<Figure>();
            for (int i = 0; i < input.Length; i++)
            {
                string[] t = input[i].Split(new string[] { "  " }, StringSplitOptions.RemoveEmptyEntries); //取出第一行
                if (t.Length > 1 && t[0] == " 0")
                {
                    for (int j = i; j < input.Length; j++)  //获取迭代数据
                    {
                        string[] t2 = input[j].Split(new string[] { "  " }, StringSplitOptions.RemoveEmptyEntries); //取出迭代数据
                        if (input[j].Length > 6)
                        {
                            if (input[j].Length > 6 && input[j].Substring(0, 6) == "Solver") //终止
                                return list;
                            //break;   跳出该层循环，可以求平均值 
                            //注意这个正则表达式！！！
                            //预处理的值（带指数）
                            string[] prevalue = t2[2].Split(new string[] { "e+", "e-" }, StringSplitOptions.RemoveEmptyEntries);
                            double value = double.Parse(prevalue[0]) * Math.Pow(10, double.Parse(prevalue[1]));
                            //预处理时间
                            string[] pretime = t2[9].Split(new string[] { "e+", "e-" }, StringSplitOptions.RemoveEmptyEntries);
                            double time = double.Parse(pretime[0]) * Math.Pow(10, double.Parse(pretime[1]));

                            if (initial == 0)
                                initial = value;
                            if (value > 0)  //负值为非成功迭代
                            {
                                Figure f = new Figure();
                                f.time = time;
                                f.value = value;
                                list.Add(f); //增加数据
                            }
                        }
                    }
                }
            }
            return null;
        }
    }
}
