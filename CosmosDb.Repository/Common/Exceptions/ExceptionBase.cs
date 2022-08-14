namespace CosmosDb.Repository.Common.Exceptions
{
    public class ExceptionBase 
        : Exception
    {
        public string Details { get; set; }

        public ExceptionBase(string message, string details = "") : base(message)
        {
            Details = details;
        }
    }
}
