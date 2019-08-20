using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Uplift.DataAccess.Data;
using Uplift.Models;

namespace Uplift.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class WebImageController : Controller
    {
        // for this controller access the context directly NOT using the repo pattern with the UoW
        // just for reference only as the repo pattern has been used up to this point
        private readonly ApplicationDbContext _db;

        public WebImageController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            WebImages imageObj = new WebImages();
            if (id == null)
            {

            }
            else
            {
                imageObj = _db.WebImages.SingleOrDefault(m => m.Id == id);
                if (imageObj == null)
                {
                    return NotFound();
                }
            }
            return View(imageObj);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(int id, WebImages imageObj)
        {
            if (ModelState.IsValid)
            {
                // get the files uploaded with the post data
                var files = HttpContext.Request.Form.Files;

                // make sure there is at least one file
                if (files.Count > 0)
                {
                    byte[] p1 = null;
                    using (var fs1 = files[0].OpenReadStream())
                    {
                        using (var ms1 = new MemoryStream())
                        {
                            fs1.CopyTo(ms1);
                            p1 = ms1.ToArray();
                        }
                    }
                    imageObj.Picture = p1;
                }

                if (imageObj.Id == 0)
                {
                    // add a new image
                    _db.WebImages.Add(imageObj);
                }
                else
                {
                    // update an existing image
                    var imageFromDb = _db.WebImages.Where(i => i.Id == id).FirstOrDefault();

                    imageFromDb.Name = imageObj.Name;
                    if (files.Count > 0)
                    {
                        imageFromDb.Picture = imageObj.Picture;
                    }
                }
                _db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(imageObj);
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            return Json(new { data = _db.WebImages.ToList() });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _db.WebImages.Find(id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error occurred while deleting the image." });
            }

            _db.WebImages.Remove(objFromDb);
            _db.SaveChanges();
            return Json(new { success = true, message = "Image deleted successfully." });
        }
        #endregion
    }
}