using DAJDAJ.DataAccess;
using DAJDAJ.Entities;
using DAJDAJ.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DAJDAJ.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = ("Admin"))]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var cliam = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            string userid = cliam.Value;
            return View(_context.ApplicationUsers.Where(x => x.Id != userid).ToList());
        }

        public IActionResult LockUnlock(string? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var user = _context.ApplicationUsers.FirstOrDefault(x => x.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            if (user.LockoutEnd == null || user.LockoutEnd <= DateTime.Now)
            {
                // Lock the user for 1 year
                user.LockoutEnd = DateTime.Now.AddYears(1);
            }
            else
            {
                // Unlock the user
                user.LockoutEnd = DateTime.Now;
            }

            _context.SaveChanges();
            return RedirectToAction("Index", "Users", new { area = "Admin" });
        }
    }

    }
 