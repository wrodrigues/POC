using System.Threading.Tasks;

namespace POC.Web.Kafka.Consumer.Services.Background
{
    public class Observer : IObserver
    {
        public Task Notify(string message)
        {
            return Task.Run(() =>
            {
                System.Diagnostics.Debug.WriteLine($"Mensagem recebida do barramento: {message}");
            });
        }
    }
}
