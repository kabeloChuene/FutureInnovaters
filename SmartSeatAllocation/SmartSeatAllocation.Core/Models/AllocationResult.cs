namespace SmartSeatAllocation.Core.Models
{
    public class AllocationResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public Participant Participant { get; set; }
        public Session Session { get; set; }

        public AllocationResult()
        {
        }

        public AllocationResult(bool isSuccess, string message)
        {
            IsSuccess = isSuccess;
            Message = message;
        }

        public AllocationResult(bool isSuccess, string message, Participant participant, Session session) 
            : this(isSuccess, message)
        {
            Participant = participant;
            Session = session;
        }

        public static AllocationResult Success(Participant participant, Session session)
        {
            return new AllocationResult(
                true, 
                $"Successfully allocated {participant.Name} to {session.Name} session", 
                participant, 
                session);
        }

        public static AllocationResult Failure(string message)
        {
            return new AllocationResult(false, message);
        }
    }
}
