namespace EPloy.Hotfix.Net
{
    public interface IMessage
    {
       
    }

    public interface IRequest : IMessage
    {
        int RpcId { get; set; }
    }

    public interface IResponse : IMessage
    {
        int RpcId { get; set; }
        int Error { get; set; }
        string Message { get; set; }
    }

    public class ErrorResponse : IResponse
    {
        public int Error { get; set; }
        public string Message { get; set; }
        public int RpcId { get; set; }
    }
}