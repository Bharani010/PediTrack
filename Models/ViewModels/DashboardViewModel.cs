namespace PediTrack.Models.ViewModels
{
    public class DashboardViewModel
    {
        // ── Summary Counts ──────────────────────────────────────
        public int TotalParticipants { get; set; }
        public int ActiveParticipants { get; set; }
        public int TotalStudies { get; set; }
        public int ActiveStudies { get; set; }
        public int TotalVisits { get; set; }
        public int UpcomingVisits { get; set; }
        public int PendingConsents { get; set; }
        public int TotalEnrollments { get; set; }
        public int ExpiredConsents { get; set; }
        public int MissedVisits { get; set; }
        public int ExpiringConsentsThisMonth { get; set; }

        // ── Enrollment Trend (monthly, from first record) ───────
        public List<string> EnrollmentMonths { get; set; } = new();
        public List<int>    EnrollmentCounts { get; set; } = new();

        // ── Study Status Doughnut ────────────────────────────────
        public List<string> StudyStatusLabels { get; set; } = new();
        public List<int>    StudyStatusCounts { get; set; } = new();

        // ── Visit Status Doughnut ───────────────────────────────
        public List<string> VisitStatusLabels { get; set; } = new();
        public List<int>    VisitStatusCounts { get; set; } = new();

        // ── Enrollments per Study (bar chart) ───────────────────
        public List<string> StudyEnrollmentLabels { get; set; } = new();
        public List<int>    StudyEnrollmentCounts { get; set; } = new();

        // ── Age Distribution (bar chart) ────────────────────────
        public List<string> AgeGroupLabels { get; set; } = new();
        public List<int>    AgeGroupCounts { get; set; } = new();

        // ── Study Progress (enrolled vs target) ─────────────────
        public List<StudyProgressRow> StudyProgress { get; set; } = new();

        // ── Recent Activity ─────────────────────────────────────
        public List<Visit>            UpcomingVisitList  { get; set; } = new();
        public List<StudyEnrollment>  RecentEnrollments  { get; set; } = new();
    }

    public class StudyProgressRow
    {
        public string StudyCode     { get; set; } = string.Empty;
        public string StudyName     { get; set; } = string.Empty;
        public string Status        { get; set; } = string.Empty;
        public int    Enrolled      { get; set; }
        public int    MaxParticipants { get; set; }
        public double Percentage    => MaxParticipants > 0
            ? Math.Round((double)Enrolled / MaxParticipants * 100, 1)
            : 0;
    }
}
