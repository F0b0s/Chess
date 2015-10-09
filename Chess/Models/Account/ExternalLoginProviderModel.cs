using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Chess.Models.Account
{
    public class ExternalLoginProviderModel
    {
        //[JsonProperty("name", Required = Required.Always)]
        public string Provider { get; set; }

        //[JsonProperty("url", Required = Required.Always)]
        public string Url { get; set; }

        //[JsonProperty("state", Required = Required.Always)]
        public string State { get; set; }
    }
}