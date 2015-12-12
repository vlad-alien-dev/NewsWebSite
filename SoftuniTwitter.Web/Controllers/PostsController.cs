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
using System.Drawing;
using System.Drawing.Imaging;
using SoftuniTwitter.Web.ViewModels;

namespace SoftuniTwitter.Web.Controllers
{
    public class PostsController : Controller
    {
        private SoftuniTwitterDbContext db = new SoftuniTwitterDbContext();

        // GET: /Posts/
        public ActionResult Index()
        {

            var posts = (from p in db.Posts
                         join fp in db.FilePaths
                         on p.PostId equals fp.PostId
                         select new
                         {
                             PostId = p.PostId,
                             PostName = p.Name,
                             PostContent = p.Content,
                             Picture = fp.FileName,
                             ApplicationUserId = p.ApplicationUserId
                         }).ToList();
            var postsList = new List<PostIndexViewModel>();
            foreach (var item in posts)
            {
                var postsViewModel = new PostIndexViewModel();
                postsViewModel.PostId = item.PostId;
                postsViewModel.Name = item.PostName;
                postsViewModel.Content = item.PostContent;
                postsViewModel.FilePath = String.Format("{0}{1}{2}", WebConfigurationManager.AppSettings["ImagesStorage"], 
                                                                     item.Picture.Substring(0, item.Picture.LastIndexOf(".")),
                                                                     "-thumbnail" + item.Picture.Substring(item.Picture.LastIndexOf(".")));
                postsViewModel.ApplicationUser = db.Users.Single(u => u.ApplicationUserId == item.ApplicationUserId);
                postsList.Add(postsViewModel);
            }
            return View(postsList);
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
            if (filePaths != null)
            {
                ViewBag.FilePath = WebConfigurationManager.AppSettings["ImagesStorage"] + filePaths.FileName;
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
                    var fileExtension = postImage.FileName.Substring(postImage.FileName.LastIndexOf("."));
                    if (fileExtension == ".jpg" || fileExtension == ".jpeg" || fileExtension == ".png" || fileExtension == ".gif" || fileExtension == ".bmp")
                    {


                        string path = Path.Combine(Server.MapPath(System.Web.Configuration.WebConfigurationManager.AppSettings["ImagesStorage"]),
                   Path.GetFileName(upload.FileName));
                        upload.SaveAs(path);
                        Image imgOriginal = Image.FromFile(path);
                        Image imgResized = new Bitmap(imgOriginal, 200, 100);
                        imgOriginal.Dispose();
                        var imgThumbnail = path.Substring(0, path.LastIndexOf(".")) + "-thumbnail" + fileExtension;
                        imgResized.Save(imgThumbnail);
                        imgResized.Dispose();
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
        public ActionResult Edit([Bind(Include = "PostId,Name,Content,ApplicationUserId")] Post post)
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
            if (post.Likes < 1)
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
