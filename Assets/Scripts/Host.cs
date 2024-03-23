using Newtonsoft.Json;

public class Host
{
    public string HostConnection { get; private set; }

    [JsonConstructor]
    public Host(string hostConnection)
    {
        HostConnection = hostConnection;
    }
}
