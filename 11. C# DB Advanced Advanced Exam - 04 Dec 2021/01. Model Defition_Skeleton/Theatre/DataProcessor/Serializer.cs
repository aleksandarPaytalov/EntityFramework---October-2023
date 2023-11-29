using Newtonsoft.Json;
using Theatre.DataProcessor.ExportDto;
using Theatre.Utilities;

namespace Theatre.DataProcessor
{

    using System;
    using System.Diagnostics;
    using System.Xml.Linq;
    using Theatre.Data;
    using Theatre.Data.Models;
    using Theatre.Data.Models.Enums;

    public class Serializer
    {
        public static string ExportTheatres(TheatreContext context, int numbersOfHalls)
        {
            var theatersJson = context.Theatres
                .ToArray()
                .Where(t => t.NumberOfHalls >= numbersOfHalls && t.Tickets.Count >= 20)
                .Select(t => new
                {
                    Name = t.Name,
                    Halls = t.NumberOfHalls,
                    TotalIncome = t.Tickets.Where(ti => ti.RowNumber >= 1 && ti.RowNumber <= 5).Sum(ti => ti.Price),
                    Tickets = t.Tickets.Where(ti => ti.RowNumber >= 1 && ti.RowNumber <= 5)
                        .Select(ti => new
                        {
                            Price = ti.Price,
                            RowNumber = ti.RowNumber
                        })
                        .OrderByDescending(ti => ti.Price)
                        .ToArray()
                })
                .OrderByDescending(t => t.Halls)
                .ThenBy(t => t.Name)
                .ToArray();

            return JsonConvert.SerializeObject(theatersJson, Formatting.Indented);
        }

        public static string ExportPlays(TheatreContext context, double raiting)
        {
            XmlHelper xmlHelper = new XmlHelper();

            var playsDtos = context.Plays
                .Where(p => p.Rating <= raiting)
                .AsEnumerable()
                .Select(p => new ExportPlaysDto()
                {
                    Title = p.Title,
                    Duration = p.Duration.ToString("c"),
                    Rating = p.Rating == 0 ? "Premier" : p.Rating.ToString(),
                    Genre = p.Genre.ToString(),
                    Actors = p.Casts
                        .Where(c => c.IsMainCharacter)
                        .Select(c => new ExportActorDto()
                        {
                            FullName = c.FullName,
                            MainCharacter = $"Plays main character in '{p.Title}'."
                        })
                        .OrderByDescending(c => c.FullName)
                        .ToArray()
                })
                .OrderBy(p => p.Title)
                .ThenByDescending(p => p.Genre)
                .ToArray();

            return xmlHelper.Serialize<ExportPlaysDto[]>(playsDtos, "Plays"); 
        }
    }
}
