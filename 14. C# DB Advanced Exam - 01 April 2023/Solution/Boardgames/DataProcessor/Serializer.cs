using Boardgames.DataProcessor.ExportDto;
using Boardgames.Utilities;
using Newtonsoft.Json;

namespace Boardgames.DataProcessor
{
    using Boardgames.Data;

    public class Serializer
    {
        public static string ExportCreatorsWithTheirBoardgames(BoardgamesContext context)
        {
            var creators = context.Creators
                .ToArray()
                .Where(c => c.Boardgames.Any())
                .Select(c => new ExportCreatorDto()
                {
                    CreatorName = $"{c.FirstName} {c.LastName}",
                    BoardgamesCount = c.Boardgames.Count,
                    Boardgames = c.Boardgames
                        .Select(bg => new ExportBoardGameDto()
                        {
                            BoardgameName = bg.Name,
                            BoardgameYearPublished = bg.YearPublished
                        })
                        .OrderBy(bg => bg.BoardgameName)
                        .ToArray()
                })
                .OrderByDescending(c => c.Boardgames.Length)
                .ThenBy(c => c.CreatorName)
                .ToArray();

            XmlHelper xmlHelper = new XmlHelper();

            return xmlHelper.Serialize(creators, "Creators");
        }

        public static string ExportSellersWithMostBoardgames(BoardgamesContext context, int year, double rating)
        {
            var sellers = context.Sellers
                .Where(s => s.BoardgamesSellers.Any() && s.BoardgamesSellers
                    .Any(bg => bg.Boardgame.YearPublished >= year && bg.Boardgame.Rating <= rating))
                .Select(s => new
                {
                    Name = s.Name,
                    Website = s.Website,
                    Boardgames = s.BoardgamesSellers
                        .Where(bg => bg.Boardgame.YearPublished >= year && bg.Boardgame.Rating <= rating)
                        .Select(bg => new
                        {
                            Name = bg.Boardgame.Name,
                            Rating = bg.Boardgame.Rating,
                            Mechanics = bg.Boardgame.Mechanics,
                            Category = bg.Boardgame.CategoryType.ToString()
                        })
                        .OrderByDescending(bg => bg.Rating)
                        .ThenBy(bg => bg.Name)
                        .ToArray()
                })
                .OrderByDescending(s => s.Boardgames.Length)
                .ThenBy(s => s.Name)
                .Take(5)
                .ToArray();

            return JsonConvert.SerializeObject(sellers, Formatting.Indented);
        }
    }
}