using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using DotNetCoreCalendar.Data;
using DotNetCoreCalendar.Models;

namespace DotNetCoreCalendar.Controllers
{
    [Authorize]
    public class LocationController : Controller
    {
        private readonly IDAL _dal;
        private readonly UserManager<ApplicationUser> _usermanager;

        public LocationController(IDAL idal, UserManager<ApplicationUser> usermanager)
        {
            _dal = idal;
            _usermanager = usermanager;
        }

        // GET: Location
        public IActionResult Index()
        {
            if (TempData["Alert"] != null)
                ViewData["Alert"] = TempData["Alert"];
            return View(_dal.GetLocations());
        }

        // GET: Location/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null) return NotFound();
            var location = _dal.GetLocation(id.Value);
            if (location == null) return NotFound();
            return View(location);
        }

        // GET: Location/Create
        public IActionResult Create() => View();

        // POST: Location/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Location location)
        {
            if (!ModelState.IsValid) return View(location);

            try
            {
                _dal.CreateLocation(location);
                TempData["Alert"] = "Success! You created a location for: " + location.Name;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewData["Alert"] = "An error occurred: " + ex.Message;
                return View(location);
            }
        }

        // ===== EDIT =====
        // GET: Location/Edit/5
        public IActionResult Edit(int? id, string returnUrl = null)
        {
            if (id == null) return NotFound();
            var location = _dal.GetLocation(id.Value);
            if (location == null) return NotFound();
            ViewData["ReturnUrl"] = returnUrl;
            return View(location); // Views/Location/Edit.cshtml
        }

        // POST: Location/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,Name")] Location location, string returnUrl = null)
        {
            if (id != location.Id) return NotFound();
            if (!ModelState.IsValid) return View(location);

            try
            {
                _dal.UpdateLocation(location); // must exist in DAL
                TempData["Alert"] = $"Saved changes to “{location.Name}”.";
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewData["Alert"] = "An error occurred: " + ex.Message;
                return View(location);
            }
        }

        // ===== DELETE (optional but handy) =====
        // GET: Location/Delete/5
        public IActionResult Delete(int? id, string returnUrl = null)
        {
            if (id == null) return NotFound();
            var location = _dal.GetLocation(id.Value);
            if (location == null) return NotFound();
            ViewData["ReturnUrl"] = returnUrl;
            return View(location);
        }

        // POST: Location/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id, string returnUrl = null)
        {
            try
            {
                _dal.DeleteLocation(id); // must exist in DAL
                TempData["Alert"] = "Location deleted.";
            }
            catch (Exception ex)
            {
                TempData["Alert"] = "An error occurred: " + ex.Message;
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return RedirectToAction(nameof(Index));
        }
    }
}


