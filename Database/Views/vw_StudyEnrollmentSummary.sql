-- =============================================================================
-- View:   vw_StudyEnrollmentSummary
-- Purpose: Aggregates enrollment and visit statistics per study.
--          Used by the SQL Analytics page via EF Core keyless entity.
-- =============================================================================
IF OBJECT_ID('dbo.vw_StudyEnrollmentSummary', 'V') IS NOT NULL
    DROP VIEW dbo.vw_StudyEnrollmentSummary;
GO

CREATE VIEW dbo.vw_StudyEnrollmentSummary
AS
SELECT
    s.StudyId,
    s.StudyCode,
    s.StudyName,
    s.Status                                            AS StudyStatus,
    s.Phase,
    ISNULL(s.MaxParticipants, 0)                        AS MaxParticipants,

    -- Enrollment counts
    COUNT(DISTINCT se.StudyEnrollmentId)                AS EnrolledCount,
    COUNT(DISTINCT CASE WHEN se.Status = 'Active'     THEN se.StudyEnrollmentId END) AS ActiveEnrolled,
    COUNT(DISTINCT CASE WHEN se.Status = 'Completed'  THEN se.StudyEnrollmentId END) AS CompletedEnrolled,
    COUNT(DISTINCT CASE WHEN se.Status = 'Withdrawn'  THEN se.StudyEnrollmentId END) AS WithdrawnCount,

    -- Visit counts
    COUNT(v.VisitId)                                    AS TotalVisits,
    SUM(CASE WHEN v.Status = 'Completed'  THEN 1 ELSE 0 END) AS CompletedVisits,
    SUM(CASE WHEN v.Status = 'Missed'     THEN 1 ELSE 0 END) AS MissedVisits,
    SUM(CASE WHEN v.Status = 'Scheduled'
             AND v.ScheduledDate >= CAST(GETDATE() AS DATE)
             THEN 1 ELSE 0 END)                         AS UpcomingVisits,

    -- Consent counts
    COUNT(DISTINCT cf.ConsentFormId)                    AS TotalConsents,
    COUNT(DISTINCT CASE WHEN cf.Status = 'Active' THEN cf.ConsentFormId END) AS ActiveConsents,

    -- Enrollment percentage of MaxParticipants
    CAST(
        CASE
            WHEN ISNULL(s.MaxParticipants, 0) = 0 THEN 0.0
            ELSE CAST(COUNT(DISTINCT se.StudyEnrollmentId) AS DECIMAL(10,2))
                 / CAST(s.MaxParticipants AS DECIMAL(10,2)) * 100.0
        END
    AS DECIMAL(5,2))                                    AS EnrollmentPercentage

FROM dbo.Studies s
LEFT JOIN dbo.StudyEnrollments se ON se.StudyId = s.StudyId
LEFT JOIN dbo.Visits           v  ON v.StudyId  = s.StudyId
LEFT JOIN dbo.ConsentForms     cf ON cf.StudyId = s.StudyId

GROUP BY
    s.StudyId,
    s.StudyCode,
    s.StudyName,
    s.Status,
    s.Phase,
    s.MaxParticipants;
GO
