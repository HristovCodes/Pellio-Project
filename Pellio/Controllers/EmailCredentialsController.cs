using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pellio.Data;
using Pellio.Models;

namespace Pellio.Controllers
{
    public class EmailCredentialsController : Controller
    {
        private readonly PellioContext _context;

        public EmailCredentialsController(PellioContext context)
        {
            _context = context;
        }

        // GET: EmailCredentials
        public async Task<IActionResult> Index()
        {
            return View(await _context.EmailCredentials.ToListAsync());
        }

        // GET: EmailCredentials/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var emailCredentials = await _context.EmailCredentials
                .FirstOrDefaultAsync(m => m.Id == id);
            if (emailCredentials == null)
            {
                return NotFound();
            }

            return View(emailCredentials);
        }

        // GET: EmailCredentials/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: EmailCredentials/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Email,Password")] EmailCredentials emailCredentials)
        {
            if (ModelState.IsValid)
            {
                _context.Add(emailCredentials);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(emailCredentials);
        }

        // GET: EmailCredentials/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var emailCredentials = await _context.EmailCredentials.FindAsync(id);
            if (emailCredentials == null)
            {
                return NotFound();
            }
            return View(emailCredentials);
        }

        // POST: EmailCredentials/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Email,Password")] EmailCredentials emailCredentials)
        {
            if (id != emailCredentials.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(emailCredentials);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmailCredentialsExists(emailCredentials.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(emailCredentials);
        }

        // GET: EmailCredentials/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var emailCredentials = await _context.EmailCredentials
                .FirstOrDefaultAsync(m => m.Id == id);
            if (emailCredentials == null)
            {
                return NotFound();
            }

            return View(emailCredentials);
        }

        // POST: EmailCredentials/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var emailCredentials = await _context.EmailCredentials.FindAsync(id);
            _context.EmailCredentials.Remove(emailCredentials);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmailCredentialsExists(int id)
        {
            return _context.EmailCredentials.Any(e => e.Id == id);
        }
    }
}
