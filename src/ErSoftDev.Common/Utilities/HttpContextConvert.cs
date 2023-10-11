using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Text;

namespace ErSoftDev.Common.Utilities
{
    public class HttpContentConvert : StringContent
    {
        public HttpContentConvert(object obj) :
            base(
                JsonConvert.SerializeObject(obj,
                    new JsonSerializerSettings()
                    {
                        ContractResolver =
                            new DefaultContractResolver() { NamingStrategy = new CamelCaseNamingStrategy() }
                    }), Encoding.UTF8, "application/json")
        {
        }
    }
}
