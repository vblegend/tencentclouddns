# TencentCloudDNS
腾讯云域名DNS自动同步 路由器外网IP地址
适合NAS的DDNS实现 把路由器的公网拨号地址更新至腾讯云域名DNS解析

### 原理
启动时通过UDP组播的发现协议查找路由器设备
通过发现协议查找路由器外网IP地址接口并组装参数
通过定时器间隔指定时间从路由器获取公网IP
当公网IP变动时更新至域名解析记录



## docker 快速使用

仓库地址 https://hub.docker.com/r/vblegend2030/tencentclouddns

``` bash
# 拉取最新镜像
docker pull vblegend2030/tencentclouddns:latest

# 运行容器 记得增加 config logs 卷的映射
docker run --network=host -v /config:/config -v /logs:/logs -d vblegend2030/tencentclouddns:latest
```
### 初次运行会在 config目录下创建 `Tencent.Cloud.Dns.config` 内容如下
``` json
{
  "SecretId": "tencentcloud SecretId",
  "SecretKey": "tencentcloud SecretKey",
  "Domain": "domain.com",
  "Record": "www",
  "IntervalSeconds": 300,
  "UpdateCron": "0 /5 * * * ? *",
  "NetAdapter": "eth0",
  "IPAddress": ""
}
```


| 字段名 | 说明 | 必须 |
| ------ | ------ | ------ |
| SecretId | TencentCloud SecretId | 是 |
| SecretKey | TencentCloud SecretKey | 是 |
| Domain | 域名（必须是SecretKey下的） | 是 |
| Record | 域名的记录名 必须是已存在的A记录 | 是 |
| IntervalSeconds | 更新间隔（秒），默认 | 是 |
| UpdateCron | 更新crom表达式，IntervalSeconds为空时使用此字段 | 是 |
| NetAdapter | 路由器所在网卡名 | 是 |
| IPAddress | 上次更新IP地址（更新至DNS时会保存），初次留空 "" | 否 |

### 修改配置文件后重启docker容器即可。
