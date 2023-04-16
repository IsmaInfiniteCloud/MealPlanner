using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MealPlanner.Data;
using MealPlanner.Models;

namespace MealPlanner.Controllers
{
    public class MealPlansController : Controller
    {
        private readonly MealPlannerContext _context;

        public MealPlansController(MealPlannerContext context)
        {
            _context = context;
        }

        // GET: MealPlans
        public async Task<IActionResult> Index()
        {
              return _context.MealPlan != null ? 
                          View(await _context.MealPlan.ToListAsync()) :
                          Problem("Entity set 'MealPlannerContext.MealPlan'  is null.");
        }

        // GET: MealPlans/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.MealPlan == null)
            {
                return NotFound();
            }

            var mealPlan = await _context.MealPlan
                .FirstOrDefaultAsync(m => m.Id == id);
            if (mealPlan == null)
            {
                return NotFound();
            }

            return View(mealPlan);
        }

        // GET: MealPlans/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MealPlans/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,StartDate,EndDate")] MealPlan mealPlan)
        {
            if (ModelState.IsValid)
            {
                _context.Add(mealPlan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(mealPlan);
        }

        // GET: MealPlans/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.MealPlan == null)
            {
                return NotFound();
            }

            var mealPlan = await _context.MealPlan.FindAsync(id);
            if (mealPlan == null)
            {
                return NotFound();
            }
            return View(mealPlan);
        }

        // POST: MealPlans/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StartDate,EndDate")] MealPlan mealPlan)
        {
            if (id != mealPlan.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(mealPlan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MealPlanExists(mealPlan.Id))
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
            return View(mealPlan);
        }

        // GET: MealPlans/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.MealPlan == null)
            {
                return NotFound();
            }

            var mealPlan = await _context.MealPlan
                .FirstOrDefaultAsync(m => m.Id == id);
            if (mealPlan == null)
            {
                return NotFound();
            }

            return View(mealPlan);
        }

        // POST: MealPlans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.MealPlan == null)
            {
                return Problem("Entity set 'MealPlannerContext.MealPlan'  is null.");
            }
            var mealPlan = await _context.MealPlan.FindAsync(id);
            if (mealPlan != null)
            {
                _context.MealPlan.Remove(mealPlan);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: MealPlans/Search
        public async Task<IActionResult> Search(DateTime searchDate)
        {
            var mealPlans = await _context.MealPlan
                .Include(mp => mp.Meals)
                .Where(mp => mp.StartDate <= searchDate && mp.EndDate >= searchDate)
                .ToListAsync();

            return View(mealPlans);
        }


        private bool MealPlanExists(int id)
        {
          return (_context.MealPlan?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
