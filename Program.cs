using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;


namespace ConsoleApp4
{

    internal class Program
    {
        static Dictionary<string, string> mask = new Dictionary<string, string>() {
            {"0", "0.0.0.0" },
            {"1", "128.0.0.0"},
            {"2", "192.0.0.0" },
            {"3", "224.0.0.0" },
            {"4", "240.0.0.0" },
            {"5", "248.0.0.0" },
            {"6", "252.0.0.0" },
            {"7", "254.0.0.0" },
            {"8", "255.0.0.0" },
            {"9", "255.128.0.0" },
            {"10", "255.192.0.0" },
            {"11", "255.224.0.0" },
            {"12", "255.240.0.0" },
            {"13", "255.248.0.0" },
            {"14", "255.252.0.0" },
            {"15", "255.254.0.0" },
            {"16", "255.255.0.0" },
            {"17", "255.255.128.0" },
            {"18", "255.255.192.0"},
            {"19", "255.255.224.0" },
            {"20", "255.255.240.0" },
            {"21", "255.255.248.0" },
            {"22", "255.255.252.0" },
            {"23", "255.255.254.0" },
            {"24", "255.255.255.0" },
            {"25", "255.255.255.128" },
            {"26", "255.255.255.192" },
            {"27", "255.255.255.224" },
            {"28", "255.255.255.240" },
            {"29", "255.255.255.248" },
            {"30", "255.255.255.252" },
            {"31", "255.255.255.254" },
            {"32", "255.255.255.255" },
        };

        public static Dictionary<string, string> Mask { get => mask; set => mask = value; }

        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Начните ввод");
                string input = Console.ReadLine().ToLower();


                if (input == "help")
                {
                    Help();
                    Main(args);
                }

                else
                {
                    string[] input_with_parpms = input.ToLower().Split(' ');
                    if (input_with_parpms.Length > 2)
                    {


                        var param = GetParam(input_with_parpms);
                        if (param["address-mask"] != " ")
                        {
                            param["address-mask"] = Mask[param["address-mask"]];
                        }
                        else
                        {
                            var para = param["address-mask"] = "32";
                            param["address-mask"] = Mask[para];
                        }
                        IpCalc(param["file-log"], param["file-output"], param["address-start"], param["address-mask"], param["time-start"], param["time-end"]);


                    }


                    else
                    {
                        Console.WriteLine("Некорректный ввод/Неизвестная команда");
                        Main(args);
                    }
                }


            }

            catch (Exception ex)
            {
                Console.WriteLine("Произошла ошибка/Некорректный ввод. Попробуйте снова");
                Main(args);
            }


        }
        static void Help()
        {
            Console.WriteLine("--file-log — путь к файлу с логами\r\n" +
                        "--file-output — путь к файлу с результатом\r\n" +
                        "--address-start —  нижняя граница диапазона адресов, необязательный параметр, по умолчанию обрабатываются все адреса\r\n" +
                        "--address-mask — маска подсети, задающая верхнюю границу диапазона десятичное число. Необязательный параметр. В случае, если он не указан, обрабатываются все адреса, начиная с нижней границы диапазона. Параметр нельзя использовать, если не задан address-start\r\n" +
                        "--time-start —  нижняя граница временного интервала\r\n" +
                        "--time-end — верхняя граница временного интервала.\r\n");
        }
        static Dictionary<string, string> GetParam(string[] parametrs)
        {

            string file_log = " ";
            string file_output = " ";
            string address_start = "0.0.0.0";
            string address_mask = " ";
            string time_start = " ";
            string time_end = " ";

            Dictionary<string, string> parms = new Dictionary<string, string>() { { "file-log", file_log }, { "file-output", file_output }, { "address-start", address_start }, { "address-mask", address_mask }, { "time-start", time_start }, { "time-end", time_end } };

            for (int i = 0; i < parametrs.Length; i++)
            {
                switch (parametrs[i])
                {
                    case "--file-log":

                        parms["file-log"] = parametrs[i - 1];
                        continue;
                    case "--file-output":

                        parms["file-output"] = parametrs[i - 1];
                        continue;
                    case "--address-start":

                        parms["address-start"] = parametrs[i - 1];
                        continue;
                    case "--address-mask":

                        parms["address-mask"] = parametrs[i - 1];
                        continue;
                    case "--time-start":

                        parms["time-start"] = parametrs[i - 2] + " " + parametrs[i - 1];
                        continue;
                    case "--time-end":
                        parms["time-end"] = parametrs[i - 2] + " " + parametrs[i - 1];
                        continue;


                }
            }

            return parms;

        }
        static DateTime StringToDateTime(string date, int flag)
        {
            var calendar = date.Split(' ')[0].Split('-');
            var times = date.Split(' ')[1].Split(':');
            DateTime time = new DateTime();

            if (flag == 0)
            {
                var day = Convert.ToInt32(calendar[0]);
                var mounth = Convert.ToInt32(calendar[1]);
                var year = Convert.ToInt32(calendar[2]);

                var hour = Convert.ToInt32(times[0]);
                var minute = Convert.ToInt32(times[1]);
                var seconds = Convert.ToInt32(times[2]);

                time = new DateTime(year, mounth, day, hour, minute, seconds);

            }
            else if (flag == 1)
            {
                var day = Convert.ToInt32(calendar[2]);
                var mounth = Convert.ToInt32(calendar[1]);
                var year = Convert.ToInt32(calendar[0]);

                var hour = Convert.ToInt32(times[0]);
                var minute = Convert.ToInt32(times[1]);
                var seconds = Convert.ToInt32(times[2]);

                time = new DateTime(year, mounth, day, hour, minute, seconds);

            }
            return time;
        }
        static void AddIp(string output, string line, string line_ip, string ip_start, string mask)
        {
            StreamWriter streamWriter = new StreamWriter(output, true);

            var int_ip = BitConverter.ToUInt32(IPAddress.Parse(line_ip).GetAddressBytes(), 0);

            var ip_range_start = BitConverter.ToUInt32(IPAddress.Parse(ip_start).GetAddressBytes(), 0);


            var ip_start_uint = IPAddress.Parse(ip_start).GetAddressBytes();
            var ip_line_uint = BitConverter.ToUInt32(ip_start_uint, 0);
            var mask_uint = BitConverter.ToUInt32(IPAddress.Parse(mask).GetAddressBytes(), 0);

            var end_ip = ip_line_uint | mask_uint;

            if (int_ip > ip_range_start && int_ip < end_ip)
            {

                streamWriter.WriteLine(line);

            }
            streamWriter.Close();
        }
        static void IpCalc(string input, string output, string ip_start = "0.0.0.0", string mask = "255.255.255.255", string time_start = " ", string time_end = " ")
        {
            StreamReader sr = new StreamReader(input);

            DateTime time_s = StringToDateTime("01-03-1900 06:17:54", 0);


            if (time_start != " ")
            {
                time_s = StringToDateTime(time_start, 0);

            }

            string line = sr.ReadLine();
            int i = 1;

            while (line != null)
            {
                var line_ip = line.Split(' ')[0];

                if (time_start != " ")
                {

                    var test = line.Split(' ')[1] + " " + line.Split(' ')[2];
                    var line_date = StringToDateTime(test, 1);


                    if (line_date > time_s)
                    {
                        AddIp(output, line, line_ip, ip_start, mask);
                        line = sr.ReadLine();



                    }
                    else
                    {
                        Console.WriteLine(i);
                        i++;
                        line = sr.ReadLine();
                        continue;
                    }
                }
                else if (time_end != " ")
                {
                    var test = line.Split(' ')[1] + " " + line.Split(' ')[2];
                    var line_date = StringToDateTime(test, 1);
                    var time_e = StringToDateTime(time_end, 1);


                    if (line_date < time_e)
                    {
                        AddIp(output, line, line_ip, ip_start, mask);
                        line = sr.ReadLine();



                    }
                    else
                    {
                        Console.WriteLine(i);
                        i++;
                        line = sr.ReadLine();
                        continue;
                    }
                }
                else if (time_end != " " && time_start != " ")
                {
                    var test = line.Split(' ')[1] + " " + line.Split(' ')[2];
                    var line_date = StringToDateTime(test, 1);
                    var time_e = StringToDateTime(time_end, 1);


                    if (line_date > time_s && line_date < time_e)
                    {
                        AddIp(output, line, line_ip, ip_start, mask);
                        line = sr.ReadLine();



                    }
                    else
                    {
                        Console.WriteLine(i);
                        i++;
                        line = sr.ReadLine();
                        continue;
                    }
                }
                else
                {
                    AddIp(output, line, line_ip, ip_start, mask);
                    Console.WriteLine(i);
                    i++;
                    line = sr.ReadLine();
                    continue;
                }
                line = sr.ReadLine();
                Console.WriteLine(i);
                i++;

            }
            sr.Close();

        }

    }
}
