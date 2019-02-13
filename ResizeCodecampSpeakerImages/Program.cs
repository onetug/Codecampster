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
            const int MaxSizePixels = 300;

            using (var context = new CodecampDbContext())
            {
                var speakers = from speaker in context.Speakers
                               join _event in context.Events
                                    on speaker.EventId equals _event.EventId
                               where _event.IsActive == true
                               select speaker;
            
                foreach (var speaker in speakers)
                {
                    // Get the speaker image
                    var imageArray = speaker.Image;
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
                                        speaker.SpeakerId, image.Width, image.Height);

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

                                            speaker.Image = ms.ToArray();
                                        }
                                    }
                                }
                            }
                        }
                        catch(ArgumentException)
                        {
                            Console.WriteLine("SpeakerId: {0} image is invalid, deleting speaker image.", 
                                speaker.SpeakerId);
                            speaker.Image = null;
                        }
                    }
                }

                context.SaveChanges();
            }
        }
    }
}
