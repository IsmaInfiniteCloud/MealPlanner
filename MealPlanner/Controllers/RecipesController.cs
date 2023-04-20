using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MealPlanner.Data;
using MealPlanner.Models;
using Microsoft.Data.SqlClient;
using System.Reflection.Metadata;
using Microsoft.AspNetCore.Hosting;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Document = iTextSharp.text.Document;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;
using Microsoft.CodeAnalysis.RulesetToEditorconfig;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Hosting;

namespace MealPlanner.Controllers
{

    public class RecipesController : Controller
    {
        private readonly MealPlannerContext _context;

        private readonly IWebHostEnvironment _env;

        private readonly IConverter<Recipe, byte[]> _converter;


        public RecipesController(MealPlannerContext context, IConverter<Recipe, byte[]> converter, IWebHostEnvironment env)
        {
            _context = context;
            _converter = converter;
            _env = env;
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
                TempData["SuccessMessage"] = "Recipe added successfully!";
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
                TempData["SuccessMessage"] = "Recipe updated successfully!";
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


                var sql = "EXEC dbo.AddRecipeReview @RecipeId, @UserId, @Rating, @ReviewText";
                await _context.Database.ExecuteSqlRawAsync(sql, new SqlParameter("@RecipeId", recipeId), new SqlParameter("@UserId", userId ?? (object)DBNull.Value), new SqlParameter("@Rating", rating), new SqlParameter("@ReviewText", reviewText));

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


        // GET: Recipes/RecipeSearchForm
        public async Task<IActionResult> RecipeSearchForm()
        {
            return View();
        }
        // GET: Meals/showSearchResults
        public async Task<IActionResult> showSearchResults(String SearchPhrase)
        {
            return View("Index", await _context.Recipe.Where(m => m.Name.Contains
                        (SearchPhrase)).ToListAsync());
        }



        private bool RecipeExists(int id)
        {
            return (_context.Recipe?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public async Task<ActionResult> GetInspired()
        {
            // Exécuter la procédure stockée "GetRandomRecipe" et obtenir le premier résultat (ou défaut)
            var randomRecipes = await _context.Recipe.FromSqlRaw("EXEC GetRandomRecipe").ToListAsync();
            var randomRecipe = randomRecipes.FirstOrDefault();

            // Si aucun résultat n'est trouvé, retourner une erreur 404 (non trouvé)
            if (randomRecipe == null)
            {
                return NotFound();
            }

            // Retourner la vue avec la recette aléatoire
            return View(randomRecipe);
        }


        // ...

        
        [HttpGet]
        public FileResult SaveRecipeToPDF(int recipeId)
        {
            Recipe recipe = GetRecipeById(recipeId);
            string pdfFileName = recipe.Name + ".pdf";

            string pdfDirectoryPath = Path.Combine(_env.WebRootPath, "pdfs"); // <-- Change made here
            if (!Directory.Exists(pdfDirectoryPath))
            {
                Directory.CreateDirectory(pdfDirectoryPath);
            }

            string pdfFilePath = Path.Combine(pdfDirectoryPath, pdfFileName);

            using (MemoryStream memoryStream = new MemoryStream())
            {
                Document document = new Document();
                PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);

                document.Open();

                Paragraph recipeName = new Paragraph(recipe.Name);
                document.Add(recipeName);

                Paragraph recipeInstructions = new Paragraph(recipe.Instructions);
                document.Add(recipeInstructions);

                if (recipe.Ingredients.Count > 0)
                {
                    PdfPTable table = new PdfPTable(2);
                    table.AddCell("Ingredient");
                    table.AddCell("Quantity");

                    // ...

                    document.Add(table);
                }

                document.Close();

                System.IO.File.WriteAllBytes(pdfFilePath, memoryStream.ToArray());
            }

            return File(System.IO.File.ReadAllBytes(pdfFilePath), "application/pdf", pdfFileName);
        }



        public Recipe GetRecipeById(int id)
        {
            return _context.Recipe.Where(x => x.Id == id).FirstOrDefault();
        }

        public ActionResult DisplayRecipeById(int id) // Renamed from GetRecipeById
        {
            var recipe = GetRecipeById(id); // Use GetRecipeById instead of _recipeRepository
            return View(recipe);
        }




        public async Task<ActionResult> SaveRecipeAsPdf(int id)
        {
            var recipe = GetRecipeById(id);
            if (recipe == null)
            {
                return NotFound();
            }

            byte[] pdfBytes = _converter.Convert(recipe);

            // Save the PDF file
            var pdfFileName = $"Recipe_{recipe.Id}.pdf";
            var pdfFilePath = Path.Combine(_env.WebRootPath, "pdfs", pdfFileName);
            await System.IO.File.WriteAllBytesAsync(pdfFilePath, pdfBytes);

            return Ok("Recipe saved as PDF.");
        }




    }
}

