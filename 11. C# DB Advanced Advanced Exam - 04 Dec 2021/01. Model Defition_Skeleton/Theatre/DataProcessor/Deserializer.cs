using Newtonsoft.Json;

namespace Theatre.DataProcessor;

using Data.Models.Enums;
using System.ComponentModel.DataAnnotations;

using Data;
using System;
using ImportDto;
using Utilities;
using System.Text;
using Data.Models;
using System.Globalization;
using System.Xml.Linq;
using System.IO;
using System.Diagnostics;


public class Deserializer
{
    private const string ErrorMessage = "Invalid data!";

    private const string SuccessfulImportPlay
        = "Successfully imported {0} with genre {1} and a rating of {2}!";

    private const string SuccessfulImportActor
        = "Successfully imported actor {0} as a {1} character!";

    private const string SuccessfulImportTheatre
        = "Successfully imported theatre {0} with #{1} tickets!";



    public static string ImportPlays(TheatreContext context, string xmlString)
    {
        var validGenres = new string[] { "Drama", "Comedy", "Romance", "Musical" };
        var minimumTime = new TimeSpan(1, 0, 0);
        

        XmlHelper xmlHelper = new XmlHelper();
        var playsDto = xmlHelper.Deserialize<ImportPlayDto[]>(xmlString, "Plays");

        StringBuilder sb = new StringBuilder();
        HashSet<Play> validPlays = new HashSet<Play>();
        foreach (var play in playsDto)
        {
            if (!IsValid(play))
            {
                sb.AppendLine(ErrorMessage);
                continue;
            }

            var duration = TimeSpan.ParseExact(play.Duration, "c", CultureInfo.InvariantCulture);
            if(duration < minimumTime)
            {
                sb.AppendLine(ErrorMessage);
                continue;
            }

            if (!validGenres.Contains(play.Genre))
            {
                sb.AppendLine(ErrorMessage);
                continue;
            }

            Play validPlay = new Play()
            {
                Title = play.Title,
                Duration = duration,
                Rating = play.Rating,
                Genre = (Genre)Enum.Parse(typeof(Genre), play.Genre),
                Description = play.Description,
                Screenwriter = play.Screenwriter
            };

            validPlays.Add(validPlay);
            sb.AppendLine(string.Format(SuccessfulImportPlay, play.Title, play.Genre, play.Rating));
        }

        context.AddRange(validPlays);
        context.SaveChanges();

        return sb.ToString().TrimEnd();
    }

    public static string ImportCasts(TheatreContext context, string xmlString)
    {
        XmlHelper xmlHelper = new XmlHelper();
        var castsDto = xmlHelper.Deserialize<ImportCastDto[]>(xmlString, "Casts");

        StringBuilder sb = new StringBuilder();
        HashSet<Cast> validCasts = new HashSet<Cast>();
        foreach (var cast in castsDto)
        {
            if (!IsValid(cast))
            {
                sb.AppendLine(ErrorMessage);
                continue;
            }

            Cast validCast = new Cast()
            {
                FullName = cast.FullName,
                IsMainCharacter = cast.IsMainCharacter,
                PhoneNumber = cast.PhoneNumber,
                PlayId = cast.PlayId
            };

            validCasts.Add(validCast);
            sb.AppendLine(string.Format(SuccessfulImportActor, cast.FullName, cast.IsMainCharacter ? "main" : "lesser"));
        }

        context.AddRange(validCasts);
        context.SaveChanges();

        return sb.ToString().TrimEnd();
    }
    
    public static string ImportTtheatersTickets(TheatreContext context, string jsonString)
    {
        ImportTheatreDto[] theatresDto = JsonConvert.DeserializeObject<ImportTheatreDto[]>(jsonString)!;

        StringBuilder sb = new StringBuilder();
        HashSet<Theatre> validTheatres = new HashSet<Theatre>();

        foreach (var theater in theatresDto)
        {
            if (!IsValid(theater))
            {
                sb.AppendLine(ErrorMessage);
                continue;
            }

            Theatre validTheatre = new Theatre()
            {
                Name = theater.Name,
                NumberOfHalls = theater.NumberOfHalls,
                Director = theater.Director
            };

            foreach (var ticket in theater.Tickets)
            {
                if (!IsValid(ticket))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Ticket validTicket = new Ticket()
                {
                    Price = ticket.Price,
                    RowNumber = ticket.RowNumber,
                    PlayId = ticket.PlayId
                };

                validTheatre.Tickets.Add(validTicket);
            }

            validTheatres.Add(validTheatre);
            sb.AppendLine(string.Format(SuccessfulImportTheatre, validTheatre.Name, validTheatre.Tickets.Count));
        }

        context.AddRange(validTheatres);
        context.SaveChanges();

        return sb.ToString().TrimEnd();
    }


    private static bool IsValid(object obj)
    {
        var validator = new ValidationContext(obj);
        var validationRes = new List<ValidationResult>();

        var result = Validator.TryValidateObject(obj, validator, validationRes, true);
        return result;
    }
}

