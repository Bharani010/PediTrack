-- =============================================================================
-- Stored Procedure: usp_GetEnrollmentSummaryByStudy
-- Purpose: Returns enrollment and visit summary per study, with optional
--          filtering by StudyId and/or Status.
-- Parameters:
--   @StudyId  INT          = NULL  -- filter to a specific study (optional)
--   @Status   NVARCHAR(30) = NULL  -- filter by study status (optional)
-- =============================================================================
IF OBJECT_ID('dbo.usp_GetEnrollmentSummaryByStudy', 'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_GetEnrollmentSummaryByStudy;
GO

CREATE PROCEDURE dbo.usp_GetEnrollmentSummaryByStudy
    @StudyId  INT           = NULL,
    @Status   NVARCHAR(30)  = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        s.StudyCode,
        s.StudyName,
        s.Status,
        s.Phase,
        ISNULL(s.MaxParticipants, 0)                                    AS MaxParticipants,

        -- Total enrolled (all statuses)
        COUNT(DISTINCT se.StudyEnrollmentId)                            AS TotalEnrolled,

        -- Active participants
        COUNT(DISTINCT CASE WHEN se.Status = 'Active'    THEN se.StudyEnrollmentId END) AS ActiveParticipants,
        COUNT(DISTINCT CASE WHEN se.Status = 'Completed' THEN se.StudyEnrollmentId END) AS CompletedParticipants,
        COUNT(DISTINCT CASE WHEN se.Status = 'Withdrawn' THEN se.StudyEnrollmentId END) AS WithdrawnParticipants,

        -- Visit summary
        SUM(CASE WHEN v.Status = 'Completed' THEN 1 ELSE 0 END)        AS CompletedVisits,
        SUM(CASE WHEN v.Status = 'Missed'    THEN 1 ELSE 0 END)        AS MissedVisits,

        -- Enrollment percentage
        CAST(
            CASE
                WHEN ISNULL(s.MaxParticipants, 0) = 0 THEN 0.00
                ELSE
                    CAST(COUNT(DISTINCT se.StudyEnrollmentId) AS DECIMAL(10,2))
                    / CAST(s.MaxParticipants AS DECIMAL(10,2))
                    * 100.0
            END
        AS DECIMAL(5,2))                                                AS EnrollmentPct

    FROM dbo.Studies s
    LEFT JOIN dbo.StudyEnrollments se ON se.StudyId = s.StudyId
    LEFT JOIN dbo.Visits           v  ON v.StudyId  = s.StudyId

    WHERE
        -- Optional study ID filter
        (@StudyId IS NULL OR s.StudyId = @StudyId)
        -- Optional status filter
    AND (@Status  IS NULL OR s.Status  = @Status)

    GROUP BY
        s.StudyId,
        s.StudyCode,
        s.StudyName,
        s.Status,
        s.Phase,
        s.MaxParticipants

    ORDER BY
        TotalEnrolled DESC,
        s.StudyCode;
END;
GO
