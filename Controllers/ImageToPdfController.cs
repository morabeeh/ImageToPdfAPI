using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using ImageToPdfAPI.Models.RequestModel;
using ImageToPdfAPI.Services;
using Microsoft.AspNetCore.Mvc;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

[ApiController]
[Route("api/[controller]")]
public class ImageToPdfController : ControllerBase
{
    private readonly IConversionLogicService _conversionLogicService;
    private readonly IGetPdfLogicService _getPdfLogicService;

    public ImageToPdfController(IConversionLogicService conversionLogic,IGetPdfLogicService getPdfLogicService)
    {
        _conversionLogicService = conversionLogic;
        _getPdfLogicService = getPdfLogicService;
    }


    [HttpPost("Convert")]
    public IActionResult ConvertToPdf([FromForm] UserRequest request)
    {
        var response = _getPdfLogicService.ConvertToPdf(request.File);
        return Ok(response);
    }

}


