using BitMiracle.LibTiff.Classic;
using MsgReader.Outlook;
using System.Drawing;
using System.Drawing.Imaging;
using Xceed.Words.NET;

namespace ImageToPdfAPI.Services
{   public interface IConversionLogicService
    {
        //List<byte[]> ConvertDocxToImages(Stream docxStream);
        List<byte[]> ConvertTiffToImages(Stream tifStream);
        List<byte[]> ConvertMsgToImages(Stream msgStream);
        List<byte[]> ConvertDocxToImages(Stream docxStream);
    }
    public class ConversionLogicService: IConversionLogicService
    {
        #region DocxConversion
        public List<byte[]> ConvertDocxToImages(Stream docxStream)
        {
            using (var doc = DocX.Load(docxStream))
            {
                var images = new List<byte[]>();

                foreach (Xceed.Document.NET.Picture picture in doc.Pictures)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        // Use the appropriate method or property to get the image data
                        var imageData = GetImageDataFromPicture(picture);
                        ms.Write(imageData, 0, imageData.Length);
                        images.Add(ms.ToArray());
                    }
                }

                return images;
            }
        }

        //to extract image data from the Picture class
        private byte[] GetImageDataFromPicture(Xceed.Document.NET.Picture picture)
        {
            // to obtain the image data from the Picture class
            using (MemoryStream ms = new MemoryStream())
            {
                picture.Stream.CopyTo(ms);
                return ms.ToArray();
            }
        }
        #endregion



        #region .tif Conversion
        public List<byte[]> ConvertTiffToImages(Stream tifStream)
        {
            var images = new List<byte[]>();

            using (var tif = Tiff.ClientOpen("in-memory", "r", tifStream, new TiffStream()))
            {
                do
                {
                    // Convert TIFF image to JPEG
                    var jpegBytes = ConvertTiffToJpeg(tif);
                    images.Add(jpegBytes);
                } 
                while (tif.ReadDirectory());
            }

            return images;
        }

        private byte[] ConvertTiffToJpeg(Tiff tif)
        {
            var jpegStream = new MemoryStream();

            do
            {
                var width = tif.GetField(TiffTag.IMAGEWIDTH)[0].ToInt();
                var height = tif.GetField(TiffTag.IMAGELENGTH)[0].ToInt();
                var bitsPerSample = tif.GetField(TiffTag.BITSPERSAMPLE)[0].ToInt();
                var samplesPerPixel = tif.GetField(TiffTag.SAMPLESPERPIXEL)[0].ToInt();
                var photoMetric = tif.GetField(TiffTag.PHOTOMETRIC)[0].ToInt();

                // Create a Bitmap from the TIFF frame
                using (var bitmap = new Bitmap(width, height))
                {
                    for (var i = 0; i < height; i++)
                    {
                        var scanline = new byte[width * samplesPerPixel * bitsPerSample / 8];
                        tif.ReadScanline(scanline, i);

                        for (var j = 0; j < width; j++)
                        {
                            // Set pixel color directly from the original scanline
                            var index = j * samplesPerPixel * bitsPerSample / 8;
                            Color pixelColor;

                            if (bitsPerSample == 8)
                            {
                                pixelColor = Color.FromArgb(scanline[index], scanline[index + 1], scanline[index + 2]);
                            }
                            else if (bitsPerSample == 16)
                            {
                                pixelColor = Color.FromArgb(scanline[index + 1], scanline[index + 3], scanline[index + 5]);
                            }
                            else
                            {
                                // Handle other cases as needed
                                pixelColor = Color.Black;
                            }

                            bitmap.SetPixel(j, i, pixelColor);
                        }
                    }

                    // Save the Bitmap as JPEG to the MemoryStream
                    bitmap.Save(jpegStream, ImageFormat.Jpeg);
                }

            } while (tif.ReadDirectory());

            return jpegStream.ToArray();
        }

        #endregion




        #region .msg conversion
        public List<byte[]> ConvertMsgToImages(Stream msgStream)
        {
            var images = new List<byte[]>();
            using (var msg = new Storage.Message(msgStream))
            {
                foreach (var attachment in msg.Attachments)
                {
                    if (attachment is Storage.Attachment)
                    {
                        var attachmentData = (attachment as Storage.Attachment).Data;
                        if (attachmentData != null)
                        {
                            images.Add(attachmentData);
                        }
                    }
                }
            }
            return images;
        }
        #endregion



    }
}
