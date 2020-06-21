using System.Threading.Tasks;

namespace POC.Web.Kafka.Consumer.Services.Background
{
    public interface IObserver
    {
        Task Notify(string message);
    }
}
