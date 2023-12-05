using System.Text;
using Boardgames.Data.Models;
using Boardgames.Data.Models.Enums;
using Boardgames.DataProcessor.ImportDto;
using Boardgames.Utilities;
using Newtonsoft.Json;

namespace Boardgames.DataProcessor
{
    using System.ComponentModel.DataAnnotations;
    using Boardgames.Data;
   
    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedCreator
            = "Successfully imported creator – {0} {1} with {2} boardgames.";

        private const string SuccessfullyImportedSeller
            = "Successfully imported seller - {0} with {1} boardgames.";

        public static string ImportCreators(BoardgamesContext context, string xmlString)
        {
            XmlHelper xmlHelper = new XmlHelper();
            var creatorsDto = xmlHelper.Deserialize<ImportCreatorDto[]>(xmlString, "Creators");

            StringBuilder sb = new StringBuilder();
            HashSet<Creator> validCreators = new HashSet<Creator>();
            foreach (var creatorDto in creatorsDto)
            {
                if (!IsValid(creatorDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Creator creator = new Creator()
                {
                    FirstName = creatorDto.FirstName,
                    LastName = creatorDto.LastName
                };

                foreach (var boardGameDto in creatorDto.ImportBoardgameDtos)
                {
                    if (!IsValid(boardGameDto))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    Boardgame boardgame = new Boardgame()
                    {
                        Name = boardGameDto.Name,
                        Rating = boardGameDto.Rating, 
                        YearPublished = boardGameDto.YearPublished,
                        CategoryType = (CategoryType)boardGameDto.CategoryType,
                        Mechanics = boardGameDto.Mechanics
                    };

                    creator.Boardgames.Add(boardgame);
                }

                validCreators.Add(creator);
                sb.AppendLine(string.Format(SuccessfullyImportedCreator, creator.FirstName, creator.LastName,
                    creator.Boardgames.Count));
            }

            context.AddRange(validCreators);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportSellers(BoardgamesContext context, string jsonString)
        {
            var sellersJson = JsonConvert.DeserializeObject<ImportSellerDto[]>(jsonString);

            StringBuilder sb = new StringBuilder();
            int[] existingGamesIds = context.Boardgames.Select(bg => bg.Id).ToArray();

            HashSet<Seller> validSellers = new HashSet<Seller>();
            foreach (var sellerDto in sellersJson!)
            {
                if (!IsValid(sellerDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Seller seller = new Seller()
                {
                    Name = sellerDto.Name,
                    Address = sellerDto.Address,
                    Country = sellerDto.Country,
                    Website = sellerDto.Website,
                };

                foreach (var gameDto in sellerDto.Boardgames.Distinct())
                {
                    var gameExist = existingGamesIds.Contains(gameDto);
                    if (!gameExist)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    BoardgameSeller boardgame = new BoardgameSeller()
                    {
                        BoardgameId = gameDto,
                        Seller = seller
                    };

                    seller.BoardgamesSellers.Add(boardgame);
                }

                validSellers.Add(seller);
                sb.AppendLine(string.Format(SuccessfullyImportedSeller, seller.Name, seller.BoardgamesSellers.Count));
            }

            context.AddRange(validSellers);
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
