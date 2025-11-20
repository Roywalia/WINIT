namespace WINIT.Models
{
    // Simple view model used by HomeController's dashboard view
    public class LogViewModel
    {
        public string ReferenceId { get; set; } = null!;
        public string Domain { get; set; } = null!;
        public DateTime ReceivedTime { get; set; }
        public int ApiStatus { get; set; }
        public string ProcessingStatus { get; set; } = null!;
        public string? ValidationErrors { get; set; }
        public string RawPayload { get; set; } = null!;
    }
}