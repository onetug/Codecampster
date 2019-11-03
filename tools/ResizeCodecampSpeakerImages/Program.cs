using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Codecamp.Data;

namespace ResizeCodecampSpeakerImages
{
    class Program
    {

        static void Main(string[] args)
        {
            const int MaxSizePixels = 290;

            StreamWriter log = new StreamWriter("db_image_resize.txt");
            log.WriteLine("Beginning image resize");

            using (var context = new CodecampDbContext())
            {
                var speakers = (from speaker in context.Speakers
                               join _event in context.Events
                                    on speaker.EventId equals _event.EventId
                               where _event.IsActive == true
                               select speaker).ToList();
            
                for (int index = 0; index < speakers.Count(); index++)
                {
                    if (speakers[index].SpeakerId == 46)
                        continue;

                    // Get the speaker image
                    var imageArray = speakers[index].Image;
                    if (imageArray != null)
                    {
                        var imageStream = new MemoryStream(imageArray);
                        try
                        {
                            using (var image = new Bitmap(imageStream))
                            {
                                // If the image width or height is greater
                                if (image.Width > MaxSizePixels
                                    || image.Height > MaxSizePixels)
                                {
                                    Console.WriteLine("SpeakerId: {0}, Height: {1} px, Width: {2}."
                                        + "  Speaker will be resized.",
                                        speakers[index].SpeakerId, image.Width, image.Height);
                                    log.WriteLine("SpeakerId: {0}, Height: {1} px, Width: {2}."
                                        + "  Speaker will be resized.",
                                        speakers[index].SpeakerId, image.Width, image.Height);

                                    // Resize this image
                                    int width, height;
                                    if (image.Width > image.Height)
                                    {
                                        width = MaxSizePixels;
                                        height = Convert.ToInt32(image.Height * MaxSizePixels 
                                            / (double)image.Width);
                                    }
                                    else
                                    {
                                        width = Convert.ToInt32(image.Width * MaxSizePixels 
                                            / (double)image.Height);
                                        height = MaxSizePixels;
                                    }

                                    var resized = new Bitmap(width, height);
                                    using (var graphics = Graphics.FromImage(resized))
                                    {
                                        graphics.CompositingQuality = CompositingQuality.HighSpeed;
                                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                        graphics.CompositingMode = CompositingMode.SourceCopy;
                                        graphics.DrawImage(image, 0, 0, width, height);

                                        using (var ms = new MemoryStream())
                                        {
                                            var qualityParamId = Encoder.Quality;
                                            var encoderParameters = new EncoderParameters(1);
                                            encoderParameters.Param[0] 
                                                = new EncoderParameter(qualityParamId, 100L);
                                            var codec = ImageCodecInfo.GetImageDecoders()
                                                .FirstOrDefault(c => c.FormatID == ImageFormat.Jpeg.Guid);
                                            resized.Save(ms, codec, encoderParameters);

                                            speakers[index].Image = ms.ToArray();
                                        }
                                    }

                                    context.SaveChanges();
                                }
                            }
                        }
                        catch(Exception)
                        {
                            Console.WriteLine("SpeakerId: {0} image is invalid, deleting speaker image.",
                                speakers[index].SpeakerId);
                            log.WriteLine("SpeakerId: {0} image is invalid, deleting speaker image.",
                                speakers[index].SpeakerId);

                            speakers[index].Image = null;

                            context.SaveChanges();
                        }
                    }
                }
            }

            log.WriteLine("End image resize");
            log.Close();
        }
    }
}
