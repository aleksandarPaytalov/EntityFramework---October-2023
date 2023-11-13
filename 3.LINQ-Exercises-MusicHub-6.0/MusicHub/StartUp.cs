using System.Globalization;
using System.Linq.Expressions;
using System.Text;

namespace MusicHub;

using System;
using Data;
using Initializer;

public class StartUp
{
    public static void Main()
    {
        MusicHubDbContext context =
            new MusicHubDbContext();

        DbInitializer.ResetDatabase(context);

        var result = ExportSongsAboveDuration(context, 4);
        Console.WriteLine(result);
    }

    public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
    {
        var albumsInfo = context.Albums
            .Where(p => p.ProducerId == producerId)
            .ToList()// && p.ProducerId.HasValue)
            .OrderByDescending(a => a.Price)
            .Select(a => new
            {
                a.Name,
                ReleaseDate = a.ReleaseDate.ToString("MM/dd/yyy", CultureInfo.InvariantCulture),
                ProducerName = a.Producer.Name,
                Songs = a.Songs.Select(s => new
                {
                    SongName = s.Name,
                    Price = s.Price.ToString("f2"),
                    WriterName = s.Writer.Name
                })
                    .OrderByDescending(s => s.SongName)
                    .ThenBy(s => s.WriterName)
                    .ToList(),
                TotalAlbumPrice = a.Price.ToString("f2")
            })
            .ToList();

        StringBuilder sb = new StringBuilder();

        foreach (var a in albumsInfo)
        {
            sb.AppendLine($"-AlbumName: {a.Name}");
            sb.AppendLine($"-ReleaseDate: {a.ReleaseDate}");
            sb.AppendLine($"-ProducerName: {a.ProducerName}");
            sb.AppendLine($"-Songs:");

            int albumIncrement = 1;
            foreach (var s in a.Songs)
            {
                sb.AppendLine($"---#{albumIncrement++}");
                sb.AppendLine($"---SongName: {s.SongName}");
                sb.AppendLine($"---Price: {s.Price}");
                sb.AppendLine($"---Writer: {s.WriterName}");
            }

            sb.AppendLine($"-AlbumPrice: {a.TotalAlbumPrice}");
        }

        return sb.ToString().TrimEnd();
    }

    public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
    {
        var songsAboveGivenDuration = context.Songs
            //.ToList()
            .Where(s => s.Duration.TotalSeconds > duration)
            .Select(s => new
            {
                s.Name,
                PerformersNames = s.SongPerformers
                    .Select(p => $"{p.Performer.FirstName} {p.Performer.LastName}")
                    .OrderBy(p => p)
                    .ToList(),
                WriterName = s.Writer.Name,
                AlbumProducer = s.Album!.Producer!.Name,
                Duration = s.Duration.ToString("c")

            })
            .OrderBy(s => s.Name)
            .ThenBy(s => s.WriterName)
            .ToList();

        StringBuilder sb = new StringBuilder();
        int songIncrement = 1;
        foreach (var s in songsAboveGivenDuration)
        {
            sb.AppendLine($"-Song #{songIncrement++}");
            sb.AppendLine($"---SongName: {s.Name}");
            sb.AppendLine($"---Writer: {s.WriterName}");

            foreach (var p in s.PerformersNames)
            {
                sb.AppendLine($"---Performer: {p}");
            }
            
            sb.AppendLine($"---AlbumProducer: {s.AlbumProducer}");
            sb.AppendLine($"---Duration: {s.Duration}");
        }

        return sb.ToString().TrimEnd();
    }
}

