﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Uplift.DataAccess.Data.Repository.IRepository;
using Uplift.Models;
using Uplift.Utility;

namespace Uplift.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }
               
        public IActionResult Upsert(int? id)
        {
            Category category = new Category();

            // if id is null then this is an insert
            if (id == null)
            {
                return View(category);
            }

            // get the persisted category
            category = _unitOfWork.Category.Get(id.GetValueOrDefault());
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Category category)
        {
            if (ModelState.IsValid)
            {
                if (category.Id == 0)
                {
                    _unitOfWork.Category.Add(category);
                }
                else
                {
                    _unitOfWork.Category.Update(category);
                }
                _unitOfWork.Save();

                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }

        #region API Calls
        [HttpGet]
        public IActionResult GetAll()
        {
            //return Json(new { data = _unitOfWork.Category.GetAll() });
            return Json(new { data = _unitOfWork.StoredProcedure_Call.ReturnList<Category>(SD.usp_GetAllCategory, null) });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var catFromDb = _unitOfWork.Category.Get(id);
            if (catFromDb == null)
            {
                return Json(new { success = false, message = "Error deleting category!" });
            }

            _unitOfWork.Category.Remove(catFromDb);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Category deleted successfully!" });
        }
        #endregion
    }
}