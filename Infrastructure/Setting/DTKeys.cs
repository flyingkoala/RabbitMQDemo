using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Setting
{
    public class DTKeys
    {
        //Rabbit MQ配置(格式：环境.项目.服务.类别.子类.名称)
        public const string MQ_OSP_ExchangeName = "msg.exchange.osp";//口扫数据包
        public const string MQ_OSP_QueueName = "msg.queuename.osp";//口扫数据包
        public const string MQ_OSP_RouteKey = "msg.routekey.osp";//口扫数据包
    }
}
