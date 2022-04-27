using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UKHO.MaritimeSafetyInformation.Common;
using UKHO.MaritimeSafetyInformation.Common.Models.DTO;

namespace UKHO.MaritimeSafetyInformation.Web.Controllers
{
    public class RadioNavigationalWarningsController : Controller
    {
        private readonly RadioNavigationalWarningsContext _context;

        public RadioNavigationalWarningsController(RadioNavigationalWarningsContext context)
        {
            _context = context;
        }

        // GET: RadioNavigationalWarnings
        public async Task<IActionResult> Index()
        {
            return View(await _context.RadioNavigationalWarnings.ToListAsync());
        }

        // GET: RadioNavigationalWarnings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var radioNavigationalWarnings = await _context.RadioNavigationalWarnings
                .FirstOrDefaultAsync(m => m.Id == id);
            if (radioNavigationalWarnings == null)
            {
                return NotFound();
            }

            return View(radioNavigationalWarnings);
        }

        // GET: RadioNavigationalWarnings/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: RadioNavigationalWarnings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,WarningType,Reference,DateTimeGroup,Summary,Content,ExpiryDate,IsDeleted")] RadioNavigationalWarnings radioNavigationalWarnings)
        {
            if (ModelState.IsValid)
            {
                _context.Add(radioNavigationalWarnings);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(radioNavigationalWarnings);
        }

        // GET: RadioNavigationalWarnings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var radioNavigationalWarnings = await _context.RadioNavigationalWarnings.FindAsync(id);
            if (radioNavigationalWarnings == null)
            {
                return NotFound();
            }
            return View(radioNavigationalWarnings);
        }

        // POST: RadioNavigationalWarnings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,WarningType,Reference,DateTimeGroup,Summary,Content,ExpiryDate,IsDeleted")] RadioNavigationalWarnings radioNavigationalWarnings)
        {
            if (id != radioNavigationalWarnings.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(radioNavigationalWarnings);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RadioNavigationalWarningsExists(radioNavigationalWarnings.Id))
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
            return View(radioNavigationalWarnings);
        }

        // GET: RadioNavigationalWarnings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var radioNavigationalWarnings = await _context.RadioNavigationalWarnings
                .FirstOrDefaultAsync(m => m.Id == id);
            if (radioNavigationalWarnings == null)
            {
                return NotFound();
            }

            return View(radioNavigationalWarnings);
        }

        // POST: RadioNavigationalWarnings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var radioNavigationalWarnings = await _context.RadioNavigationalWarnings.FindAsync(id);
            _context.RadioNavigationalWarnings.Remove(radioNavigationalWarnings);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RadioNavigationalWarningsExists(int id)
        {
            return _context.RadioNavigationalWarnings.Any(e => e.Id == id);
        }
    }
}
