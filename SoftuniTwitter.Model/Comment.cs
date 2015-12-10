using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftuniTwitter.Model
{
   public class Comment
   {
       public int CommentId { get; set; }
       public string Content { get; set; }
       public int ApplicationUserId { get; set; }
       public virtual ApplicationUser ApplicationUser { get; set; }
       public int PostId { get; set; }
       public virtual Post Post { get; set; }
   }
}
