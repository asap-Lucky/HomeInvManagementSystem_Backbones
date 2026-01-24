using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Runtime.CompilerServices;

namespace Domain.Wrapper
{
    public class BaseRequestWrapper<T> where T : class
    {
        [JsonProperty("requestid")]
        public string RequestId { get; set; } = Guid.NewGuid().ToString();

        [JsonProperty("requestetimestamp")]
        public string RequestTimeStamp { get; set; } = DateTime.Now.ToString("o");

        [JsonProperty("dataset")]
        public T? Dataset { get; set; }
    }
}
