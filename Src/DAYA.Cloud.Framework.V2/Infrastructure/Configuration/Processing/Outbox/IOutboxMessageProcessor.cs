using System.Threading.Tasks;

public interface IOutboxMessageProcessor
{
    Task ProcessMessageAsync(string messageBody);
}