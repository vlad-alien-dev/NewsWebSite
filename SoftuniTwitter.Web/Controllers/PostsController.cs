using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Configuration;
using SoftuniTwitter.Model;
using SoftuniTwitter.Data;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SoftuniTwitter.Model.Enums;
using System.IO;

namespace SoftuniTwitter.Web.Controllers
{
    public class PostsController : Controller
    {
        private SoftuniTwitterDbContext db = new SoftuniTwitterDbContext();

        // GET: /Posts/
        public ActionResult Index()
        {
            var posts = db.Posts.Take(3).OrderByDescending(x=>x.PostId).ToList();
            foreach (var item in posts)
            {
                item.ApplicationUser = db.Users.Single(u => u.ApplicationUserId==item.ApplicationUserId);
            }
            return View(posts);
        }

        // GET: /Posts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            post.ApplicationUser = db.Users.Single(u => u.ApplicationUserId == post.ApplicationUserId);
            var filePaths = db.FilePaths.Where(x => x.PostId == post.PostId).Where(x => (int)x.FileType == 1).FirstOrDefault();
            if (filePaths!=null)
            {
               ViewBag.FilePath = WebConfigurationManager.AppSettings["ImagesStorage"]+filePaths.FileName;
            }
            ViewBag.PostId = post.PostId;
            return View(post);
        }

        // GET: /Posts/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Posts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PostId,Name,ApplicationUserId")] Post post, HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {
                var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(SoftuniTwitterDbContext.Create()));
                var currentUser = manager.FindById(User.Identity.GetUserId());
                post.ApplicationUserId = currentUser.ApplicationUserId;
                if (upload != null && upload.ContentLength > 0)
                {
                    var postImage = new FilePath
                    {
                        FileName = System.IO.Path.GetFileName(upload.FileName),
                        FileType = FileType.PostImage,
                        PostId = post.PostId
                    };
                    var fileExtension = postImage.FileName.Substring(postImage.FileName.IndexOf("."));
                    if (fileExtension==".jpg"||fileExtension==".jpeg"||fileExtension==".png"||fileExtension==".gif"||fileExtension==".bmp")
                    {
                        string path = Path.Combine(Server.MapPath(System.Web.Configuration.WebConfigurationManager.AppSettings["ImagesStorage"]),
                   Path.GetFileName(upload.FileName));
                        upload.SaveAs(path);
                        post.FilePaths = new List<FilePath>();
                        post.FilePaths.Add(postImage);
                    }
                }
                db.Posts.Add(post);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(post);
        }

        // GET: /Posts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        // POST: /Posts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="PostId,Name,Content,ApplicationUserId")] Post post)
        {
            if (ModelState.IsValid)
            {
                db.Entry(post).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(post);
        }

        // GET: /Posts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        // POST: /Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Post post = db.Posts.Find(id);
            db.Posts.Remove(post);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // POST: /Posts/Like/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.


        public JsonResult Like([Bind(Include = "PostId,Name,Content,ApplicationUserId")] int id, int like)
        {

                Post post = db.Posts.Single(x => x.PostId == id);
                post.Likes += like;
                if (post.Likes<1)
                {
                    post.Likes = 0;
                }
                db.SaveChanges();

                return Json(post.Likes);
        }

        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
