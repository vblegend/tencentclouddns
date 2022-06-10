using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TencentCloudDns
{
    public class Configure
    {
        public String SecretId { get; set; } = "your，SecretId";
        public String SecretKey { get; set; } = "your，SecretKey";
        public String Domain { get; set; } = "domain.com";
        public String Record { get; set; } = "www";
        public String UpdateCron { get; set; } = "0 /5 * * * ? *";
        public String NetAdapter { get; set; } = "eth0";
        public String IPAddress { get; set; } = "";
        public Int32? IntervalSeconds { get; set; } = 300;
    }
}
