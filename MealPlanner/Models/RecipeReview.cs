namespace MealPlanner.Models
{
    public class RecipeReview
    {
        public int Id { get; set; }
        public int RecipeId { get; set; }
        public string UserId { get; set; }
        public int Rating { get; set; }
        public string ReviewText { get; set; }

        public RecipeReview()
        {
            
        }
    }

}
