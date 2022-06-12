// See https://aka.ms/new-console-template for more information
using Quartz;
using Quartz.Impl;
using Router.Device.Net;
using System.Text;
using TencentCloud.Common;
using TencentCloud.Common.Profile;
using TencentCloud.Dnspod.V20210323;
using TencentCloud.Dnspod.V20210323.Models;

namespace TencentCloudDns
{
    public class Program
    {

        private static System.Timers.Timer _Timer { get; set; }




        private static void InitializeClient(Configure configure)
        {
            Credential cred = new Credential
            {
                SecretId = configure.SecretId,
                SecretKey = configure.SecretKey
            };
            Util.OutLog("Initialize Client...");
            ClientProfile clientProfile = new ClientProfile();
            HttpProfile httpProfile = new HttpProfile();
            httpProfile.Endpoint = ("dnspod.tencentcloudapi.com");
            clientProfile.HttpProfile = httpProfile;
            Global.Client = new DnspodClient(cred, "", clientProfile);
        }

        public static void Main(String[] args)
        {
            Console.WriteLine("Hello, World!");
            if (!Directory.Exists("/config")) throw new DirectoryNotFoundException("directory not found '/config'");
            if (!Directory.Exists("/logs")) throw new DirectoryNotFoundException("directory not found '/logs'");
            Global.Configure = Util.GetConfigure();
            // Find Router
            Util.OutLog($"Find Router From {Global.Configure.NetAdapter}...");
            var routers = RouterDeviceNet.FindRouters(Global.Configure.NetAdapter).Result;
            if (routers.Count == 0)
            {
                Util.OutLog("Fail: Router not found from network adapter...");
                throw new Exception("Fail:Router not found from network adapter...");
            }
            Global.Router = routers.FirstOrDefault();
            Util.OutLog($"Router Name：{Global.Router.friendlyName}...");
            Util.OutLog($"Router  Url：{Global.Router.presentationURL.Trim()}...");
            // SDK Client
            InitializeClient(Global.Configure);
            try
            {
                Util.OutLog($"Request Domain List ...");
                Global.Domain = Global.GetDomain(Global.Configure.Domain);
                if (Global.Domain == null)
                {
                    Util.OutLog($"Fail: No domain name '{Global.Configure.Domain}' found under account...");
                    throw new Exception($"Fail: No domain name '{Global.Configure.Domain}' found under account...");
                }
                Util.OutLog($"Request Record List ...");
                Global.Record = Global.GetRecord(Global.Domain, Global.Configure.Record);
                if (Global.Record == null)
                {
                    Util.OutLog($"Fail: No resolution record found under domain name and of Type 'A' '{Global.Configure.Record}'...");
                    throw new Exception($"Fail: No resolution record found under domain name and of Type 'A' '{Global.Configure.Record}'...");
                }
            }
            catch (Exception ex)
            {
                Util.OutLog(ex.Message);
                return;
            }
            Util.OutLog($"Run Upgrade Job...");
            if (Global.Configure.IntervalSeconds.HasValue)
            {
                _Timer = new System.Timers.Timer();
                _Timer.Elapsed += _Timer_Elapsed;
                _Timer.AutoReset = true;
                _Timer.Interval = Global.Configure.IntervalSeconds.Value * 1000;
                _Timer.Start();
            }
            else
            {
                RunJob().Wait();
            }
            GC.Collect();
            Console.ReadKey();
        }

        private static void _Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Util.OutLog("Request ExternalIPAddress..");
            var originIpAddress = Global.Configure.IPAddress;
            var ipAddress = "";
            try
            {
                var task = Global.Router.GetExternalIPAddress();
                task.Wait();
                ipAddress = task.Result;
            }
            catch (Exception ex)
            {
                Util.OutLog($"Error: Request ExternalIPAddress Fail because:" + ex.Message);
            }
            try
            {
                if (Global.Configure.IPAddress != ipAddress)
                {
                    Util.OutLog($"The external IP address was changed to {ipAddress}..");
                    Global.UpdateIpAddress(ipAddress);
                    Global.Configure.IPAddress = ipAddress;
                    Util.SaveConfigure(Global.Configure);
                    Util.OutLog($"Updated Completely..");
                }
                else
                {
                    Util.OutLog($"The external IP address is not changed..");
                }
            }
            catch (Exception ex)
            {
                Util.OutLog($"Update failed because:{ex.Message}");
                Global.Configure.IPAddress = originIpAddress;
            }
        }



        public static async Task RunJob()
        {
            //创建一个工作
            IJobDetail job = JobBuilder.Create<WorkJob>()
            .WithIdentity("TestJob", "Test")
            .UsingJobData("name", "Hello")
            .Build();

            ITrigger trigger = TriggerBuilder.Create()
            .WithIdentity("TestJobTrigger", "Test")
            .WithCronSchedule(Global.Configure.UpdateCron)
            .Build();

            StdSchedulerFactory factory = new StdSchedulerFactory();
            //创建任务调度器
            IScheduler scheduler = await factory.GetScheduler();
            //启动任务调度器
            await scheduler.Start();
            //将创建的任务和触发器条件添加到创建的任务调度器当中
            await scheduler.ScheduleJob(job, trigger);
        }
    }


}