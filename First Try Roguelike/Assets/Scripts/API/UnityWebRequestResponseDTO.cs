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

    public UnityWebRequestResponseDTO(UnityWebRequest request)
    {
        this.result = request.result;
        this.code = request.responseCode;
        this.body = request.downloadHandler.text;
    }

    public UnityWebRequest.Result getResult() { return this.result; }
    public long getCode() { return this.code; }
    public string getBody() { return this.body; }

    public void setResult(UnityWebRequest.Result result) { this.result = result; }
    public void setCode(long code) { this.code = code; }
    public void setBody(string body) { this.body = body; }

}
