namespace POC.Web.Kafka.Consumer.Configs.Kafka
{
    public class ServiceConfig
    {
        public string Server { get; set; }
        public string Topic { get; set; }
        public string Group { get; set; }
        public int Timeout { get; set; }
    }
}
