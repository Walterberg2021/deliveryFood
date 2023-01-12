using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pizza.DataAccess;
using Pizza.DataAccess.Repository.IRepository;
using Pizza.Models;
using Pizza.Utility;
using System.Data;

namespace deliveryFood.Areas.Admin.Controllers;
[Area("Admin")]
[Authorize(Roles = SD.Role_Admin)]

public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Category> objCategoryList = _unitOfWork.Category.GetAll();

            return View(objCategoryList);
        }

        public IActionResult Create() //get
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category obj) //post
        {
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "Шось не так з ім'ям");
            }
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Add(obj);
                _unitOfWork.Save();
                TempData["Done"] = "Category created";
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        //****************************edit

        public IActionResult Edit(int? id) //get
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            //var categoryFromDb = _db.Categories.Find(id);
            var categoryFromDb = _unitOfWork.Category.GetFirstOrDefault(u => u.id == id);
            if (categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category obj) //post
        {
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "Шось не так з ім'ям");
            }
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Update(obj);
                _unitOfWork.Save();
                TempData["Done"] = "Category updated";
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        //****************************delete

        public IActionResult Delete(int? id) //get
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            //var categoryFromDb = _db.Categories.Find(id);
            var categoryFromDb = _unitOfWork.Category.GetFirstOrDefault(u => u.id == id);
            if (categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(int? id) //post
        {
            var obj = _unitOfWork.Category.GetFirstOrDefault(u => u.id == id);
            if (obj == null)
            {
                return NotFound();
            }

            _unitOfWork.Category.Remove(obj);
            _unitOfWork.Save();
            TempData["Done"] = "Category deleted";
            return RedirectToAction("Index");
        }
    }


