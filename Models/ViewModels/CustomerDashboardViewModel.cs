namespace PediTrack.Models.ViewModels
{
    public class CustomerDashboardViewModel
    {
        public Participant Participant { get; set; } = null!;
        public List<StudyEnrollment> Enrollments { get; set; } = new();
        public List<Visit> UpcomingVisits { get; set; } = new();
        public List<Visit> RecentVisits { get; set; } = new();
        public List<ConsentForm> ConsentForms { get; set; } = new();
        public int TotalVisits { get; set; }
        public int CompletedVisits { get; set; }
        public int MissedVisits { get; set; }
        public int ScheduledVisits { get; set; }
    }
}
