namespace Transaction.Monitor.Common;
public class ResultDto<T>
{
    public T Data { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; }
    
    public ResultDto() { }

    public ResultDto(T data, bool success = true, string message = "")
    {
        Data = data;
        Success = success;
        Message = message;
    }

    public static ResultDto<T> SuccessResult(T data, string message = "")
    {
        return new ResultDto<T>(data, true, message);
    }

    public static ResultDto<T> FailureResult(string message)
    {
        return new ResultDto<T>(default, false, message);
    }
}
