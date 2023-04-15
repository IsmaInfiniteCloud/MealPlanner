namespace MealPlanner.Models
{
    public class MealPlan
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<Meal> Meals { get; set; }
    }
}
