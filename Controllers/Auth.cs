using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Project.Controllers
{
    public class Auth : Controller
    {
        public ActionResult SignIn()
        {
            return View();
        }

        public ActionResult SignUp(int id)
        {
            return View();
        }

        // POST: Auth/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Auth/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Auth/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Auth/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Auth/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
