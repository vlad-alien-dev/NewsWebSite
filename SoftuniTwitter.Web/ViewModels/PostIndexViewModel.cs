using SoftuniTwitter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftuniTwitter.Web.ViewModels
{
    public class PostIndexViewModel
    {
        public int PostId { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public int Likes { get; set; }
        public int ApplicationUserId { get; set; }
        public IEnumerable<Post> Posts { get; set; }

        public string FilePath { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}