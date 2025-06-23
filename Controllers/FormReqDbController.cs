using FormRequest.Data;
using FormRequest.Models;
using FormRequest.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FormRequest.Controllers
{
    public class FormReqDbController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FormReqDbController(ApplicationDbContext context)
        {
            _context = context;
        }

        // === Home page ===
        public async Task<IActionResult> Index()
        {
            var forms = await _context.FormReqDb.ToListAsync();
            return View(forms);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var form = await _context.FormReqDb
                .Include(f => f.Registries)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (form == null) return NotFound();

            var viewModel = new RegistryViewModel
            {
                FormReqDb = form,
                RegistryList = form.Registries.ToList() 
            };

            return View(viewModel);
        }

        public IActionResult Create()
        {
            return View(new FormReqDb());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FormReqDb formReqDb)
        {
            if (ModelState.IsValid)
            {
                formReqDb.RequestDate = DateTime.Now;
                _context.FormReqDb.Add(formReqDb);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(formReqDb);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var form = await _context.FormReqDb.FindAsync(id);
            if (form == null) return NotFound();

            return View(form);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FormReqDb formReqDb)
        {
            if (id != formReqDb.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(formReqDb);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(formReqDb);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var form = await _context.FormReqDb.FirstOrDefaultAsync(m => m.Id == id);
            if (form == null) return NotFound();

            return View(form);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var form = await _context.FormReqDb.FindAsync(id);
            if (form != null)
            {
                _context.FormReqDb.Remove(form);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
        //registry


        public IActionResult Registry()
        {
            var requests = _context.FormReqDb.ToList();
            return View(requests); // Passes list of FormReqDb to view
        }

        public IActionResult RegistryDetails(int id)
        {
            var formReq = _context.FormReqDb
                .Include(f => f.Registries)
                .FirstOrDefault(x => x.Id == id);

            if (formReq == null) return NotFound();

            var viewModel = new RegistryViewModel
            {
                FormReqDb = formReq,
                Registry = new Registry
                {
                    FormReqDbId = id,
                    
                },
                RegistryList = formReq.Registries.ToList()
            };

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitRegistry(RegistryViewModel viewModel)
        {
            try
            {
                _context.Registry.Add(viewModel.Registry);
                _context.SaveChanges();
                Console.WriteLine("Registry saved successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine("DB error: " + ex.Message);
            }

            return RedirectToAction("Registry");
        }

        public IActionResult OnSite()
        {
            var forms = _context.FormReqDb
                .Include(f => f.Registries)
                .Where(f => f.Registries.Any(r => r.IsOnSite))
                .ToList();
            return View("Registry", forms);
        }

        public IActionResult InTransit()
        {
            var forms = _context.FormReqDb
                .Include(f => f.Registries)
                .Where(f => f.Registries.Any(r => r.IsInTransit))
                .ToList();
            return View("Registry", forms);
        }







    }
}
