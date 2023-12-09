using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Newtonsoft.Json;
using VaporStore.DataProcessor.ExportDto;
using VaporStore.Utilities;

namespace VaporStore.DataProcessor
{ 
    using Data;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using VaporStore.Data.Models;
    using VaporStore.Data.Models.Enums;
    using VaporStore.DataProcessor.ImportDto;

    public static class Serializer
    {
        public static string ExportGamesByGenres(VaporStoreDbContext context, string[] genreNames)
        {
            var games = context.Genres
                .ToArray()
                .Where(g => genreNames.Contains(g.Name))
                .Select(g => new
                {
                    Id = g.Id,
                    Genre = g.Name,
                    Games = g.Games
                        .Where(gm => gm.Purchases.Any())
                        .Select(gm => new
                        {
                            Id = gm.Id,
                            Title = gm.Name,
                            Developer = gm.Developer.Name,
                            Tags = string.Join(", ", gm.GameTags
                                .Select(gt => gt.Tag.Name)
                                .ToArray()),
                            Players = gm.Purchases.Count
                        })
                        .OrderByDescending(gm => gm.Players)
                        .ThenBy(gm => gm.Id)
                        .ToArray(),
                    TotalPlayers = g.Games.Sum(gm => gm.Purchases.Count)
                })
                .OrderByDescending(g => g.TotalPlayers)
                .ThenBy(g => g.Id)
                .ToArray();

            return JsonConvert.SerializeObject(games, Formatting.Indented);
        }

        public static string ExportUserPurchasesByType(VaporStoreDbContext context, string purchaseType)
        {
            var users = context.Users
                .ToArray()
                .Where(u => u.Cards.Any(c => c.Purchases.Any()))
                .Select(u => new ExportUserDto()
                {
                    Username = u.Username,
                    Purchases = context.Purchases
                        .ToArray()
                        .Where(p => p.Type.ToString() == purchaseType && p.Card.User.Username == u.Username)
                        .OrderBy(p => p.Date)
                        .Select(p => new ExportUserPurchaseDto()
                        {
                            CardNumber = p.Card.Number,
                            CardCvc = p.Card.Cvc,
                            Date = p.Date.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
                            Game = new ExportUserPurchaseGameDto()
                            {
                                Genre = p.Game.Genre.Name,
                                Price = p.Game.Price,
                                Name = p.Game.Name
                            }
                        })
                        .ToArray(),
                    TotalSpent = context.Purchases
                        .ToArray()
                        .Where(p => p.Type.ToString() == purchaseType && p.Card.User.Username == u.Username)
                        .Sum(p => p.Game.Price)
                })
                .Where(u => u.Purchases.Length > 0)
                .OrderByDescending(u => u.TotalSpent)
                .ThenBy(u => u.Username)
                .ToArray();

            XmlHelper xmlHelper = new XmlHelper();

            return xmlHelper.Serialize(users, "Users");
        }
    }
}