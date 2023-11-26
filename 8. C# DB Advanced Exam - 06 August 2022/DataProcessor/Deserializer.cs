namespace Footballers.DataProcessor;

using Data.Models;
using ImportDto;
using Utilities;
using Data;
using Data.Models.Enums;

using System.Text;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

public class Deserializer
{
    private const string ErrorMessage = "Invalid data!";

    private const string SuccessfullyImportedCoach
        = "Successfully imported coach - {0} with {1} footballers.";

    private const string SuccessfullyImportedTeam
        = "Successfully imported team - {0} with {1} footballers.";


    public static string ImportCoaches(FootballersContext context, string xmlString)
    {
        StringBuilder sb = new StringBuilder();

        var xmlHelper = new XmlHelper();
        ImportCoachDTO[] coaches = xmlHelper.Deserialize<ImportCoachDTO[]>(xmlString, "Coaches");

        List<Coach> validCoaches = new List<Coach>();
        foreach (var coach in coaches)
        {
            if (!IsValid(coach))
            {
                sb.AppendLine(ErrorMessage);
                continue;
            }

            if (String.IsNullOrEmpty(coach.Nationality))
            {
                sb.AppendLine(ErrorMessage);
                continue;
            }
            Coach coachToAdd = new Coach()
            {
                Name = coach.Name,
                Nationality = coach.Nationality
            };

            
            foreach (var footballer in coach.Footballers)
            {
                DateTime contractStartDate;
                DateTime contractEndDate;

                if (!IsValid(footballer))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (!DateTime.TryParseExact(footballer.ContractStartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out contractStartDate)
                    || !DateTime.TryParseExact(footballer.ContractEndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out contractEndDate))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (contractStartDate > contractEndDate)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Footballer validFootballer = new Footballer()
                {
                    Name = footballer.Name,
                    ContractStartDate = contractStartDate,
                    ContractEndDate = contractEndDate,
                    BestSkillType = (BestSkillType)footballer.BestSkillType,
                    PositionType = (PositionType)footballer.PositionType
                };

                coachToAdd.Footballers.Add(validFootballer);
            }

            validCoaches.Add(coachToAdd);
            sb.AppendLine(string.Format(SuccessfullyImportedCoach, coach.Name, coachToAdd.Footballers.Count));
        }

        context.AddRange(validCoaches);
        context.SaveChanges();

        return sb.ToString().TrimEnd();
    }

    public static string ImportTeams(FootballersContext context, string jsonString)
    {
        StringBuilder sb = new StringBuilder();
        ImportTeamDto[] teams = JsonConvert.DeserializeObject<ImportTeamDto[]>(jsonString);

        int[] existingFootballersId = context.Footballers.Select(f => f.Id).ToArray();

        List<Team> validTeams = new List<Team>();
        foreach (var team in teams)
        {
            //check for invalid teams
            if (!IsValid(team))
            {
                sb.AppendLine(ErrorMessage);
                continue;
            }
            if (team.Trophies == 0 || string.IsNullOrEmpty(team.Nationality))
            {
                sb.AppendLine(ErrorMessage);
                continue;
            }

            //create a valid Team
            Team validTeam = new Team()
            {
                Name = team.Name,
                Nationality = team.Nationality,
                Trophies = team.Trophies
            };

            //Footballers validation
            foreach (var footballerId in team.FootballersId.Distinct())
            {
                //Invalid Footballer
                if (!existingFootballersId.Contains(footballerId))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                //creating valid Footballer
                TeamFootballer teamFootballer = new TeamFootballer()
                {
                    TeamId = validTeam.Id,
                    FootballerId = footballerId
                };

                //Add the valid Footballer to the Team
                validTeam.TeamsFootballers.Add(teamFootballer);
            }
            
            //Add the valid team to the Collection of valid teams
            validTeams.Add(validTeam);
            sb.AppendLine(string.Format(SuccessfullyImportedTeam, validTeam.Name,
                validTeam.TeamsFootballers.Count));
        }

        context.AddRange(validTeams);
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

