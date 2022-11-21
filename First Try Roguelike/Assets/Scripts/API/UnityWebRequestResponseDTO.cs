using UnityEngine.Networking;

public class UnityWebRequestResponseDTO 
{
    private UnityWebRequest.Result result;
    private long code;
    private string body;

    public UnityWebRequestResponseDTO(UnityWebRequest.Result result, long code, string body) {
        this.result = result;
        this.code = code;
        this.body = body;
    }

    public UnityWebRequest.Result getResult() { return this.result; }
    public long getCode() { return this.code; }
    public string getBody() { return this.body; }

    public void setResult(UnityWebRequest.Result result) { this.result = result; }
    public void setCode(long code) { this.code = code; }
    public void setBody(string body) { this.body = body; }

}
