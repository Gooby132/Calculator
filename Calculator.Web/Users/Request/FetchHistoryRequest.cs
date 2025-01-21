namespace Calculator.Web.Users.Request;

public class FetchHistoryRequest
{

    public int UserId { get; set; }

    public int Size { get; set; }
    public int Page { get; set; }
    public string FromInUtc { get; set; }
    public string UntillInUtc { get; set; }

}
