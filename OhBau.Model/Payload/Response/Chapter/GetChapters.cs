﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhBau.Model.Payload.Response.Chapter
{
    public class GetChapters
    {
        public Guid Id { get; set; }

        public string Image {  get; set; }
        public string Title {  get; set; }
        public string Content {  get; set; }
    }
}
