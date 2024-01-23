namespace CMS.Shared.Models
{
    public class FileRequestModel
    {
        public int UserId { get; set; }

        public int PatientId { get; set; }

        public string Base64Image { get; set; }

        public string FileName { get; set; }
    }
}
