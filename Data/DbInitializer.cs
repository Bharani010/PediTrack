using PediTrack.Models;

namespace PediTrack.Data
{
    public static class DbInitializer
    {
        public static void Seed(ApplicationDbContext db)
        {
            // --- Investigators ---
            if (!db.Investigators.Any())
            {
                var investigators = new List<Investigator>
                {
                    new() { FirstName = "Sarah",  LastName = "Chen",     Title = "Dr.", Role = "PI",                 Email = "s.chen@meridian.org",      Phone = "713-555-0101", Department = "Pediatric Oncology",    Institution = "Meridian Children's Hospital", Status = "Active" },
                    new() { FirstName = "Marcus",  LastName = "Rivera",   Title = "Dr.", Role = "Co-Investigator",    Email = "m.rivera@meridian.org",     Phone = "713-555-0102", Department = "Pediatric Neurology",   Institution = "Meridian Children's Hospital", Status = "Active" },
                    new() { FirstName = "Aisha",   LastName = "Patel",    Title = "Ms.", Role = "Study Coordinator",  Email = "a.patel@meridian.org",      Phone = "713-555-0103", Department = "Research Operations",   Institution = "Meridian Children's Hospital", Status = "Active" },
                    new() { FirstName = "James",   LastName = "Okoye",    Title = "Dr.", Role = "PI",                 Email = "j.okoye@meridian.org",      Phone = "713-555-0104", Department = "Pediatric Cardiology",  Institution = "Meridian Children's Hospital", Status = "Active" },
                    new() { FirstName = "Linda",   LastName = "Tran",     Title = "Ms.", Role = "Research Staff",     Email = "l.tran@meridian.org",       Phone = "713-555-0105", Department = "Research Operations",   Institution = "Meridian Children's Hospital", Status = "Active" },
                };
                db.Investigators.AddRange(investigators);
                db.SaveChanges();
            }

            // --- Studies ---
            if (!db.Studies.Any())
            {
                var pi1 = db.Investigators.First(i => i.LastName == "Chen");
                var pi2 = db.Investigators.First(i => i.LastName == "Okoye");
                var pi3 = db.Investigators.First(i => i.LastName == "Rivera");

                var studies = new List<Study>
                {
                    new() {
                        StudyCode = "MCH-ONC-001", StudyName = "Pediatric ALL Immunotherapy Response Study",
                        IRBNumber = "IRB-2023-0441", Description = "Evaluating immunotherapy response markers in pediatric acute lymphoblastic leukemia patients aged 2–17.",
                        StartDate = new DateTime(2023, 6, 1), EndDate = new DateTime(2026, 5, 31),
                        Status = "Active", MaxParticipants = 80, Sponsor = "NIH – National Cancer Institute",
                        Phase = "Phase II", PrincipalInvestigatorId = pi1.InvestigatorId
                    },
                    new() {
                        StudyCode = "MCH-CARD-002", StudyName = "Congenital Heart Defect Longitudinal Registry",
                        IRBNumber = "IRB-2022-0289", Description = "Long-term outcomes registry for children with surgically corrected congenital heart defects.",
                        StartDate = new DateTime(2022, 1, 15), EndDate = null,
                        Status = "Recruiting", MaxParticipants = 200, Sponsor = "American Heart Association",
                        Phase = "Observational", PrincipalInvestigatorId = pi2.InvestigatorId
                    },
                    new() {
                        StudyCode = "MCH-NEURO-003", StudyName = "Early Biomarkers in Pediatric Epilepsy",
                        IRBNumber = "IRB-2024-0112", Description = "Identifying serum biomarkers predictive of drug-resistant epilepsy onset in children under 12.",
                        StartDate = new DateTime(2024, 3, 1), EndDate = new DateTime(2027, 2, 28),
                        Status = "Active", MaxParticipants = 60, Sponsor = "Meridian Research Foundation",
                        Phase = "Phase I", PrincipalInvestigatorId = pi3.InvestigatorId
                    },
                    new() {
                        StudyCode = "MCH-ONC-004", StudyName = "Survivorship Nutrition Program – Closed",
                        IRBNumber = "IRB-2021-0055", Description = "Nutritional intervention study in pediatric cancer survivors.",
                        StartDate = new DateTime(2021, 9, 1), EndDate = new DateTime(2023, 8, 31),
                        Status = "Closed", MaxParticipants = 40, Sponsor = "Children's Cancer Fund",
                        Phase = "Phase II", PrincipalInvestigatorId = pi1.InvestigatorId
                    },
                };
                db.Studies.AddRange(studies);
                db.SaveChanges();
            }

            // --- Participants ---
            if (!db.Participants.Any())
            {
                var participants = new List<Participant>
                {
                    new() { MRN = "MCH-00001", FirstName = "Ethan",    LastName = "Brooks",    DateOfBirth = new DateTime(2015, 4, 12), Gender = "Male",   GuardianName = "Patricia Brooks",   GuardianPhone = "713-555-1001", GuardianEmail = "p.brooks@email.com",  Status = "Active",    EnrollmentDate = new DateTime(2023, 7, 5) },
                    new() { MRN = "MCH-00002", FirstName = "Sofia",    LastName = "Martinez",  DateOfBirth = new DateTime(2012, 8, 22), Gender = "Female", GuardianName = "Carlos Martinez",   GuardianPhone = "713-555-1002", GuardianEmail = "c.martinez@email.com",Status = "Active",    EnrollmentDate = new DateTime(2023, 7, 18) },
                    new() { MRN = "MCH-00003", FirstName = "Liam",     LastName = "Johnson",   DateOfBirth = new DateTime(2018, 1, 30), Gender = "Male",   GuardianName = "Diana Johnson",     GuardianPhone = "713-555-1003", GuardianEmail = null,                  Status = "Active",    EnrollmentDate = new DateTime(2023, 8, 2) },
                    new() { MRN = "MCH-00004", FirstName = "Aaliyah",  LastName = "Washington",DateOfBirth = new DateTime(2010, 11, 5), Gender = "Female", GuardianName = "Robert Washington", GuardianPhone = "713-555-1004", GuardianEmail = "r.wash@email.com",    Status = "Active",    EnrollmentDate = new DateTime(2022, 3, 14) },
                    new() { MRN = "MCH-00005", FirstName = "Noah",     LastName = "Kim",       DateOfBirth = new DateTime(2016, 6, 19), Gender = "Male",   GuardianName = "Sun-Yi Kim",        GuardianPhone = "713-555-1005", GuardianEmail = "sunyi.kim@email.com", Status = "Active",    EnrollmentDate = new DateTime(2022, 4, 1) },
                    new() { MRN = "MCH-00006", FirstName = "Isabella", LastName = "Thompson",  DateOfBirth = new DateTime(2014, 3, 7),  Gender = "Female", GuardianName = "Mark Thompson",     GuardianPhone = "713-555-1006", GuardianEmail = "m.thompson@email.com",Status = "Withdrawn", EnrollmentDate = new DateTime(2023, 9, 10) },
                    new() { MRN = "MCH-00007", FirstName = "Mason",    LastName = "Garcia",    DateOfBirth = new DateTime(2019, 7, 25), Gender = "Male",   GuardianName = "Elena Garcia",      GuardianPhone = "713-555-1007", GuardianEmail = null,                  Status = "Active",    EnrollmentDate = new DateTime(2024, 1, 20) },
                    new() { MRN = "MCH-00008", FirstName = "Zoe",      LastName = "Pham",      DateOfBirth = new DateTime(2013, 9, 14), Gender = "Female", GuardianName = "Minh Pham",         GuardianPhone = "713-555-1008", GuardianEmail = "m.pham@email.com",    Status = "Active",    EnrollmentDate = new DateTime(2024, 2, 8) },
                    new() { MRN = "MCH-00009", FirstName = "Oliver",   LastName = "Nguyen",    DateOfBirth = new DateTime(2017, 12, 3), Gender = "Male",   GuardianName = "Lisa Nguyen",       GuardianPhone = "713-555-1009", GuardianEmail = "l.nguyen@email.com",  Status = "Completed", EnrollmentDate = new DateTime(2021, 10, 5) },
                    new() { MRN = "MCH-00010", FirstName = "Amara",    LastName = "Osei",      DateOfBirth = new DateTime(2011, 5, 28), Gender = "Female", GuardianName = "Kwame Osei",        GuardianPhone = "713-555-1010", GuardianEmail = "k.osei@email.com",    Status = "Active",    EnrollmentDate = new DateTime(2024, 4, 3) },
                };
                db.Participants.AddRange(participants);
                db.SaveChanges();
            }

            // --- Study Enrollments ---
            if (!db.StudyEnrollments.Any())
            {
                var allParticipants = db.Participants.ToList();
                var studies = db.Studies.ToList();
                var oncStudy   = studies.First(s => s.StudyCode == "MCH-ONC-001");
                var cardStudy  = studies.First(s => s.StudyCode == "MCH-CARD-002");
                var neuroStudy = studies.First(s => s.StudyCode == "MCH-NEURO-003");
                var closedStudy = studies.First(s => s.StudyCode == "MCH-ONC-004");

                var enrollments = new List<StudyEnrollment>
                {
                    new() { ParticipantId = allParticipants[0].ParticipantId, StudyId = oncStudy.StudyId,    EnrollmentDate = new DateTime(2023, 7, 10),  Status = "Active",    SubjectId = "ALL-001" },
                    new() { ParticipantId = allParticipants[1].ParticipantId, StudyId = oncStudy.StudyId,    EnrollmentDate = new DateTime(2023, 7, 20),  Status = "Active",    SubjectId = "ALL-002" },
                    new() { ParticipantId = allParticipants[2].ParticipantId, StudyId = oncStudy.StudyId,    EnrollmentDate = new DateTime(2023, 8, 5),   Status = "Active",    SubjectId = "ALL-003" },
                    new() { ParticipantId = allParticipants[3].ParticipantId, StudyId = cardStudy.StudyId,   EnrollmentDate = new DateTime(2022, 3, 20),  Status = "Active",    SubjectId = "CHD-001" },
                    new() { ParticipantId = allParticipants[4].ParticipantId, StudyId = cardStudy.StudyId,   EnrollmentDate = new DateTime(2022, 4, 5),   Status = "Active",    SubjectId = "CHD-002" },
                    new() { ParticipantId = allParticipants[5].ParticipantId, StudyId = oncStudy.StudyId,    EnrollmentDate = new DateTime(2023, 9, 15),  Status = "Withdrawn", SubjectId = "ALL-004", WithdrawalDate = new DateTime(2024, 1, 10), WithdrawalReason = "Guardian request" },
                    new() { ParticipantId = allParticipants[6].ParticipantId, StudyId = neuroStudy.StudyId,  EnrollmentDate = new DateTime(2024, 1, 25),  Status = "Active",    SubjectId = "EPI-001" },
                    new() { ParticipantId = allParticipants[7].ParticipantId, StudyId = neuroStudy.StudyId,  EnrollmentDate = new DateTime(2024, 2, 12),  Status = "Active",    SubjectId = "EPI-002" },
                    new() { ParticipantId = allParticipants[8].ParticipantId, StudyId = closedStudy.StudyId, EnrollmentDate = new DateTime(2021, 10, 10), Status = "Completed", SubjectId = "NUT-001" },
                    new() { ParticipantId = allParticipants[9].ParticipantId, StudyId = cardStudy.StudyId,   EnrollmentDate = new DateTime(2024, 4, 8),   Status = "Active",    SubjectId = "CHD-003" },
                };
                db.StudyEnrollments.AddRange(enrollments);
                db.SaveChanges();
            }

            // --- Visits ---
            if (!db.Visits.Any())
            {
                var allParticipants = db.Participants.ToList();
                var studies = db.Studies.ToList();
                var staff = db.Investigators.First(i => i.LastName == "Patel");
                var oncStudy   = studies.First(s => s.StudyCode == "MCH-ONC-001");
                var cardStudy  = studies.First(s => s.StudyCode == "MCH-CARD-002");
                var neuroStudy = studies.First(s => s.StudyCode == "MCH-NEURO-003");

                var visits = new List<Visit>
                {
                    new() { ParticipantId = allParticipants[0].ParticipantId, StudyId = oncStudy.StudyId,   VisitType = "Baseline",   ScheduledDate = new DateTime(2023, 7, 15),  CompletedDate = new DateTime(2023, 7, 15),  Status = "Completed", Location = "Clinic A", AssignedStaffId = staff.InvestigatorId, VisitNumber = 1 },
                    new() { ParticipantId = allParticipants[0].ParticipantId, StudyId = oncStudy.StudyId,   VisitType = "Follow-up",  ScheduledDate = new DateTime(2023, 10, 15), CompletedDate = new DateTime(2023, 10, 16), Status = "Completed", Location = "Clinic A", AssignedStaffId = staff.InvestigatorId, VisitNumber = 2 },
                    new() { ParticipantId = allParticipants[0].ParticipantId, StudyId = oncStudy.StudyId,   VisitType = "Follow-up",  ScheduledDate = DateTime.Today.AddDays(7),  CompletedDate = null,                       Status = "Scheduled", Location = "Clinic A", AssignedStaffId = staff.InvestigatorId, VisitNumber = 3 },
                    new() { ParticipantId = allParticipants[1].ParticipantId, StudyId = oncStudy.StudyId,   VisitType = "Baseline",   ScheduledDate = new DateTime(2023, 7, 25),  CompletedDate = new DateTime(2023, 7, 25),  Status = "Completed", Location = "Clinic A", AssignedStaffId = staff.InvestigatorId, VisitNumber = 1 },
                    new() { ParticipantId = allParticipants[1].ParticipantId, StudyId = oncStudy.StudyId,   VisitType = "Follow-up",  ScheduledDate = DateTime.Today.AddDays(14), CompletedDate = null,                       Status = "Scheduled", Location = "Clinic A", AssignedStaffId = staff.InvestigatorId, VisitNumber = 2 },
                    new() { ParticipantId = allParticipants[3].ParticipantId, StudyId = cardStudy.StudyId,  VisitType = "Baseline",   ScheduledDate = new DateTime(2022, 3, 25),  CompletedDate = new DateTime(2022, 3, 25),  Status = "Completed", Location = "Cardiology Lab", AssignedStaffId = staff.InvestigatorId, VisitNumber = 1 },
                    new() { ParticipantId = allParticipants[3].ParticipantId, StudyId = cardStudy.StudyId,  VisitType = "Annual",     ScheduledDate = new DateTime(2023, 3, 25),  CompletedDate = new DateTime(2023, 3, 26),  Status = "Completed", Location = "Cardiology Lab", AssignedStaffId = staff.InvestigatorId, VisitNumber = 2 },
                    new() { ParticipantId = allParticipants[3].ParticipantId, StudyId = cardStudy.StudyId,  VisitType = "Annual",     ScheduledDate = DateTime.Today.AddDays(3),  CompletedDate = null,                       Status = "Scheduled", Location = "Cardiology Lab", AssignedStaffId = staff.InvestigatorId, VisitNumber = 3 },
                    new() { ParticipantId = allParticipants[6].ParticipantId, StudyId = neuroStudy.StudyId, VisitType = "Screening",  ScheduledDate = new DateTime(2024, 1, 20),  CompletedDate = new DateTime(2024, 1, 20),  Status = "Completed", Location = "Neuro Unit", AssignedStaffId = staff.InvestigatorId, VisitNumber = 1 },
                    new() { ParticipantId = allParticipants[6].ParticipantId, StudyId = neuroStudy.StudyId, VisitType = "Baseline",   ScheduledDate = new DateTime(2024, 2, 3),   CompletedDate = new DateTime(2024, 2, 3),   Status = "Completed", Location = "Neuro Unit", AssignedStaffId = staff.InvestigatorId, VisitNumber = 2 },
                    new() { ParticipantId = allParticipants[6].ParticipantId, StudyId = neuroStudy.StudyId, VisitType = "Follow-up",  ScheduledDate = DateTime.Today.AddDays(21), CompletedDate = null,                       Status = "Scheduled", Location = "Neuro Unit", AssignedStaffId = staff.InvestigatorId, VisitNumber = 3 },
                    new() { ParticipantId = allParticipants[2].ParticipantId, StudyId = oncStudy.StudyId,   VisitType = "Baseline",   ScheduledDate = new DateTime(2023, 8, 10),  CompletedDate = null,                       Status = "Missed",   Location = "Clinic A", AssignedStaffId = staff.InvestigatorId, VisitNumber = 1, Notes = "No-show. Guardian notified." },
                };
                db.Visits.AddRange(visits);
                db.SaveChanges();
            }

            // --- Consent Forms ---
            if (!db.ConsentForms.Any())
            {
                var allParticipants = db.Participants.ToList();
                var studies = db.Studies.ToList();
                var oncStudy   = studies.First(s => s.StudyCode == "MCH-ONC-001");
                var cardStudy  = studies.First(s => s.StudyCode == "MCH-CARD-002");
                var neuroStudy = studies.First(s => s.StudyCode == "MCH-NEURO-003");

                var consents = new List<ConsentForm>
                {
                    new() { ParticipantId = allParticipants[0].ParticipantId, StudyId = oncStudy.StudyId,   ConsentDate = new DateTime(2023, 7, 5),  ExpirationDate = new DateTime(2025, 7, 5),  Version = "2.1", SignedByGuardian = true,  WitnessName = "Aisha Patel",  Status = "Active" },
                    new() { ParticipantId = allParticipants[1].ParticipantId, StudyId = oncStudy.StudyId,   ConsentDate = new DateTime(2023, 7, 18), ExpirationDate = new DateTime(2025, 7, 18), Version = "2.1", SignedByGuardian = true,  WitnessName = "Aisha Patel",  Status = "Active" },
                    new() { ParticipantId = allParticipants[2].ParticipantId, StudyId = oncStudy.StudyId,   ConsentDate = new DateTime(2023, 8, 2),  ExpirationDate = new DateTime(2025, 8, 2),  Version = "2.1", SignedByGuardian = true,  WitnessName = "Linda Tran",   Status = "Active" },
                    new() { ParticipantId = allParticipants[3].ParticipantId, StudyId = cardStudy.StudyId,  ConsentDate = new DateTime(2022, 3, 14), ExpirationDate = new DateTime(2024, 3, 14), Version = "1.0", SignedByGuardian = true,  WitnessName = "Aisha Patel",  Status = "Expired", ReconsentRequired = true },
                    new() { ParticipantId = allParticipants[4].ParticipantId, StudyId = cardStudy.StudyId,  ConsentDate = new DateTime(2022, 4, 1),  ExpirationDate = new DateTime(2024, 4, 1),  Version = "1.0", SignedByGuardian = true,  WitnessName = "Linda Tran",   Status = "Expired", ReconsentRequired = true },
                    new() { ParticipantId = allParticipants[6].ParticipantId, StudyId = neuroStudy.StudyId, ConsentDate = new DateTime(2024, 1, 20), ExpirationDate = new DateTime(2026, 1, 20), Version = "1.2", SignedByGuardian = true,  WitnessName = "Aisha Patel",  Status = "Active" },
                    new() { ParticipantId = allParticipants[7].ParticipantId, StudyId = neuroStudy.StudyId, ConsentDate = new DateTime(2024, 2, 8),  ExpirationDate = new DateTime(2026, 2, 8),  Version = "1.2", SignedByGuardian = true,  WitnessName = "Aisha Patel",  Status = "Active" },
                    new() { ParticipantId = allParticipants[9].ParticipantId, StudyId = cardStudy.StudyId,  ConsentDate = new DateTime(2024, 4, 3),  ExpirationDate = new DateTime(2026, 4, 3),  Version = "1.1", SignedByGuardian = false, WitnessName = null,           Status = "Active", Notes = "Awaiting guardian signature" },
                };
                db.ConsentForms.AddRange(consents);
                db.SaveChanges();
            }

            // --- Data Dictionary ---
            if (!db.DataDictionaryEntries.Any())
            {
                var entries = new List<DataDictionaryEntry>
                {
                    // Participants
                    new() { TableName = "Participants", ColumnName = "ParticipantId", DataType = "int",           IsPrimaryKey = true,  IsNullable = false, Description = "Auto-incremented unique identifier for each participant." },
                    new() { TableName = "Participants", ColumnName = "MRN",           DataType = "nvarchar(20)",   IsPrimaryKey = false, IsNullable = false, Description = "Medical Record Number — unique per patient.", BusinessRule = "Must be unique across all participants." },
                    new() { TableName = "Participants", ColumnName = "FirstName",      DataType = "nvarchar(100)", IsPrimaryKey = false, IsNullable = false, Description = "Participant's legal first name." },
                    new() { TableName = "Participants", ColumnName = "LastName",       DataType = "nvarchar(100)", IsPrimaryKey = false, IsNullable = false, Description = "Participant's legal last name." },
                    new() { TableName = "Participants", ColumnName = "DateOfBirth",    DataType = "date",          IsPrimaryKey = false, IsNullable = false, Description = "Participant's date of birth (YYYY-MM-DD)." },
                    new() { TableName = "Participants", ColumnName = "Gender",         DataType = "nvarchar(20)",  IsPrimaryKey = false, IsNullable = false, Description = "Participant's gender.", AllowedValues = "Male, Female, Non-binary, Prefer not to say" },
                    new() { TableName = "Participants", ColumnName = "Status",         DataType = "nvarchar(20)",  IsPrimaryKey = false, IsNullable = false, Description = "Current enrollment status.", AllowedValues = "Active, Withdrawn, Completed, Pending" },
                    // Studies
                    new() { TableName = "Studies",      ColumnName = "StudyId",        DataType = "int",           IsPrimaryKey = true,  IsNullable = false, Description = "Auto-incremented unique identifier for each study." },
                    new() { TableName = "Studies",      ColumnName = "StudyCode",      DataType = "nvarchar(20)",  IsPrimaryKey = false, IsNullable = false, Description = "Short internal code for the study.", BusinessRule = "Must be unique. Format: MCH-[DEPT]-[NNN]" },
                    new() { TableName = "Studies",      ColumnName = "IRBNumber",      DataType = "nvarchar(50)",  IsPrimaryKey = false, IsNullable = true,  Description = "IRB approval number issued by the Institutional Review Board." },
                    new() { TableName = "Studies",      ColumnName = "Status",         DataType = "nvarchar(30)",  IsPrimaryKey = false, IsNullable = false, Description = "Current status of the study.", AllowedValues = "Active, Recruiting, Closed, Suspended" },
                    // Visits
                    new() { TableName = "Visits",       ColumnName = "VisitId",        DataType = "int",           IsPrimaryKey = true,  IsNullable = false, Description = "Unique identifier for each visit." },
                    new() { TableName = "Visits",       ColumnName = "VisitType",      DataType = "nvarchar(100)", IsPrimaryKey = false, IsNullable = false, Description = "Type/purpose of the visit.", AllowedValues = "Screening, Baseline, Follow-up, Annual, Final, Unscheduled" },
                    new() { TableName = "Visits",       ColumnName = "Status",         DataType = "nvarchar(30)",  IsPrimaryKey = false, IsNullable = false, Description = "Current status of the visit.", AllowedValues = "Scheduled, Completed, Missed, Cancelled, Rescheduled" },
                    // ConsentForms
                    new() { TableName = "ConsentForms", ColumnName = "ConsentFormId",  DataType = "int",           IsPrimaryKey = true,  IsNullable = false, Description = "Unique identifier for each consent record." },
                    new() { TableName = "ConsentForms", ColumnName = "Version",        DataType = "nvarchar(20)",  IsPrimaryKey = false, IsNullable = false, Description = "Version number of the consent form used.", BusinessRule = "Must match an IRB-approved form version." },
                    new() { TableName = "ConsentForms", ColumnName = "Status",         DataType = "nvarchar(30)",  IsPrimaryKey = false, IsNullable = false, Description = "Status of the consent.", AllowedValues = "Active, Expired, Revoked, Superseded" },
                };
                db.DataDictionaryEntries.AddRange(entries);
                db.SaveChanges();
            }
        }
    }
}
