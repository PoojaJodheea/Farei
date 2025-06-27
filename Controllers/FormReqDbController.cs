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

            var viewModel = new RequestViewModel
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
        public async Task<IActionResult> Create(FormReqDb model)
        {
            if (ModelState.IsValid)
            {
                _context.FormReqDb.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(model); 
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
            var requests = _context.FormReqDb
                .Include(f => f.Registries)
                .Where(f =>
                    !f.Registries.Any(r =>
                        r.IsValid && (r.IsOnSite || r.IsInTransit)
                    )
                )
                .ToList();

            return View(requests);
        }



        public IActionResult RegistryDetails(int id)
        {
            var formReq = _context.FormReqDb
                .Include(f => f.Registries)
                .FirstOrDefault(x => x.Id == id);

            if (formReq == null) return NotFound();

            var viewModel = new RequestViewModel
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
        public IActionResult SubmitRegistry(RequestViewModel model)
        {
            ModelState.Remove("Registry.FormReqDb");

            if (!ModelState.IsValid)
            {
                model.FormReqDb = _context.FormReqDb

                    .Include(f => f.Registries)
                    .FirstOrDefault(f => f.Id == model.Registry.FormReqDbId);

                model.RegistryList = model.FormReqDb?.Registries.ToList();
               

                return View("RegistryDetails", model);
            }

            //Adding registry
            model.Registry.MovementDate = DateTime.Now.Date;//local date only
            _context.Registry.Add(model.Registry);
            _context.SaveChanges();
         
            return RedirectToAction("RegistryDetails", new { id = model.Registry.FormReqDbId });
        }

      
        public IActionResult OnSite()
        {
            var forms = _context.FormReqDb
                .Include(f => f.Registries)
                .Where(f => f.Registries.Any(r => ( r.IsOnSite)))
                .ToList();
            return View("IsOnSite", forms);
        }

        public IActionResult InTransit()
        {
            var forms = _context.FormReqDb
                .Include(f => f.Registries)
                .Where(f => f.Registries.Any(r => ( r.IsInTransit)))
                .ToList();
            return View("IsInTransit", forms);
        }
        //acknowlege movement was successful
        [HttpPost]
        public IActionResult AcknowledgeRequest(int id)
        {
            var registry = _context.Registry.FirstOrDefault(r => r.RegistryId == id);
            if (registry != null)
            {
                registry.IsValid = true;
                _context.SaveChanges();
            }

            return RedirectToAction("IsOnSite");
        }



        //Delete request after acknowlegdement

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteRegistry(int registryId)
        {
            var registry = _context.Registry.FirstOrDefault(r => r.RegistryId == registryId);

            if (registry != null)
            {
                _context.Registry.Remove(registry);
                _context.SaveChanges();
            }

            return RedirectToAction("IsOnSite");
        }





    }
}
