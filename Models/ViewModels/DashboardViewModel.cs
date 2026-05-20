namespace PediTrack.Models.ViewModels
{
    public class DashboardViewModel
    {
        // Summary Counts
        public int TotalParticipants { get; set; }
        public int ActiveParticipants { get; set; }
        public int TotalStudies { get; set; }
        public int ActiveStudies { get; set; }
        public int TotalVisits { get; set; }
        public int UpcomingVisits { get; set; }
        public int PendingConsents { get; set; }
        public int TotalEnrollments { get; set; }

        // Chart Data
        public List<string> EnrollmentMonths { get; set; } = new();
        public List<int> EnrollmentCounts { get; set; } = new();
        public List<string> StudyStatusLabels { get; set; } = new();
        public List<int> StudyStatusCounts { get; set; } = new();
        public List<string> VisitStatusLabels { get; set; } = new();
        public List<int> VisitStatusCounts { get; set; } = new();

        // Recent Activity
        public List<Participant> RecentParticipants { get; set; } = new();
        public List<Visit> UpcomingVisitList { get; set; } = new();
        public List<StudyEnrollment> RecentEnrollments { get; set; } = new();

        // Alerts
        public int ExpiredConsents { get; set; }
        public int MissedVisits { get; set; }
        public int ExpiringConsentsThisMonth { get; set; }
    }
}
