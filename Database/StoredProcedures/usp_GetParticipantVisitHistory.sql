-- =============================================================================
-- Stored Procedure: usp_GetParticipantVisitHistory
-- Purpose: Returns the full visit history for a participant, with optional
--          filtering to a specific study.
-- Parameters:
--   @ParticipantId  INT          (required) -- the participant to query
--   @StudyId        INT = NULL              -- optional study filter
-- =============================================================================
IF OBJECT_ID('dbo.usp_GetParticipantVisitHistory', 'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_GetParticipantVisitHistory;
GO

CREATE PROCEDURE dbo.usp_GetParticipantVisitHistory
    @ParticipantId  INT,
    @StudyId        INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        -- Participant identity
        p.MRN,
        p.FirstName + ' ' + p.LastName                                     AS ParticipantName,

        -- Study info
        s.StudyCode,
        s.StudyName,

        -- Visit details
        v.VisitType,
        v.VisitNumber,
        v.ScheduledDate,
        v.CompletedDate,
        v.Status                                                            AS VisitStatus,
        v.Location,

        -- Assigned staff (may be NULL if SetNull FK fired)
        ISNULL(i.FirstName + ' ' + i.LastName, 'Unassigned')               AS AssignedStaff,
        v.Notes,

        -- Enrollment context
        se.Status                                                           AS EnrollmentStatus,
        se.SubjectId

    FROM dbo.Visits v
    INNER JOIN dbo.Participants     p  ON p.ParticipantId = v.ParticipantId
    INNER JOIN dbo.Studies          s  ON s.StudyId       = v.StudyId
    LEFT  JOIN dbo.Investigators    i  ON i.InvestigatorId = v.AssignedStaffId
    LEFT  JOIN dbo.StudyEnrollments se ON se.ParticipantId = v.ParticipantId
                                      AND se.StudyId       = v.StudyId

    WHERE
        v.ParticipantId = @ParticipantId
    AND (@StudyId IS NULL OR v.StudyId = @StudyId)

    ORDER BY
        s.StudyCode,
        v.ScheduledDate;
END;
GO
