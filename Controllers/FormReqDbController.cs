using FormRequest.Data;
using FormRequest.Models;
using FormRequest.ViewModel;
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
        [HttpGet]
        
        public async Task<IActionResult> Registry()
        {
            var viewModels = await _context.FormReqDb
                .Include(f => f.Registries)
                .Select(f => new RegistryViewModel
                {
                    FormReqDb = f,
                    RegistryList = f.Registries
                })
                .ToListAsync();

            return View(viewModels);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitRegistry(Registry registry)
        {
            if (ModelState.IsValid && registry.FormReqDbId != 0)
            {
                _context.Registry.Add(registry);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Registry));
            }

            // On failure, reload data and return view
            var viewModels = await _context.FormReqDb
                .Include(f => f.Registries)
                .Select(f => new RegistryViewModel
                {
                    FormReqDb = f,
                    RegistryList = f.Registries
                })
                .ToListAsync();

            return View("Registry", viewModels);
        }



    }
}
