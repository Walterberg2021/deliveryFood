using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Pizza.DataAccess;
using Pizza.DataAccess.Repository.IRepository;
using Pizza.Models;
using Pizza.Models.ViewModels;
using Pizza.Utility;
using System.Data;

namespace deliveryFood.Areas.Admin.Controllers;
[Area("Admin")]
[Authorize(Roles = SD.Role_Admin)]
public class ProductController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _hostEnvironment;

    public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
    {
        _unitOfWork = unitOfWork;
        _hostEnvironment = hostEnvironment;
    }

    public IActionResult Index()
    {
        return View();
    }



    //****************************edit

    public IActionResult Upsert(int? id) //get
    {
        ProductVM productVM = new()
        {
            product = new(),
            CategoryList = _unitOfWork.Category.GetAll().Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.id.ToString()
            })
        };
        if (id == null || id == 0)
        {
            //create
            //ViewBag.CategoryList = CategoryList;
            return View(productVM);
        }
        else
        {
            productVM.product=_unitOfWork.Product.GetFirstOrDefault(u => u.Id == id);
            return View(productVM);
            //updare
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Upsert(ProductVM obj, IFormFile? file) //post
    {
    
        if (ModelState.IsValid)
        {
            string wwwRootPath = _hostEnvironment.WebRootPath;
            if (file != null)
            {
                string fileName = Guid.NewGuid().ToString();
                var uploads = Path.Combine(wwwRootPath, @"Images\Product");
                var extension = Path.GetExtension(file.FileName);

                if(obj.product.imgurl!= null)
                {
                    var oldImagePath = Path.Combine(wwwRootPath,obj.product.imgurl.TrimStart('\\'));
                    if(System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                using (var FileStreams = new FileStream(Path.Combine(uploads, fileName+extension), FileMode.Create))
                {
                    file.CopyTo(FileStreams);
                }
                obj.product.imgurl = @"\Images\Product\" + fileName + extension;
            }

            if (obj.product.Id == 0)
            {
                _unitOfWork.Product.Add(obj.product);
            }
            else
            {
                _unitOfWork.Product.Update(obj.product);
            }
            _unitOfWork.Save();
            TempData["Done"] = "Product added";
            return RedirectToAction("Index");
        }
        return View(obj);
    }



    #region API CALLS
    [HttpGet]
    public IActionResult GetAll() 
    { 
        var productList = _unitOfWork.Product.GetAll(includeProperties:"Category");
        return Json(new { data = productList });
    }

    [HttpDelete]
    public IActionResult Delete(int? id) //post
    {
        var obj = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == id);
        if (obj == null)
        {
            return Json(new { success = false, message = "Error while deleting" });
        }

        var oldImagePath = Path.Combine(_hostEnvironment.WebRootPath , obj.imgurl.TrimStart('\\'));
        if (System.IO.File.Exists(oldImagePath))
        {
            System.IO.File.Delete(oldImagePath);
        }

        _unitOfWork.Product.Remove(obj);
        _unitOfWork.Save();
        return Json(new { success = true, message = "Delete successful" });
    }

    #endregion
}


