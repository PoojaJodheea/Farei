using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FormRequest.Data;
using FormRequest.Models;

namespace FormRequest.Controllers
{
    public class FormReqDbsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FormReqDbsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FormReqDbs
        public async Task<IActionResult> Index()
        {
            return View(await _context.FormReqDb.ToListAsync());
        }

        // GET: FormReqDbs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var formReqDb = await _context.FormReqDb
                .FirstOrDefaultAsync(m => m.Id == id);
            if (formReqDb == null)
            {
                return NotFound();
            }

            return View(formReqDb);
        }

        // GET: FormReqDbs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: FormReqDbs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,RequestDate,Site,Department,ResponsibleOfficer,ContactPhone,EquipmentType,ProblemDescription,SerialNumber,From,To,MovementDate,Remarks,IsApproved,IsInvalid")] FormReqDb formReqDb)
        {
            if (ModelState.IsValid)
            {
                _context.Add(formReqDb);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(formReqDb);
        }

        // GET: FormReqDbs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var formReqDb = await _context.FormReqDb.FindAsync(id);
            if (formReqDb == null)
            {
                return NotFound();
            }
            return View(formReqDb);
        }

        // POST: FormReqDbs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,RequestDate,Site,Department,ResponsibleOfficer,ContactPhone,EquipmentType,ProblemDescription,SerialNumber,From,To,MovementDate,Remarks,IsApproved,IsInvalid")] FormReqDb formReqDb)
        {
            if (id != formReqDb.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(formReqDb);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FormReqDbExists(formReqDb.Id))
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
            return View(formReqDb);
        }

        // GET: FormReqDbs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var formReqDb = await _context.FormReqDb
                .FirstOrDefaultAsync(m => m.Id == id);
            if (formReqDb == null)
            {
                return NotFound();
            }

            return View(formReqDb);
        }

        // POST: FormReqDbs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var formReqDb = await _context.FormReqDb.FindAsync(id);
            if (formReqDb != null)
            {
                _context.FormReqDb.Remove(formReqDb);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FormReqDbExists(int id)
        {
            return _context.FormReqDb.Any(e => e.Id == id);
        }
    }
}
