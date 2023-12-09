using System.Globalization;
using System.Text;
using Newtonsoft.Json;
using VaporStore.Data.Models;
using VaporStore.Data.Models.Enums;
using VaporStore.DataProcessor.ImportDto;
using VaporStore.Utilities;

namespace VaporStore.DataProcessor
{
    using System.ComponentModel.DataAnnotations;
    using AutoMapper.Execution;
    using Data;
    using Microsoft.EntityFrameworkCore.Metadata.Internal;

    public static class Deserializer
    {
        public const string ErrorMessage = "Invalid Data";

        public const string SuccessfullyImportedGame = "Added {0} ({1}) with {2} tags";

        public const string SuccessfullyImportedUser = "Imported {0} with {1} cards";

        public const string SuccessfullyImportedPurchase = "Imported {0} for {1}";

        public static string ImportGames(VaporStoreDbContext context, string jsonString)
        {
            var gamesDto = JsonConvert.DeserializeObject<ImportGameDeveloperGenreTagDto[]>(jsonString);

            StringBuilder sb = new StringBuilder();
            HashSet<Game> validGames = new HashSet<Game>();
            HashSet<Genre> validGenres = new HashSet<Genre>();
            HashSet<Developer> validDevelopers = new HashSet<Developer>();
            HashSet<Tag> validTags = new HashSet<Tag>();

            foreach (var gameDto in gamesDto!)
            {
                if (!IsValid(gameDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                DateTime releaseDate;
                bool isDateValid = DateTime.TryParseExact(gameDto.ReleaseDate, "yyyy-MM-dd",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out releaseDate);
                if (!isDateValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                // Не прави проверка с Isvalid за колекцията дали е празна, дори да я зададем required в Dto-то
                if (gameDto.Tags.Length == 0)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Game game = new Game()
                {
                    Name = gameDto.Name,
                    Price = gameDto.Price,
                    ReleaseDate = releaseDate
                };

                // Developer
                Developer developer = new Developer()
                {
                    Name = gameDto.Developer
                };
                var devExist = validDevelopers.FirstOrDefault(d => d.Name == gameDto.Developer);
                if (devExist == null)
                {
                    validDevelopers.Add(developer);
                    game.Developer = developer;
                }
                else
                {
                    game.Developer = devExist;
                }

                // Genre
                Genre genre = new Genre()
                {
                    Name = gameDto.Genre
                };
                Genre genreExist = validGenres.FirstOrDefault(g => g.Name == gameDto.Genre);
                if (genreExist == null)
                {
                    validGenres.Add(genre); 
                    game.Genre = genre;
                }
                else
                {
                    game.Genre = genreExist;
                }

                

                //Tags
                foreach (var tagDto in gameDto.Tags)
                {
                    Tag tag = new Tag()
                    {
                        Name = tagDto
                    };
                    Tag existedTag = validTags.FirstOrDefault(t => t.Name == tagDto);
                    if (existedTag == null)
                    {
                        validTags.Add(tag);
                        GameTag gameTag = new GameTag()
                        {
                            Tag = tag,
                            Game = game
                        };
                        game.GameTags.Add(gameTag);
                    }
                    else
                    {
                        GameTag gameTag = new GameTag()
                        {
                            Tag = existedTag,
                            Game = game
                        };
                        game.GameTags.Add(gameTag);
                    }
                    
                }

                validGames.Add(game);
                sb.AppendLine(string.Format(SuccessfullyImportedGame, game.Name, game.Genre.Name, game.GameTags.Count));
            }

            context.AddRange(validGames);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportUsers(VaporStoreDbContext context, string jsonString)
        {
            var usersJson = JsonConvert.DeserializeObject<ImportUserDto[]>(jsonString);

            StringBuilder sb = new StringBuilder();
            HashSet<User> validUsers = new HashSet<User>();
            foreach (var userDto in usersJson)
            {
                if (!IsValid(userDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (userDto.Cards.Length == 0)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                User user = new User()
                {
                    FullName = userDto.FullName,
                    Username = userDto.Username,
                    Email = userDto.Email,
                    Age = userDto.Age
                };

                bool userCanBeAdded = true;
                foreach (var cardDto in userDto.Cards)
                {
                    if (!IsValid(cardDto))
                    {
                        sb.AppendLine(ErrorMessage);
                        userCanBeAdded = false;
                        break;
                    }

                    CardType type;
                    var enumIsValid = Enum.TryParse(cardDto.Type, true, out type);
                    if (!enumIsValid)
                    {
                        sb.AppendLine(ErrorMessage);
                        userCanBeAdded = false;
                        break;
                    }

                    Card card = new Card()
                    {
                        Number = cardDto.Number,
                        Cvc = cardDto.Cvc,
                        Type = type
                    };

                    user.Cards.Add(card);
                }

                if (userCanBeAdded)
                {
                    validUsers.Add(user);
                    sb.AppendLine(string.Format(SuccessfullyImportedUser, user.Username, user.Cards.Count));
                }
            }

            context.AddRange(validUsers);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportPurchases(VaporStoreDbContext context, string xmlString)
        {
            XmlHelper xmlHelper = new XmlHelper();
            var purchasesXml = xmlHelper.Deserialize<ImportPurchaseDto[]>(xmlString, "Purchases");

            StringBuilder sb = new StringBuilder();
            HashSet<Purchase> validPurchases = new HashSet<Purchase>();

            foreach (var purchaseDto in purchasesXml)
            {
                if (!IsValid(purchaseDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                DateTime date;
                bool isDateValid = DateTime.TryParseExact(purchaseDto.Date, "dd/MM/yyyy HH:mm",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
                if (!isDateValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                PurchaseType purchaseType;
                bool isTypeValid = Enum.TryParse(purchaseDto.Type, true, out purchaseType);
                if (!isTypeValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Game game = context.Games.FirstOrDefault(g => g.Name == purchaseDto.title);
                if (game == null)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Card card = context.Cards.FirstOrDefault(c => c.Number == purchaseDto.Card);
                if (card == null)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                
                Purchase purchase = new Purchase()
                {
                    Type = purchaseType,
                    ProductKey = purchaseDto.Key,
                    Date = date,
                    Game = game,
                    Card = card
                };

                validPurchases.Add(purchase);
                sb.AppendLine(string.Format(SuccessfullyImportedPurchase, purchase.Game.Name,
                    purchase.Card.User.Username));

            }

            context.AddRange(validPurchases);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}