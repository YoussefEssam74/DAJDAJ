using DAJDAJ.DataAccess;
using DAJDAJ.DataAccess.Implementation;
using DAJDAJ.Entities.Models;
using DAJDAJ.Entities.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace DAJDAJ.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private IUntiOfWork _untiOfWork;
        public CategoryController(IUntiOfWork untiOfWork)
        {
            _untiOfWork = untiOfWork;
        }
        public IActionResult Index()
        {
            var categories = _untiOfWork.Category.GetAll();
            return View(categories);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                //_context.categories.Add(category);
                _untiOfWork.Category.Add(category);

                //_context.SaveChanges();
                _untiOfWork.Complete();
                TempData["Create"] = "Data Has Created Successfully";

                return RedirectToAction("Index");
            }
            return View(category);
        }
        [HttpGet]
        public IActionResult Edit(int?id)
        {
            if(id==null | id==0)
            {
                NotFound();

            }
            //var categoryIndb = _context.categories.Find(id);
            var categoryIndb = _untiOfWork.Category.GetFirstorDefault(x => x.Id == id);

            return View(categoryIndb);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
              //  _context.categories.Update(category);

                _untiOfWork.Category.Update(category);
                //  _context.SaveChanges();
                _untiOfWork.Complete();
                TempData["Update"] = "Data Has Updated Successfully";
                return RedirectToAction("Index");
            }
            return View(category);
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            //  var categoryIndb = _context.categories.Find(id);
            var categoryIndb = _untiOfWork.Category.GetFirstorDefault(x => x.Id == id);

            return View(categoryIndb);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult DeleteCategory(int? id)
        {
            //  var categoryIndb = _context.categories.Find(id);
            var categoryIndb = _untiOfWork.Category.GetFirstorDefault(x => x.Id == id);

            if (categoryIndb == null)
            {
                return NotFound();
            }

            //_context.categories.Remove(categoryIndb);
            _untiOfWork.Category.Remove(categoryIndb);
           //_context.SaveChanges();
           _untiOfWork.Complete();
            TempData["Delete"] = "Data Has Deleted Successfully";
            return RedirectToAction("Index");
        }

    }
}
