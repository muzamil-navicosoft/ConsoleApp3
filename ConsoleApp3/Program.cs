using System;
using System.Collections;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;

class Program
{
    private static readonly object NewtonSoft;

    static async Task Main(string[] args)
    {
        var result = await SendPostRequestAsync();
        //await SendGetRequestAsync(result);
        //await SendPostDomainCreateRequestAsync(result);
        var result2 =  await SendPostSubDomainCreateRequestAsync(result);
    }

    static async Task<string> SendPostRequestAsync()
    {
        HttpClientHandler clientHandler = new HttpClientHandler();
        clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

        using HttpClient client = new HttpClient(clientHandler);

        // Set the base address
        client.BaseAddress = new Uri("https://104.219.233.15:8443/");

        // Set the authentication credentials
        var byteArray = Encoding.ASCII.GetBytes("admin:7a24De3j#ajs,aaa9i8j");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
        
        // Prepare the content (empty JSON object in this case)
        string jsonContent = "{}";
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        // Send the POST request
        HttpResponseMessage response = await client.PostAsync("api/v2/auth/keys", content);

        // Check the response
        if (response.IsSuccessStatusCode)
        {
            string responseContent = await response.Content.ReadAsStringAsync();


            int equalsSignIndex = responseContent.IndexOf(':');

            // Get the value part of the key-value pair.
            string value = responseContent.Substring(equalsSignIndex + 1);

            // Remove any extra quotes from the value.
            value = value.Replace("\"","");
            value = value.Trim();
            value = value.Trim('\"');
            value = value.Trim('}');
            value = value.Trim('\n');
            

            Console.WriteLine(value);
            return value;
            
           // var key = responseContent.Split(':');
           // Console.WriteLine("Response: " + key[1]);
           // return responseContent.Trim('"');
        }
        else
        {
            Console.WriteLine("Error: " + response.ReasonPhrase);
            return string.Empty;
        }


    }
    static async Task<string> SendPostDomainCreateRequestAsync(string apiKey)
    {
        HttpClientHandler clientHandler = new HttpClientHandler();
        clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

        using HttpClient client = new HttpClient(clientHandler);

        // Set the base address
        client.BaseAddress = new Uri("https://104.219.233.15:8443/");

        //client.DefaultRequestHeaders.Add("accept", "application/json");
        client.DefaultRequestHeaders.Add("X-API-Key", apiKey);
       

        var obj = new Subdomain();

        obj.name = "testingConsoleDomain.navedge.co";
        var test = JsonSerializer.Serialize(obj);
        Console.WriteLine(test);
        var testing = Newtonsoft.Json.JsonConvert.SerializeObject(obj);

        var content = new StringContent(test, Encoding.UTF8, "application/json");

        // Send the POST request
        HttpResponseMessage response = await client.PostAsync("api/v2/domains", content);

        // Check the response
        if (response.IsSuccessStatusCode)
        {
            string responseContent = await response.Content.ReadAsStringAsync();

            Console.WriteLine(response.StatusCode.ToString());
            return response.StatusCode.ToString();

        }
        else
        {
            Console.WriteLine("Error: " + response.ReasonPhrase);
            return string.Empty;
        }


    }

    static async Task<string> SendPostSubDomainCreateRequestAsync(string apiKey)
    {
        HttpClientHandler clientHandler = new HttpClientHandler();
        clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

        using HttpClient client = new HttpClient(clientHandler);

        // Set the base address
        client.BaseAddress = new Uri("https://104.219.233.15:8443/");

        //client.DefaultRequestHeaders.Add("accept", "application/json");
        client.DefaultRequestHeaders.Add("X-API-Key", apiKey);


        var obj = new SSubdomain();

        obj.@params.Insert(1, "nadeemNanaabuWala");
        obj.@params.Insert(5, "testmuzamil.navedge.co");


        
        var test = JsonSerializer.Serialize(obj);
        Console.WriteLine(test);
        var testing = Newtonsoft.Json.JsonConvert.SerializeObject(obj);

        var content = new StringContent(test, Encoding.UTF8, "application/json");

        // Send the POST request
        HttpResponseMessage response = await client.PostAsync("api/v2/cli/subdomain/call", content);

        // Check the response
        
        string responseContent = await response.Content.ReadAsStringAsync();
        //var obj2 = new responseObj();
        var obj2 = JsonSerializer.Deserialize<responseObj>(responseContent);
        if(obj2.code != 0)
        {
            Console.WriteLine("Error: " + obj2.stdout);
            return string.Empty;
        }
        else
        {
            Console.WriteLine("Created");
            return "Created";
        }

    }
    static async Task SendGetRequestAsync(string apiKey)
    {
        HttpClientHandler clientHandler = new HttpClientHandler();
        clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
        using HttpClient client = new HttpClient(clientHandler);

        // Set the base address and include the API key in the header
        client.BaseAddress = new Uri("https://104.219.233.15:8443/");
        //client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
        
        //client.DefaultRequestHeaders.Add ("X - API - Key: ",apiKey);
        client.DefaultRequestHeaders.Add("accept", "application/json");
        client.DefaultRequestHeaders.Add("X-API-Key", apiKey);



        // Send the GET request
        HttpResponseMessage response = await client.GetAsync("api/v2/domains");

        // Check the response
        if (response.IsSuccessStatusCode)
        {
            string responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Response: " + responseContent);
        }
        else
        {
            Console.WriteLine("Error: " + response.ReasonPhrase);
        }
    }
}

public class HostingSettings
{
    public string ftp_login { get; set; }
    public string ftp_password { get; set; }
}

public class Plan
{
    public string name { get; set; } 
}

public class Subdomain
{
    public string name { get; set; }
    public string hosting_type { get; set; } = "virtual";
    public HostingSettings hosting_settings { get; set; } = new HostingSettings { ftp_login = "xxyyftplogin", ftp_password = "testApi@1234" };
    public List<string> ipv4 { get; set; } = new List<string> { "104.219.233.15" };
    public Plan plan { get; set; } = new Plan { name = "Unlimited"};
}

public class SSubdomain
{
    public List<string> @params { get; set; } = new List<string> { "--create", "-domain", "navedge.co", "-www-root"};
}

public class responseObj
{
    public int code { get; set; } 
    public string stdout { get; set; } = string.Empty;
    public string stderr { get; set; } = string.Empty;


}