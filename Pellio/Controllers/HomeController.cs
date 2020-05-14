namespace Pellio.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    
    /// <summary>
    /// Controls About and Contacts and Faq pages.
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// Redirects to about page.
        /// </summary>
        /// <returns>Returns view with about.</returns>
        public IActionResult About()
        {
            return View();
        }

        /// <summary>
        /// Redirects to contacts page.
        /// </summary>
        /// <returns>Returns view with contacts.</returns>
        public IActionResult Contacts()
        {
            return View();
        }

        /// <summary>
        /// Redirects to faq page.
        /// </summary>
        /// <returns>Returns view with faq.</returns>
        public IActionResult Faq()
        {
            return View();
        }
    }
}