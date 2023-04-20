using System;

namespace MealPlanner.Models
{
    public class RecipeViewModel
    {
        // pour accéder et définir la recette
        public Recipe Recipe { get; set; }

    // pour accéder et définir la liste des avis sur la recette
    public List<RecipeReview> Reviews { get; set; }
    }

    public class RecipeReview
    {
        public int Id { get; set; }
        // Identifiant unique de la recette liée à l'avis
        public int RecipeId { get; set; }
        public string UserId { get; set; }
        public int Rating { get; set; }
        public string ReviewText { get; set; }

        public RecipeReview()
        {
            
        }
    }
}
