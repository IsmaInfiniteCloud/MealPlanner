namespace MealPlanner.Models
{
    public class Ingredient
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public string Unit { get; set; }

        public Ingredient()
        {
            
        }
    }
}