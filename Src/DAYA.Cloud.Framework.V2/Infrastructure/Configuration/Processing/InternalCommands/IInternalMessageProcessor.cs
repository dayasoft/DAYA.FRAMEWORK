using System.Threading.Tasks;

public interface IInternalMessageProcessor
{
    Task ProcessMessageAsync(string messageBody);
}