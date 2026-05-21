using Microsoft.EntityFrameworkCore;
using PediTrack.Models;

namespace PediTrack.Data
{
    public static class DbInitializer
    {
        public static void Seed(ApplicationDbContext db)
        {
            // Create SQL Server objects (view + stored procedures) first.
            // Wrapped in try-catch so the app still starts on InMemory or limited SQL editions.
            CreateSqlObjects(db);

            // All dates are relative to today so dashboard charts always show real data.
            var today = DateTime.Today;

            // --- Investigators ---
            if (!db.Investigators.Any())
            {
                db.Investigators.AddRange(new List<Investigator>
                {
                    new() { FirstName = "Sarah",  LastName = "Chen",    Title = "Dr.", Role = "Principal Investigator", Email = "s.chen@meridian.org",    Phone = "713-555-0101", Department = "Pediatric Oncology",   Institution = "Meridian Children's Hospital", Status = "Active" },
                    new() { FirstName = "Marcus", LastName = "Rivera",  Title = "Dr.", Role = "Co-Investigator",        Email = "m.rivera@meridian.org",   Phone = "713-555-0102", Department = "Pediatric Neurology",  Institution = "Meridian Children's Hospital", Status = "Active" },
                    new() { FirstName = "Aisha",  LastName = "Patel",   Title = "Ms.", Role = "Study Coordinator",      Email = "a.patel@meridian.org",    Phone = "713-555-0103", Department = "Research Operations",  Institution = "Meridian Children's Hospital", Status = "Active" },
                    new() { FirstName = "James",  LastName = "Okoye",   Title = "Dr.", Role = "Principal Investigator", Email = "j.okoye@meridian.org",    Phone = "713-555-0104", Department = "Pediatric Cardiology", Institution = "Meridian Children's Hospital", Status = "Active" },
                    new() { FirstName = "Linda",  LastName = "Tran",    Title = "Ms.", Role = "Research Staff",         Email = "l.tran@meridian.org",     Phone = "713-555-0105", Department = "Research Operations",  Institution = "Meridian Children's Hospital", Status = "Active" },
                    new() { FirstName = "David",  LastName = "Hwang",   Title = "Dr.", Role = "Co-Investigator",        Email = "d.hwang@meridian.org",    Phone = "713-555-0106", Department = "Pediatric Oncology",   Institution = "Meridian Children's Hospital", Status = "Active" },
                });
                db.SaveChanges();
            }

            // --- Studies ---
            if (!db.Studies.Any())
            {
                var pi1 = db.Investigators.First(i => i.LastName == "Chen");
                var pi2 = db.Investigators.First(i => i.LastName == "Okoye");
                var pi3 = db.Investigators.First(i => i.LastName == "Rivera");

                db.Studies.AddRange(new List<Study>
                {
                    new() {
                        StudyCode = "MCH-ONC-001", StudyName = "Pediatric ALL Immunotherapy Response Study",
                        IRBNumber = "IRB-2023-0441",
                        Description = "Evaluating immunotherapy response markers in pediatric acute lymphoblastic leukemia patients aged 2–17.",
                        StartDate  = today.AddMonths(-24), EndDate = today.AddMonths(12),
                        Status = "Active", MaxParticipants = 80, Sponsor = "NIH – National Cancer Institute",
                        Phase = "Phase II", PrincipalInvestigatorId = pi1.InvestigatorId
                    },
                    new() {
                        StudyCode = "MCH-CARD-002", StudyName = "Congenital Heart Defect Longitudinal Registry",
                        IRBNumber = "IRB-2022-0289",
                        Description = "Long-term outcomes registry for children with surgically corrected congenital heart defects.",
                        StartDate  = today.AddMonths(-36), EndDate = null,
                        Status = "Recruiting", MaxParticipants = 200, Sponsor = "American Heart Association",
                        Phase = "Observational", PrincipalInvestigatorId = pi2.InvestigatorId
                    },
                    new() {
                        StudyCode = "MCH-NEURO-003", StudyName = "Early Biomarkers in Pediatric Epilepsy",
                        IRBNumber = "IRB-2024-0112",
                        Description = "Identifying serum biomarkers predictive of drug-resistant epilepsy onset in children under 12.",
                        StartDate  = today.AddMonths(-15), EndDate = today.AddMonths(21),
                        Status = "Active", MaxParticipants = 60, Sponsor = "Meridian Research Foundation",
                        Phase = "Phase I", PrincipalInvestigatorId = pi3.InvestigatorId
                    },
                    new() {
                        StudyCode = "MCH-ONC-004", StudyName = "Survivorship Nutrition Program",
                        IRBNumber = "IRB-2021-0055",
                        Description = "Nutritional intervention study in pediatric cancer survivors — now closed.",
                        StartDate  = today.AddMonths(-42), EndDate = today.AddMonths(-18),
                        Status = "Closed", MaxParticipants = 40, Sponsor = "Children's Cancer Fund",
                        Phase = "Phase II", PrincipalInvestigatorId = pi1.InvestigatorId
                    },
                });
                db.SaveChanges();
            }

            // --- Participants ---
            if (!db.Participants.Any())
            {
                db.Participants.AddRange(new List<Participant>
                {
                    new() { MRN = "MCH-00001", FirstName = "Ethan",    LastName = "Brooks",     DateOfBirth = today.AddYears(-11).AddMonths(-1),  Gender = "Male",   GuardianName = "Patricia Brooks",    GuardianPhone = "713-555-1001", GuardianEmail = "p.brooks@email.com",   Status = "Active",    EnrollmentDate = today.AddMonths(-23) },
                    new() { MRN = "MCH-00002", FirstName = "Sofia",    LastName = "Martinez",   DateOfBirth = today.AddYears(-14).AddMonths(-3),  Gender = "Female", GuardianName = "Carlos Martinez",    GuardianPhone = "713-555-1002", GuardianEmail = "c.martinez@email.com", Status = "Active",    EnrollmentDate = today.AddMonths(-21) },
                    new() { MRN = "MCH-00003", FirstName = "Liam",     LastName = "Johnson",    DateOfBirth = today.AddYears(-8).AddMonths(-5),   Gender = "Male",   GuardianName = "Diana Johnson",      GuardianPhone = "713-555-1003", GuardianEmail = null,                   Status = "Active",    EnrollmentDate = today.AddMonths(-19) },
                    new() { MRN = "MCH-00004", FirstName = "Aaliyah",  LastName = "Washington", DateOfBirth = today.AddYears(-16).AddMonths(-7),  Gender = "Female", GuardianName = "Robert Washington",  GuardianPhone = "713-555-1004", GuardianEmail = "r.wash@email.com",     Status = "Active",    EnrollmentDate = today.AddMonths(-34) },
                    new() { MRN = "MCH-00005", FirstName = "Noah",     LastName = "Kim",        DateOfBirth = today.AddYears(-10).AddMonths(-2),  Gender = "Male",   GuardianName = "Sun-Yi Kim",         GuardianPhone = "713-555-1005", GuardianEmail = "sunyi.kim@email.com",  Status = "Active",    EnrollmentDate = today.AddMonths(-33) },
                    new() { MRN = "MCH-00006", FirstName = "Isabella", LastName = "Thompson",   DateOfBirth = today.AddYears(-12).AddMonths(-9),  Gender = "Female", GuardianName = "Mark Thompson",      GuardianPhone = "713-555-1006", GuardianEmail = "m.thompson@email.com", Status = "Withdrawn", EnrollmentDate = today.AddMonths(-17) },
                    new() { MRN = "MCH-00007", FirstName = "Mason",    LastName = "Garcia",     DateOfBirth = today.AddYears(-7),                 Gender = "Male",   GuardianName = "Elena Garcia",       GuardianPhone = "713-555-1007", GuardianEmail = null,                   Status = "Active",    EnrollmentDate = today.AddMonths(-13) },
                    new() { MRN = "MCH-00008", FirstName = "Zoe",      LastName = "Pham",       DateOfBirth = today.AddYears(-13).AddMonths(-4),  Gender = "Female", GuardianName = "Minh Pham",          GuardianPhone = "713-555-1008", GuardianEmail = "m.pham@email.com",     Status = "Active",    EnrollmentDate = today.AddMonths(-11) },
                    new() { MRN = "MCH-00009", FirstName = "Oliver",   LastName = "Nguyen",     DateOfBirth = today.AddYears(-9).AddMonths(-10),  Gender = "Male",   GuardianName = "Lisa Nguyen",        GuardianPhone = "713-555-1009", GuardianEmail = "l.nguyen@email.com",   Status = "Completed", EnrollmentDate = today.AddMonths(-38) },
                    new() { MRN = "MCH-00010", FirstName = "Amara",    LastName = "Osei",       DateOfBirth = today.AddYears(-15).AddMonths(-6),  Gender = "Female", GuardianName = "Kwame Osei",         GuardianPhone = "713-555-1010", GuardianEmail = "k.osei@email.com",     Status = "Active",    EnrollmentDate = today.AddMonths(-8) },
                    new() { MRN = "MCH-00011", FirstName = "James",    LastName = "Reyes",      DateOfBirth = today.AddYears(-6).AddMonths(-3),   Gender = "Male",   GuardianName = "Carmen Reyes",       GuardianPhone = "713-555-1011", GuardianEmail = "c.reyes@email.com",    Status = "Active",    EnrollmentDate = today.AddMonths(-5) },
                    new() { MRN = "MCH-00012", FirstName = "Priya",    LastName = "Sharma",     DateOfBirth = today.AddYears(-11).AddMonths(-8),  Gender = "Female", GuardianName = "Anita Sharma",       GuardianPhone = "713-555-1012", GuardianEmail = "a.sharma@email.com",   Status = "Active",    EnrollmentDate = today.AddMonths(-3) },
                });
                db.SaveChanges();
            }

            // --- Study Enrollments ---
            if (!db.StudyEnrollments.Any())
            {
                var parts  = db.Participants.OrderBy(p => p.ParticipantId).ToList();
                var oncS   = db.Studies.First(s => s.StudyCode == "MCH-ONC-001");
                var cardS  = db.Studies.First(s => s.StudyCode == "MCH-CARD-002");
                var neuroS = db.Studies.First(s => s.StudyCode == "MCH-NEURO-003");
                var closedS = db.Studies.First(s => s.StudyCode == "MCH-ONC-004");

                db.StudyEnrollments.AddRange(new List<StudyEnrollment>
                {
                    new() { ParticipantId = parts[0].ParticipantId, StudyId = oncS.StudyId,    EnrollmentDate = today.AddMonths(-23), Status = "Active",    SubjectId = "ALL-001" },
                    new() { ParticipantId = parts[1].ParticipantId, StudyId = oncS.StudyId,    EnrollmentDate = today.AddMonths(-21), Status = "Active",    SubjectId = "ALL-002" },
                    new() { ParticipantId = parts[2].ParticipantId, StudyId = oncS.StudyId,    EnrollmentDate = today.AddMonths(-19), Status = "Active",    SubjectId = "ALL-003" },
                    new() { ParticipantId = parts[3].ParticipantId, StudyId = cardS.StudyId,   EnrollmentDate = today.AddMonths(-34), Status = "Active",    SubjectId = "CHD-001" },
                    new() { ParticipantId = parts[4].ParticipantId, StudyId = cardS.StudyId,   EnrollmentDate = today.AddMonths(-33), Status = "Active",    SubjectId = "CHD-002" },
                    new() { ParticipantId = parts[5].ParticipantId, StudyId = oncS.StudyId,    EnrollmentDate = today.AddMonths(-17), Status = "Withdrawn", SubjectId = "ALL-004", WithdrawalDate = today.AddMonths(-10), WithdrawalReason = "Guardian request" },
                    new() { ParticipantId = parts[6].ParticipantId, StudyId = neuroS.StudyId,  EnrollmentDate = today.AddMonths(-13), Status = "Active",    SubjectId = "EPI-001" },
                    new() { ParticipantId = parts[7].ParticipantId, StudyId = neuroS.StudyId,  EnrollmentDate = today.AddMonths(-11), Status = "Active",    SubjectId = "EPI-002" },
                    new() { ParticipantId = parts[8].ParticipantId, StudyId = closedS.StudyId, EnrollmentDate = today.AddMonths(-38), Status = "Completed", SubjectId = "NUT-001" },
                    new() { ParticipantId = parts[9].ParticipantId, StudyId = cardS.StudyId,   EnrollmentDate = today.AddMonths(-8),  Status = "Active",    SubjectId = "CHD-003" },
                    new() { ParticipantId = parts[10].ParticipantId, StudyId = neuroS.StudyId, EnrollmentDate = today.AddMonths(-5),  Status = "Active",    SubjectId = "EPI-003" },
                    new() { ParticipantId = parts[11].ParticipantId, StudyId = cardS.StudyId,  EnrollmentDate = today.AddMonths(-3),  Status = "Active",    SubjectId = "CHD-004" },
                });
                db.SaveChanges();
            }

            // --- Visits ---
            if (!db.Visits.Any())
            {
                var parts  = db.Participants.OrderBy(p => p.ParticipantId).ToList();
                var oncS   = db.Studies.First(s => s.StudyCode == "MCH-ONC-001");
                var cardS  = db.Studies.First(s => s.StudyCode == "MCH-CARD-002");
                var neuroS = db.Studies.First(s => s.StudyCode == "MCH-NEURO-003");
                var staff  = db.Investigators.First(i => i.LastName == "Patel");

                db.Visits.AddRange(new List<Visit>
                {
                    // P0 – Ethan Brooks (MCH-ONC-001) – 3 visits: 2 completed, 1 upcoming
                    new() { ParticipantId = parts[0].ParticipantId, StudyId = oncS.StudyId,   VisitType = "Baseline",  ScheduledDate = today.AddMonths(-23), CompletedDate = today.AddMonths(-23), Status = "Completed", Location = "Clinic A", AssignedStaffId = staff.InvestigatorId, VisitNumber = 1 },
                    new() { ParticipantId = parts[0].ParticipantId, StudyId = oncS.StudyId,   VisitType = "Follow-up", ScheduledDate = today.AddMonths(-17), CompletedDate = today.AddMonths(-17), Status = "Completed", Location = "Clinic A", AssignedStaffId = staff.InvestigatorId, VisitNumber = 2 },
                    new() { ParticipantId = parts[0].ParticipantId, StudyId = oncS.StudyId,   VisitType = "Follow-up", ScheduledDate = today.AddDays(7),     CompletedDate = null,                 Status = "Scheduled", Location = "Clinic A", AssignedStaffId = staff.InvestigatorId, VisitNumber = 3 },

                    // P1 – Sofia Martinez (MCH-ONC-001) – 2 completed + 1 upcoming
                    new() { ParticipantId = parts[1].ParticipantId, StudyId = oncS.StudyId,   VisitType = "Baseline",  ScheduledDate = today.AddMonths(-21), CompletedDate = today.AddMonths(-21), Status = "Completed", Location = "Clinic A", AssignedStaffId = staff.InvestigatorId, VisitNumber = 1 },
                    new() { ParticipantId = parts[1].ParticipantId, StudyId = oncS.StudyId,   VisitType = "Follow-up", ScheduledDate = today.AddDays(14),    CompletedDate = null,                 Status = "Scheduled", Location = "Clinic A", AssignedStaffId = staff.InvestigatorId, VisitNumber = 2 },

                    // P2 – Liam Johnson (MCH-ONC-001) – 1 missed baseline
                    new() { ParticipantId = parts[2].ParticipantId, StudyId = oncS.StudyId,   VisitType = "Baseline",  ScheduledDate = today.AddMonths(-19), CompletedDate = null,                 Status = "Missed",    Location = "Clinic A", AssignedStaffId = staff.InvestigatorId, VisitNumber = 1, Notes = "No-show. Guardian notified." },
                    new() { ParticipantId = parts[2].ParticipantId, StudyId = oncS.StudyId,   VisitType = "Baseline",  ScheduledDate = today.AddDays(5),     CompletedDate = null,                 Status = "Scheduled", Location = "Clinic A", AssignedStaffId = staff.InvestigatorId, VisitNumber = 1, Notes = "Rescheduled after missed baseline." },

                    // P3 – Aaliyah Washington (MCH-CARD-002) – 2 completed + 1 upcoming
                    new() { ParticipantId = parts[3].ParticipantId, StudyId = cardS.StudyId,  VisitType = "Baseline",  ScheduledDate = today.AddMonths(-34), CompletedDate = today.AddMonths(-34), Status = "Completed", Location = "Cardiology Lab", AssignedStaffId = staff.InvestigatorId, VisitNumber = 1 },
                    new() { ParticipantId = parts[3].ParticipantId, StudyId = cardS.StudyId,  VisitType = "Annual",    ScheduledDate = today.AddMonths(-22), CompletedDate = today.AddMonths(-22), Status = "Completed", Location = "Cardiology Lab", AssignedStaffId = staff.InvestigatorId, VisitNumber = 2 },
                    new() { ParticipantId = parts[3].ParticipantId, StudyId = cardS.StudyId,  VisitType = "Annual",    ScheduledDate = today.AddDays(3),     CompletedDate = null,                 Status = "Scheduled", Location = "Cardiology Lab", AssignedStaffId = staff.InvestigatorId, VisitNumber = 3 },

                    // P4 – Noah Kim (MCH-CARD-002) – 1 completed + 1 upcoming
                    new() { ParticipantId = parts[4].ParticipantId, StudyId = cardS.StudyId,  VisitType = "Baseline",  ScheduledDate = today.AddMonths(-33), CompletedDate = today.AddMonths(-33), Status = "Completed", Location = "Cardiology Lab", AssignedStaffId = staff.InvestigatorId, VisitNumber = 1 },
                    new() { ParticipantId = parts[4].ParticipantId, StudyId = cardS.StudyId,  VisitType = "Annual",    ScheduledDate = today.AddDays(10),    CompletedDate = null,                 Status = "Scheduled", Location = "Cardiology Lab", AssignedStaffId = staff.InvestigatorId, VisitNumber = 2 },

                    // P6 – Mason Garcia (MCH-NEURO-003) – 2 completed + 1 upcoming
                    new() { ParticipantId = parts[6].ParticipantId, StudyId = neuroS.StudyId, VisitType = "Screening", ScheduledDate = today.AddMonths(-13), CompletedDate = today.AddMonths(-13), Status = "Completed", Location = "Neuro Unit",     AssignedStaffId = staff.InvestigatorId, VisitNumber = 1 },
                    new() { ParticipantId = parts[6].ParticipantId, StudyId = neuroS.StudyId, VisitType = "Baseline",  ScheduledDate = today.AddMonths(-12), CompletedDate = today.AddMonths(-12), Status = "Completed", Location = "Neuro Unit",     AssignedStaffId = staff.InvestigatorId, VisitNumber = 2 },
                    new() { ParticipantId = parts[6].ParticipantId, StudyId = neuroS.StudyId, VisitType = "Follow-up", ScheduledDate = today.AddDays(21),    CompletedDate = null,                 Status = "Scheduled", Location = "Neuro Unit",     AssignedStaffId = staff.InvestigatorId, VisitNumber = 3 },

                    // P7 – Zoe Pham (MCH-NEURO-003) – 1 completed + 1 upcoming
                    new() { ParticipantId = parts[7].ParticipantId, StudyId = neuroS.StudyId, VisitType = "Screening", ScheduledDate = today.AddMonths(-11), CompletedDate = today.AddMonths(-11), Status = "Completed", Location = "Neuro Unit",     AssignedStaffId = staff.InvestigatorId, VisitNumber = 1 },
                    new() { ParticipantId = parts[7].ParticipantId, StudyId = neuroS.StudyId, VisitType = "Baseline",  ScheduledDate = today.AddDays(2),     CompletedDate = null,                 Status = "Scheduled", Location = "Neuro Unit",     AssignedStaffId = staff.InvestigatorId, VisitNumber = 2 },

                    // P9 – Amara Osei (MCH-CARD-002) – upcoming only
                    new() { ParticipantId = parts[9].ParticipantId, StudyId = cardS.StudyId,  VisitType = "Baseline",  ScheduledDate = today.AddDays(4),     CompletedDate = null,                 Status = "Scheduled", Location = "Cardiology Lab", AssignedStaffId = staff.InvestigatorId, VisitNumber = 1 },
                });
                db.SaveChanges();
            }

            // --- Consent Forms ---
            if (!db.ConsentForms.Any())
            {
                var parts  = db.Participants.OrderBy(p => p.ParticipantId).ToList();
                var oncS   = db.Studies.First(s => s.StudyCode == "MCH-ONC-001");
                var cardS  = db.Studies.First(s => s.StudyCode == "MCH-CARD-002");
                var neuroS = db.Studies.First(s => s.StudyCode == "MCH-NEURO-003");

                db.ConsentForms.AddRange(new List<ConsentForm>
                {
                    new() { ParticipantId = parts[0].ParticipantId, StudyId = oncS.StudyId,   ConsentDate = today.AddMonths(-23), ExpirationDate = today.AddMonths(1),  Version = "2.1", SignedByGuardian = true,  WitnessName = "Aisha Patel", Status = "Active",  ReconsentRequired = true,  Notes = "Expires soon — renewal required" },
                    new() { ParticipantId = parts[1].ParticipantId, StudyId = oncS.StudyId,   ConsentDate = today.AddMonths(-21), ExpirationDate = today.AddMonths(3),  Version = "2.1", SignedByGuardian = true,  WitnessName = "Aisha Patel", Status = "Active" },
                    new() { ParticipantId = parts[2].ParticipantId, StudyId = oncS.StudyId,   ConsentDate = today.AddMonths(-19), ExpirationDate = today.AddMonths(5),  Version = "2.1", SignedByGuardian = true,  WitnessName = "Linda Tran",  Status = "Active" },
                    new() { ParticipantId = parts[3].ParticipantId, StudyId = cardS.StudyId,  ConsentDate = today.AddMonths(-34), ExpirationDate = today.AddMonths(-10),Version = "1.0", SignedByGuardian = true,  WitnessName = "Aisha Patel", Status = "Expired", ReconsentRequired = true },
                    new() { ParticipantId = parts[4].ParticipantId, StudyId = cardS.StudyId,  ConsentDate = today.AddMonths(-33), ExpirationDate = today.AddMonths(-9), Version = "1.0", SignedByGuardian = true,  WitnessName = "Linda Tran",  Status = "Expired", ReconsentRequired = true },
                    new() { ParticipantId = parts[6].ParticipantId, StudyId = neuroS.StudyId, ConsentDate = today.AddMonths(-13), ExpirationDate = today.AddMonths(11), Version = "1.2", SignedByGuardian = true,  WitnessName = "Aisha Patel", Status = "Active" },
                    new() { ParticipantId = parts[7].ParticipantId, StudyId = neuroS.StudyId, ConsentDate = today.AddMonths(-11), ExpirationDate = today.AddMonths(13), Version = "1.2", SignedByGuardian = true,  WitnessName = "Aisha Patel", Status = "Active" },
                    new() { ParticipantId = parts[9].ParticipantId, StudyId = cardS.StudyId,  ConsentDate = today.AddMonths(-8),  ExpirationDate = today.AddMonths(16), Version = "1.1", SignedByGuardian = false, WitnessName = null,          Status = "Active",  Notes = "Awaiting guardian signature" },
                    new() { ParticipantId = parts[10].ParticipantId,StudyId = neuroS.StudyId, ConsentDate = today.AddMonths(-5),  ExpirationDate = today.AddMonths(19), Version = "1.2", SignedByGuardian = true,  WitnessName = "Linda Tran",  Status = "Active" },
                    new() { ParticipantId = parts[11].ParticipantId,StudyId = cardS.StudyId,  ConsentDate = today.AddMonths(-3),  ExpirationDate = today.AddMonths(21), Version = "1.1", SignedByGuardian = true,  WitnessName = "Aisha Patel", Status = "Active" },
                });
                db.SaveChanges();
            }

            // --- Data Dictionary ---
            if (!db.DataDictionaryEntries.Any())
            {
                db.DataDictionaryEntries.AddRange(new List<DataDictionaryEntry>
                {
                    new() { TableName = "Participants", ColumnName = "ParticipantId",  DataType = "int",           IsPrimaryKey = true,  IsNullable = false, Description = "Auto-incremented unique identifier for each participant." },
                    new() { TableName = "Participants", ColumnName = "MRN",            DataType = "nvarchar(20)",  IsPrimaryKey = false, IsNullable = false, Description = "Medical Record Number — unique per patient.", BusinessRule = "Must be unique. Format: MCH-NNNNN." },
                    new() { TableName = "Participants", ColumnName = "FirstName",      DataType = "nvarchar(100)", IsPrimaryKey = false, IsNullable = false, Description = "Participant's legal first name." },
                    new() { TableName = "Participants", ColumnName = "LastName",       DataType = "nvarchar(100)", IsPrimaryKey = false, IsNullable = false, Description = "Participant's legal last name." },
                    new() { TableName = "Participants", ColumnName = "DateOfBirth",    DataType = "date",          IsPrimaryKey = false, IsNullable = false, Description = "Participant's date of birth." },
                    new() { TableName = "Participants", ColumnName = "Gender",         DataType = "nvarchar(20)",  IsPrimaryKey = false, IsNullable = false, Description = "Participant's gender.", AllowedValues = "Male, Female, Non-binary, Prefer not to say" },
                    new() { TableName = "Participants", ColumnName = "Status",         DataType = "nvarchar(20)",  IsPrimaryKey = false, IsNullable = false, Description = "Current participation status.", AllowedValues = "Active, Withdrawn, Completed, Pending" },
                    new() { TableName = "Studies",      ColumnName = "StudyId",        DataType = "int",           IsPrimaryKey = true,  IsNullable = false, Description = "Auto-incremented unique identifier for each study." },
                    new() { TableName = "Studies",      ColumnName = "StudyCode",      DataType = "nvarchar(20)",  IsPrimaryKey = false, IsNullable = false, Description = "Short internal code.", BusinessRule = "Unique. Format: MCH-[DEPT]-[NNN]" },
                    new() { TableName = "Studies",      ColumnName = "IRBNumber",      DataType = "nvarchar(50)",  IsPrimaryKey = false, IsNullable = true,  Description = "IRB approval number." },
                    new() { TableName = "Studies",      ColumnName = "Status",         DataType = "nvarchar(30)",  IsPrimaryKey = false, IsNullable = false, Description = "Current status of the study.", AllowedValues = "Active, Recruiting, Closed, Suspended" },
                    new() { TableName = "Visits",       ColumnName = "VisitId",        DataType = "int",           IsPrimaryKey = true,  IsNullable = false, Description = "Unique identifier for each visit." },
                    new() { TableName = "Visits",       ColumnName = "VisitType",      DataType = "nvarchar(100)", IsPrimaryKey = false, IsNullable = false, Description = "Type/purpose of the visit.", AllowedValues = "Screening, Baseline, Follow-up, Annual, Final, Unscheduled" },
                    new() { TableName = "Visits",       ColumnName = "Status",         DataType = "nvarchar(30)",  IsPrimaryKey = false, IsNullable = false, Description = "Visit status.", AllowedValues = "Scheduled, Completed, Missed, Cancelled, Rescheduled" },
                    new() { TableName = "ConsentForms", ColumnName = "ConsentFormId",  DataType = "int",           IsPrimaryKey = true,  IsNullable = false, Description = "Unique identifier for each consent record." },
                    new() { TableName = "ConsentForms", ColumnName = "Version",        DataType = "nvarchar(20)",  IsPrimaryKey = false, IsNullable = false, Description = "Version number of the consent form.", BusinessRule = "Must match an IRB-approved form version." },
                    new() { TableName = "ConsentForms", ColumnName = "Status",         DataType = "nvarchar(30)",  IsPrimaryKey = false, IsNullable = false, Description = "Consent status.", AllowedValues = "Active, Expired, Revoked, Superseded" },
                });
                db.SaveChanges();
            }
        }

        // ── Creates / replaces the DB view and stored procedures on each startup ──
        private static void CreateSqlObjects(ApplicationDbContext db)
        {
            try
            {
                // ── View: vw_StudyEnrollmentSummary ──────────────────────────────
                db.Database.ExecuteSqlRaw(
                    "IF OBJECT_ID('dbo.vw_StudyEnrollmentSummary','V') IS NOT NULL DROP VIEW dbo.vw_StudyEnrollmentSummary;");

                db.Database.ExecuteSqlRaw(@"
CREATE VIEW dbo.vw_StudyEnrollmentSummary AS
SELECT
    s.StudyId,
    s.StudyCode,
    s.StudyName,
    s.Status                                            AS StudyStatus,
    s.Phase,
    ISNULL(s.MaxParticipants, 0)                        AS MaxParticipants,
    COUNT(DISTINCT se.StudyEnrollmentId)                AS EnrolledCount,
    COUNT(DISTINCT CASE WHEN se.Status = 'Active'     THEN se.StudyEnrollmentId END) AS ActiveEnrolled,
    COUNT(DISTINCT CASE WHEN se.Status = 'Completed'  THEN se.StudyEnrollmentId END) AS CompletedEnrolled,
    COUNT(DISTINCT CASE WHEN se.Status = 'Withdrawn'  THEN se.StudyEnrollmentId END) AS WithdrawnCount,
    COUNT(v.VisitId)                                    AS TotalVisits,
    SUM(CASE WHEN v.Status = 'Completed'  THEN 1 ELSE 0 END) AS CompletedVisits,
    SUM(CASE WHEN v.Status = 'Missed'     THEN 1 ELSE 0 END) AS MissedVisits,
    SUM(CASE WHEN v.Status = 'Scheduled'
             AND v.ScheduledDate >= CAST(GETDATE() AS DATE)
             THEN 1 ELSE 0 END)                         AS UpcomingVisits,
    COUNT(DISTINCT cf.ConsentFormId)                    AS TotalConsents,
    COUNT(DISTINCT CASE WHEN cf.Status = 'Active' THEN cf.ConsentFormId END) AS ActiveConsents,
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
    s.StudyId, s.StudyCode, s.StudyName, s.Status, s.Phase, s.MaxParticipants;");

                // ── SP: usp_GetEnrollmentSummaryByStudy ──────────────────────────
                db.Database.ExecuteSqlRaw(
                    "IF OBJECT_ID('dbo.usp_GetEnrollmentSummaryByStudy','P') IS NOT NULL DROP PROCEDURE dbo.usp_GetEnrollmentSummaryByStudy;");

                db.Database.ExecuteSqlRaw(@"
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
        COUNT(DISTINCT se.StudyEnrollmentId)                            AS TotalEnrolled,
        COUNT(DISTINCT CASE WHEN se.Status = 'Active'    THEN se.StudyEnrollmentId END) AS ActiveParticipants,
        COUNT(DISTINCT CASE WHEN se.Status = 'Completed' THEN se.StudyEnrollmentId END) AS CompletedParticipants,
        COUNT(DISTINCT CASE WHEN se.Status = 'Withdrawn' THEN se.StudyEnrollmentId END) AS WithdrawnParticipants,
        SUM(CASE WHEN v.Status = 'Completed' THEN 1 ELSE 0 END)        AS CompletedVisits,
        SUM(CASE WHEN v.Status = 'Missed'    THEN 1 ELSE 0 END)        AS MissedVisits,
        CAST(
            CASE
                WHEN ISNULL(s.MaxParticipants, 0) = 0 THEN 0.00
                ELSE CAST(COUNT(DISTINCT se.StudyEnrollmentId) AS DECIMAL(10,2))
                     / CAST(s.MaxParticipants AS DECIMAL(10,2)) * 100.0
            END
        AS DECIMAL(5,2))                                                AS EnrollmentPct
    FROM dbo.Studies s
    LEFT JOIN dbo.StudyEnrollments se ON se.StudyId = s.StudyId
    LEFT JOIN dbo.Visits           v  ON v.StudyId  = s.StudyId
    WHERE (@StudyId IS NULL OR s.StudyId = @StudyId)
      AND (@Status  IS NULL OR s.Status  = @Status)
    GROUP BY s.StudyId, s.StudyCode, s.StudyName, s.Status, s.Phase, s.MaxParticipants
    ORDER BY TotalEnrolled DESC, s.StudyCode;
END;");

                // ── SP: usp_GetParticipantVisitHistory ───────────────────────────
                db.Database.ExecuteSqlRaw(
                    "IF OBJECT_ID('dbo.usp_GetParticipantVisitHistory','P') IS NOT NULL DROP PROCEDURE dbo.usp_GetParticipantVisitHistory;");

                db.Database.ExecuteSqlRaw(@"
CREATE PROCEDURE dbo.usp_GetParticipantVisitHistory
    @ParticipantId  INT,
    @StudyId        INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        p.MRN,
        p.FirstName + ' ' + p.LastName                                     AS ParticipantName,
        s.StudyCode,
        s.StudyName,
        v.VisitType,
        v.VisitNumber,
        v.ScheduledDate,
        v.CompletedDate,
        v.Status                                                            AS VisitStatus,
        v.Location,
        ISNULL(i.FirstName + ' ' + i.LastName, 'Unassigned')               AS AssignedStaff,
        v.Notes,
        se.Status                                                           AS EnrollmentStatus,
        se.SubjectId
    FROM dbo.Visits v
    INNER JOIN dbo.Participants     p  ON p.ParticipantId  = v.ParticipantId
    INNER JOIN dbo.Studies          s  ON s.StudyId        = v.StudyId
    LEFT  JOIN dbo.Investigators    i  ON i.InvestigatorId = v.AssignedStaffId
    LEFT  JOIN dbo.StudyEnrollments se ON se.ParticipantId = v.ParticipantId
                                      AND se.StudyId       = v.StudyId
    WHERE v.ParticipantId = @ParticipantId
      AND (@StudyId IS NULL OR v.StudyId = @StudyId)
    ORDER BY s.StudyCode, v.ScheduledDate;
END;");
            }
            catch
            {
                // SQL Server Express may not support all DDL, or DB may not exist yet.
                // The app continues without the view/SPs — raw SQL pages will show an error.
            }
        }
    }
}
