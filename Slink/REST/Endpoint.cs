using System.Text;
using System.Collections.Generic;

namespace Slink
{
    public class NVCEndpoint
    {
        public static string BaseURL = "http://138.197.213.66:8082"; //"http://pwampa.com:8081";

        public int ID { get; set; }
        public string Name { get; set; }
        public EndpointType Method { get; set; }
        public string Controller { get; set; }
        public string URL { get; set; }
        public int TimeoutSections { get; set; }

        public NVCEndpoint() { }

        public NVCEndpoint(int id, string name, EndpointType method)
        {
            ID = id;
            Name = name;
            Method = method;
            URL = name;
        }


        public enum EndpointType
        {
            Fake = -1,

            Post = 0,
            PostXWWWFormUrlEncoded = 1,
            PostFormData = 2,
            PostRaw = 3,

            Get = 10,
            Put = 20,
            Delete = 30
        }

        public void AppendToUrl(List<string> appendStrings)
        {
            if (appendStrings != null)
            {
                StringBuilder stringBuilder = new StringBuilder();
                foreach (string str in appendStrings)
                {
                    stringBuilder.Append("/");
                    stringBuilder.Append(str);
                }
                URL = URL + stringBuilder.ToString();
            }
        }
    }









    public class Endpoint
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public EndpointType Method { get; set; }
        public string Controller { get; set; }
        public string URL { get; set; }
        public int TimeoutSections { get; set; }

        public Endpoint() { }

        public Endpoint(int id, string name, EndpointType method)
        {
            ID = id;
            Name = name;
            Method = method;
            URL = name;
        }


        public enum EndpointType
        {
            Fake = -1,

            Post = 0,
            PostXWWWFormUrlEncoded = 1,
            PostFormData = 2,

            Get = 10,
            Put = 20,
            Delete = 30
        }

        public void AppendToUrl(List<string> appendStrings)
        {
            if (appendStrings != null)
            {
                StringBuilder stringBuilder = new StringBuilder();
                foreach (string str in appendStrings)
                {
                    stringBuilder.Append("/");
                    stringBuilder.Append(str);
                }
                URL = URL + stringBuilder.ToString();
            }
        }
    }

    public class EndpointManager
    {
        public Endpoint GetEndpointByName(string name)
        {
            Endpoint.EndpointType serviceType = Endpoint.EndpointType.Fake;

            switch (name.ToLower())
            {
                case "user":
                case "oauth/token":
                    serviceType = Endpoint.EndpointType.Post;
                    break;
                case "updateuser":
                    serviceType = Endpoint.EndpointType.Put;
                    break;
            }

            return new Endpoint(0, Strings.SystemUrls.base_url, serviceType);
        }
    }
}
