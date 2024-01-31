using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using ImageToPdfAPI.Models.ResponseModel;
using ImageToPdfAPI.Services;
using Microsoft.AspNetCore.Mvc;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace ImageToPdfAPI.Services
{   
    public interface IGetPdfLogicService
    {
        UserResponse ConvertToPdf(IFormFile file);

    }


    public class GetPdfLogicService : IGetPdfLogicService
    {
        private readonly IConversionLogicService _conversionLogicService;
        private readonly IConfiguration _configuration;
        public GetPdfLogicService(IConversionLogicService conversionLogic, IConfiguration configuration)
        {
            _conversionLogicService = conversionLogic;
            _configuration = configuration;
        }

        public UserResponse ConvertToPdf(IFormFile file)
        {
            var response = new UserResponse();

            if (file != null && file.Length != 0)
            {
                try
                {
                    List<byte[]> images;

                    // Detect file type and convert to images
                    switch (Path.GetExtension(file.FileName).ToLower())
                    {
                        case ".tif":
                            images = _conversionLogicService.ConvertTiffToImages(file.OpenReadStream());
                            break;
                        case ".docx":
                            images = _conversionLogicService.ConvertDocxToImages(file.OpenReadStream());
                            break;
                        case ".msg":
                            images = _conversionLogicService.ConvertMsgToImages(file.OpenReadStream());
                            break;
                        default:
                            // Assume it's an image file
                            images = new List<byte[]> { ReadAllBytes(file) };
                            break;
                    }

                    // Create a PDF document
                    using (var document = new PdfDocument())
                    {
                        string imageBaseName = Path.GetFileNameWithoutExtension(file.FileName);
                        foreach (var imageData in images)
                        {
                            // Add a page
                            var page = document.AddPage();
                            using (var gfx = XGraphics.FromPdfPage(page))
                            {
                                // Draw the image on the page
                                using (var imageStream = new MemoryStream(imageData))
                                {
                                    // Create a new MemoryStream without using the internal buffer
                                    using (var nonSeekableStream = new NonSeekableStream(imageStream))
                                    {
                                        var image = XImage.FromStream(nonSeekableStream);
                                        gfx.DrawImage(image, 0, 0, page.Width, page.Height);
                                    }
                                }
                            }
                        }

                        // Save the PDF to a specific directory
                        string folderPath = _configuration["DownloadSettings:DownloadBaseUrl"]; // Specify the directory path
                        string pdfFileName = $"{imageBaseName}.pdf";
                        var filePath = Path.Combine(folderPath, pdfFileName);

                        Directory.CreateDirectory(folderPath);
                        document.Save(filePath);

                        response.PdfContent = File.ReadAllBytes(filePath); // Optionally, read the bytes again
                        response.Success = true;
                        response.DownloadLink = filePath; // Return the file path as the download link
                        response.Message = "Success";
                    }
                }
                catch (Exception ex)
                {
                    response.Message = $"Error: {ex.Message}";
                    response.Success = false;
                }
            }
            else
            {
                response.Message = "No file uploaded.";
                response.Success = false;
            }

            return response;
        }


        //Helper method to read bytes from IFormFile
        private static byte[] ReadAllBytes(IFormFile file)
        {
            using (var memoryStream = new MemoryStream())
            {
                file.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

    }








    #region Memory Stream
    public class NonSeekableStream : Stream
    {
        private readonly Stream _baseStream;

        public NonSeekableStream(Stream baseStream)
        {
            _baseStream = baseStream ?? throw new ArgumentNullException(nameof(baseStream));
        }

        public override bool CanRead => _baseStream.CanRead;
        public override bool CanSeek => false;
        public override bool CanWrite => _baseStream.CanWrite;
        public override long Length => _baseStream.Length;

        public override long Position
        {
            get => _baseStream.Position;
            set => _baseStream.Position = value;
        }

        public override void Flush() => _baseStream.Flush();
        public override int Read(byte[] buffer, int offset, int count) => _baseStream.Read(buffer, offset, count);
        public override long Seek(long offset, SeekOrigin origin) => _baseStream.Seek(offset, origin);
        public override void SetLength(long value) => _baseStream.SetLength(value);
        public override void Write(byte[] buffer, int offset, int count) => _baseStream.Write(buffer, offset, count);
    }

    #endregion
}
