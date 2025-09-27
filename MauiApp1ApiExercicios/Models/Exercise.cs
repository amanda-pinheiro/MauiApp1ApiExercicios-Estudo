using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MauiApp1ApiExercicios.Models
{
    public class Exercise
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("target")]
        public string Target { get; set; }

        [JsonProperty("bodyPart")]
        public string BodyPart { get; set; }

        [JsonProperty("equipment")]
        public string Equipment { get; set; }

        [JsonProperty("gifUrl")]
        public string GifUrl { get; set; }

        // Propriedade para facilitar a exibição na lista
        public string DisplayText => $"{Name} - {Target}";
    }
}
