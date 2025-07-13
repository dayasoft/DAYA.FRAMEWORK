namespace Daya.Sample.Domain.Commons
{
    public class DayaErrorAttribute : Attribute
    {
        public string Message { get; private set; } = null!;

        public DayaErrorAttribute(string message)
        {
            Message = message;
        }
    }
}