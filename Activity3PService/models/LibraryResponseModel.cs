using Activity3PService.Models;

namespace Activity3PService.Models
{
    public class LibraryResponseModel
    {
        // API Error
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public LibraryModel Library { get; set; }

        public LibraryResponseModel()
        {
            StatusCode = 500;
            Message = "Error: Library not initialized";
            Library = new LibraryModel();
        }
        public void SetStatus(int status, string msg) { StatusCode = status; Message = msg; }

    }
}
