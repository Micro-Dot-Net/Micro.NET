using System.Text;
using System.Threading.Tasks;
using Micro.Net.Abstractions;
using Newtonsoft.Json;

namespace Micro.Net.Serializing
{
    public class JsonSerializer : ISerializer
    {
        private readonly JsonSerializerSettings _settings;

        public JsonSerializer(JsonSerializerSettings settings)
        {
            _settings = settings;
        }

        public TValue Materialize<TValue>(string value)
        {
            return JsonConvert.DeserializeObject<TValue>(value, _settings);
        }

        public string Serialize<TValue>(TValue value)
        {
            return JsonConvert.SerializeObject(value, _settings);
        }
    }
}
