using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftuniTwitter.Model
{
    public class Post
    {
        public Post()
        {
            this.Comments = new HashSet<Comment>();
        }
        public int PostId { get; set; }
        [Required]
        public string Name { get; set; }

        public string Content { get; set; }
        public int Likes { get; set; }
        public int ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<FilePath> FilePaths { get; set; }
    }
}