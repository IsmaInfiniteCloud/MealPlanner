using MealPlanner.Models;
using Microsoft.Azure.WebJobs;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace MealPlanner.Services
{
    public class RecipePdfConverter : IConverter<Recipe, byte[]>
    {
        public byte[] Convert(Recipe recipe)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (Document document = new Document())
                {
                    PdfWriter.GetInstance(document, memoryStream);
                    document.Open();

                    // Add recipe content to the PDF document
                    document.Add(new Paragraph(recipe.Name));
                    document.Add(new Paragraph(recipe.Instructions));

                    // Add more content as needed

                    document.Close();
                }

                return memoryStream.ToArray();
            }
        }
    }
}
