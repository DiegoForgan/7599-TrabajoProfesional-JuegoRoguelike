using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class APIErrorResponseDTO 
{
    public int code;
    public string message;
    public string data;

    public int getCode() { return this.code; }
    public string getMessage() { return this.message; }
    public string getData() { return this.data; }

    public void setCode(int code) { this.code = code; }
    public void setMessage(string message) { this.message = message; }
    public void setData(string data) { this.data = data; }
}
