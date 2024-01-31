namespace ImageToPdfAPI.Models.ResponseModel
{
    public class UserResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public string DownloadLink { get; set; }
        public byte[] PdfContent { get; set; }
        
    }
}
