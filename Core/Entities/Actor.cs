﻿
using Newtonsoft.Json;

namespace Core.Entities
{
    public class Actor
    {
        [JsonProperty("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string FullName { get; set; }

        public string Biography { get; set; }
        public string MovieId { get; set; }

        //  public List<FileEntity>? Image { get; set; } = new List<FileEntity>();
        public string? ImageFile { get; set; }


    }
}