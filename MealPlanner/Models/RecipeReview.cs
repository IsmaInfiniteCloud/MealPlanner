namespace MealPlanner.Models
{
    public class RecipeViewModel
    {
        public Recipe Recipe { get; set; }
        public List<RecipeReview> Reviews { get; set; }
    }

    public class RecipeReview
    {
        public int Id { get; set; }
        public int RecipeId { get; set; }
        public string UserId { get; set; }
        public int Rating { get; set; }
        public string ReviewText { get; set; }
    }
}
