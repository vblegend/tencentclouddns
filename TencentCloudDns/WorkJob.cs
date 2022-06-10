using Quartz;


namespace TencentCloudDns
{


    [DisallowConcurrentExecution]
    public class WorkJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            return Task.Run(async () =>
            {
                Util.OutLog("Request ExternalIPAddress..");
                var originIpAddress = Global.Configure.IPAddress;
                var ipAddress = "";

                try
                {
                    ipAddress = await Global.Router.GetExternalIPAddress();
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
                }catch (Exception ex)
                {
                    Util.OutLog($"Update failed because:{ex.Message}");
                    Global.Configure.IPAddress = originIpAddress;
                }
            });
        }











    }

}
