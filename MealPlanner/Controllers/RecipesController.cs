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
    public class RecipesController : Controller
    {
        private readonly MealPlannerContext _context;

        public RecipesController(MealPlannerContext context)
        {
            _context = context;
        }

        // GET: Recipes
        public async Task<IActionResult> Index()
        {
              return _context.Recipe != null ? 
                          View(await _context.Recipe.ToListAsync()) :
                          Problem("Entity set 'MealPlannerContext.Recipe'  is null.");
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Recipe == null)
            {
                return NotFound();
            }

            var recipe = await _context.Recipe.FirstOrDefaultAsync(m => m.Id == id);
            if (recipe == null)
            {
                return NotFound();
            }

            var reviews = await _context.RecipeReview
                .Where(r => r.RecipeId == id)
                .ToListAsync();

            var viewModel = new RecipeViewModel
            {
                Recipe = recipe,
                Reviews = reviews
            };

            return View(viewModel);
        }


        // GET: Recipes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Recipes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Instructions")] Recipe recipe)
        {
            if (ModelState.IsValid)
            {
                _context.Add(recipe);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(recipe);
        }

        // GET: Recipes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Recipe == null)
            {
                return NotFound();
            }

            var recipe = await _context.Recipe.FindAsync(id);
            if (recipe == null)
            {
                return NotFound();
            }
            return View(recipe);
        }

        // POST: Recipes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Instructions")] Recipe recipe)
        {
            if (id != recipe.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(recipe);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RecipeExists(recipe.Id))
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
            return View(recipe);
        }

        // GET: Recipes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Recipe == null)
            {
                return NotFound();
            }

            var recipe = await _context.Recipe
                .FirstOrDefaultAsync(m => m.Id == id);
            if (recipe == null)
            {
                return NotFound();
            }

            return View(recipe);
        }

        // POST: Recipes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Recipe == null)
            {
                return Problem("Entity set 'MealPlannerContext.Recipe'  is null.");
            }
            var recipe = await _context.Recipe.FindAsync(id);
            if (recipe != null)
            {
                _context.Recipe.Remove(recipe);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }



        [HttpPost]
        public async Task<IActionResult> SubmitReview(int recipeId, int rating, string reviewText)
        {
            try
            {
                var userId = User.Identity != null && User.Identity.IsAuthenticated ? User.Identity.Name : null;
                var review = new RecipeReview
                {
                    RecipeId = recipeId,
                    UserId = userId,
                    Rating = rating,
                    ReviewText = reviewText
                };
                _context.RecipeReview.Add(review);
                await _context.SaveChangesAsync();

                var recipe = await _context.Recipe.FindAsync(recipeId);
                if (recipe == null)
                {
                    return NotFound($"Recipe with id {recipeId} not found.");
                }
                var reviews = await _context.RecipeReview.Where(r => r.RecipeId == recipeId).ToListAsync();
                var viewModel = new RecipeViewModel { Recipe = recipe, Reviews = reviews };
                TempData["SuccessMessage"] = "Review submitted successfully!";

                return RedirectToAction("Details", new { id = recipe.Id });

            }
            catch (Exception ex)
            {
                return Problem("An error occurred while submitting the review.", null, 500, ex.Message);
            }
        }


        private bool RecipeExists(int id)
        {
            return (_context.Recipe?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
