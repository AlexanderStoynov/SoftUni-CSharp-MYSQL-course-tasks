namespace MusicHub
{
    using System;
    using System.Globalization;
    using System.Text;
    using Data;
    using Initializer;
    using MusicHub.Data.Models;

    public class StartUp
    {
        public static void Main()
        {
            MusicHubDbContext context =
                new MusicHubDbContext();

            DbInitializer.ResetDatabase(context);

            //Test your solutions here
            string result = ExportSongsAboveDuration(context, 4);

            Console.WriteLine(result);
        }

        public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
        {
            StringBuilder sb = new StringBuilder();

            var albumsInfo = context.Albums
                .Where(a => a.ProducerId.HasValue && a.ProducerId.Value == producerId)
                .ToArray()
                .OrderByDescending(a => a.Price)
                .Select(a => new
                {
                    a.Name,
                    ReleaseDate = a.ReleaseDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture),
                    ProducerName = a.Producer.Name,
                    AlbumSongs = a.Songs.Select(s => new
                    {
                        SongName = s.Name,
                        Price = s.Price.ToString("f2"),
                        SongWriterName = s.Writer.Name


                    })
                    .OrderByDescending(s => s.SongName)
                    .ThenBy(s => s.SongWriterName)
                    .ToArray(),
                    TotalAlbumPrice = a.Price.ToString("f2")

                })
                .ToArray();

            foreach ( var album in albumsInfo ) 
            {
                sb.AppendLine($"-AlbumName: {album.Name}");
                sb.AppendLine($"-ReleaseDate: {album.ReleaseDate}");
                sb.AppendLine($"-ProducerName: {album.ProducerName}");
                sb.AppendLine($"-Songs:");

                int songNumber = 1;

                foreach(var song in album.AlbumSongs)
                {
                    sb.AppendLine($"---#{songNumber}");
                    sb.AppendLine($"---SongName: {song.SongName}");
                    sb.AppendLine($"---Price: {song.Price}");
                    sb.AppendLine($"---Writer: {song.SongWriterName}");
                    songNumber++;
                }
                sb.AppendLine($"-AlbumPrice: {album.TotalAlbumPrice}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
        {
            StringBuilder sb = new StringBuilder();

            var songs = context.Songs
                .AsEnumerable()
                .Where(s => s.Duration.TotalSeconds > duration)
                .Select(s => new
                {
                    s.Name,
                    Performers = s.SongPerformers
                    .Select(p => $"{p.Performer.FirstName} {p.Performer.LastName}")
                    .OrderBy(p => p)
                    .ToArray(),
                    WriterName = s.Writer.Name,
                    AlbumProducer = s.Album.Producer.Name,
                    Duration = s.Duration.ToString("c")

                })
                .OrderBy(s => s.Name)
                .ThenBy(s => s.WriterName)
                .ToArray();

            int songNumber = 1;

            foreach(var song in songs) 
            {
                sb.AppendLine($"-Song #{songNumber}");
                sb.AppendLine($"---SongName: {song.Name}");
                sb.AppendLine($"---Writer: {song.WriterName}");
                

                foreach(var p in song.Performers)
                {
                    sb.AppendLine($"---Performer: {p}");
                }

                sb.AppendLine($"---AlbumProducer: {song.AlbumProducer}");
                sb.AppendLine($"---Duration: {song.Duration}");

                songNumber++;
            }

            return sb.ToString().TrimEnd();
        }
    }
}
