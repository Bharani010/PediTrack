namespace PediTrack.Models.ViewModels
{
    public class ReportViewModel
    {
        public string ReportTitle { get; set; } = string.Empty;
        public string GeneratedBy { get; set; } = "PediTrack System";
        public DateTime GeneratedAt { get; set; } = DateTime.Now;
        public string Institution { get; set; } = "Meridian Children's Hospital – Pediatrics Research";

        // Filters
        public int? StudyId { get; set; }
        public string? Status { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public List<Study> AvailableStudies { get; set; } = new();

        // Data
        public List<Participant> Participants { get; set; } = new();
        public List<Visit> Visits { get; set; } = new();
        public List<StudyEnrollment> Enrollments { get; set; } = new();
        public List<ConsentForm> ConsentForms { get; set; } = new();

        // Summary Stats
        public int TotalRecords { get; set; }
        public Dictionary<string, int> StatusBreakdown { get; set; } = new();
    }

    public class ParticipantReportRow
    {
        public string MRN { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string StudyName { get; set; } = string.Empty;
        public DateTime EnrollmentDate { get; set; }
        public int TotalVisits { get; set; }
        public int CompletedVisits { get; set; }
        public string ConsentStatus { get; set; } = string.Empty;
    }
}
