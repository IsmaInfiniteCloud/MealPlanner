namespace MealPlanner.Models
{
    public class Recipe
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Instructions { get; set; }
        public List<Ingredient> Ingredients { get; set; }

    }
}
