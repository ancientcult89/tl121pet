namespace tl121pet.Entities.Infrastructure.Exceptions
{
    public class DataFoundException : Exception
    {
        public DataFoundException()
        {
        }

        public DataFoundException(string message)
            : base(message)
        {
        }

        public DataFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
