using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TencentCloudDns
{
    public class Util
    {
        public static readonly String logFile = "/logs/Tencent.Cloud.Dns.log";
        public static readonly String configFile = "/config/Tencent.Cloud.Dns.config";
        public static readonly JsonSerializerOptions SerializerOptions = new JsonSerializerOptions() { WriteIndented = true };

        public static Configure GetConfigure()
        {
            var configure = new Configure();
            if (!File.Exists(configFile))
            {
                OutLog("Initialize Configure File...");
                SaveConfigure(configure);
                return configure;
            }
            using (StreamReader sw = new StreamReader(configFile, Encoding.UTF8))
            {
                configure = JsonSerializer.Deserialize<Configure>(sw.BaseStream);
            }
            return configure;
        }

        public static void SaveConfigure(Configure configure)
        {
            using (StreamWriter sw = new StreamWriter(configFile, false, Encoding.UTF8))
            {
                var json = JsonSerializer.Serialize(configure, typeof(Configure), SerializerOptions);
                sw.Write(json);
            }
        }

        public static void OutLog(String message)
        {
            var outMessage = $"{DateTime.UtcNow.ToString("yyyy-MM-dd HH:dd:ss")} {message}";
            using (StreamWriter sw = new StreamWriter(logFile, true, Encoding.UTF8))
            {
                sw.WriteLine(outMessage);
            }
            Console.WriteLine(outMessage);
        }


    }
}
