using Router.Device.Net;
using TencentCloud.Dnspod.V20210323;
using TencentCloud.Dnspod.V20210323.Models;

namespace TencentCloudDns
{
    public class Global
    {
        public static DnspodClient Client { get; set; }

        public static RouterDevice Router { get; set; }
        public static DomainListItem Domain { get; set; }
        public static RecordListItem Record { get; set; }


        public static Configure Configure { get; set; }




        public static DomainListItem GetDomain(String domain)
        {
            DescribeDomainListRequest req = new DescribeDomainListRequest();
            DescribeDomainListResponse resp = Global.Client.DescribeDomainListSync(req);
            return resp.DomainList.Where(e => e.Name == domain).FirstOrDefault();
        }


        public static RecordListItem GetRecord(DomainListItem domain, String record)
        {
            DescribeRecordListRequest req = new DescribeRecordListRequest();
            req.Domain = domain.Name;
            DescribeRecordListResponse resp = Global.Client.DescribeRecordListSync(req);
            return resp.RecordList.Where(e => e.Name == record && e.Type == "A").FirstOrDefault();
        }

        public static void UpdateIpAddress(String address)
        {
            ModifyRecordRequest req = new ModifyRecordRequest();
            req.Domain = Global.Domain.Name;
            req.DomainId = Global.Domain.DomainId;
            req.RecordId = Global.Record.RecordId;
            req.SubDomain = Global.Record.Name;
            req.RecordType = Global.Record.Type;
            req.RecordLine = "默认";
            req.Value = address;
            req.MX = Global.Record.MX;
            req.TTL = Global.Record.TTL;
            Global.Client.ModifyRecordSync(req);
        }

    }
}
