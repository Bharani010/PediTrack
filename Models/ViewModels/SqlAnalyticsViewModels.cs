namespace PediTrack.Models.ViewModels
{
    // ── Result type for vw_StudyEnrollmentSummary (DB view) ──────────────────
    public class StudyEnrollmentSummaryRow
    {
        public int     StudyId              { get; set; }
        public string  StudyCode            { get; set; } = string.Empty;
        public string  StudyName            { get; set; } = string.Empty;
        public string  StudyStatus          { get; set; } = string.Empty;
        public string? Phase                { get; set; }
        public int     MaxParticipants      { get; set; }
        public int     EnrolledCount        { get; set; }
        public int     ActiveEnrolled       { get; set; }
        public int     CompletedEnrolled    { get; set; }
        public int     WithdrawnCount       { get; set; }
        public int     TotalVisits          { get; set; }
        public int     CompletedVisits      { get; set; }
        public int     MissedVisits         { get; set; }
        public int     UpcomingVisits       { get; set; }
        public int     TotalConsents        { get; set; }
        public int     ActiveConsents       { get; set; }
        public decimal EnrollmentPercentage { get; set; }
    }

    // ── Result type for usp_GetEnrollmentSummaryByStudy (stored procedure) ──
    public class EnrollmentSummaryResult
    {
        public string  StudyCode              { get; set; } = string.Empty;
        public string  StudyName              { get; set; } = string.Empty;
        public string  Status                 { get; set; } = string.Empty;
        public string? Phase                  { get; set; }
        public int     MaxParticipants        { get; set; }
        public int     TotalEnrolled          { get; set; }
        public int     ActiveParticipants     { get; set; }
        public int     CompletedParticipants  { get; set; }
        public int     WithdrawnParticipants  { get; set; }
        public int     CompletedVisits        { get; set; }
        public int     MissedVisits           { get; set; }
        public decimal EnrollmentPct          { get; set; }
    }

    // ── Result type for usp_GetParticipantVisitHistory (stored procedure) ───
    public class ParticipantVisitHistoryRow
    {
        public string    MRN              { get; set; } = string.Empty;
        public string    ParticipantName  { get; set; } = string.Empty;
        public string    StudyCode        { get; set; } = string.Empty;
        public string    StudyName        { get; set; } = string.Empty;
        public string    VisitType        { get; set; } = string.Empty;
        public int?      VisitNumber      { get; set; }
        public DateTime  ScheduledDate    { get; set; }
        public DateTime? CompletedDate    { get; set; }
        public string    VisitStatus      { get; set; } = string.Empty;
        public string?   Location         { get; set; }
        public string    AssignedStaff    { get; set; } = string.Empty;
        public string?   Notes            { get; set; }
        public string?   EnrollmentStatus { get; set; }
        public string?   SubjectId        { get; set; }
    }

    // ── ViewModel for the SqlAnalytics page ──────────────────────────────────
    public class SqlAnalyticsViewModel
    {
        public List<StudyEnrollmentSummaryRow> Summary   { get; set; } = new();
        public List<EnrollmentSummaryResult>   SpResults { get; set; } = new();
        public int?   FilterStudyId     { get; set; }
        public string? FilterStudyStatus { get; set; }
    }
}
