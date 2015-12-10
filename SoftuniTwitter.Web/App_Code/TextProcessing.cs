using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftuniTwitter.Web.App_Code
{
    public class TextProcessing
    {
        public static string Excerpt(string postContent)
        {
            if (postContent != null && postContent.Length>=101)
            {
                return String.Format(postContent.Substring(0, 100) + "...");
            }
            return postContent;
        }
        //Here you can customize the excerpt length
        public static string Excerpt(string postContent, int length)
        {
            if (postContent != null && postContent.Length >= length + 1)
            {
                return String.Format(postContent.Substring(0, length) + "...");
            }
            return postContent;
        }
    }
}