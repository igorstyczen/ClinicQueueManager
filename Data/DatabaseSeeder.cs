using ClinicQueueManager.Models;
using ClinicQueueManager.Services;

namespace ClinicQueueManager.Data;

/// <summary>Dane początkowe — tylko gdy tabela jest pusta.</summary>
public static class DatabaseSeeder
{
    public static void Seed(AppDbContext context)
    {
        if (!context.Specializations.Any())
        {
            context.Specializations.AddRange(
                new Specialization { Name = "Internista", Description = "Choroby wewnętrzne" },
                new Specialization { Name = "Kardiolog", Description = "Serce i układ krążenia" },
                new Specialization { Name = "Pulmonolog", Description = "Układ oddechowy" },
                new Specialization { Name = "Pediatra", Description = "Opieka nad dziećmi" },
                new Specialization { Name = "Neurolog", Description = "Układ nerwowy" },
                new Specialization { Name = "Dermatolog", Description = "Choroby skóry" });
        }

        if (!context.Offices.Any())
        {
            context.Offices.AddRange(
                new Office { Number = "101", Floor = 1, Description = "Gabinet internistyczny" },
                new Office { Number = "102", Floor = 1, Description = "Gabinet pediatryczny" },
                new Office { Number = "201", Floor = 2, Description = "Gabinet kardiologiczny" },
                new Office { Number = "202", Floor = 2, Description = "Gabinet pulmonologiczny" },
                new Office { Number = "301", Floor = 3, Description = "Gabinet neurologiczny" },
                new Office { Number = "302", Floor = 3, Description = "Gabinet dermatologiczny" });
        }

        if (!context.Symptoms.Any())
        {
            context.Symptoms.AddRange(
                new Symptom { Name = "Ból gardła", PriorityPoints = 1 },
                new Symptom { Name = "Kaszel", PriorityPoints = 1 },
                new Symptom { Name = "Gorączka", PriorityPoints = 2 },
                new Symptom { Name = "Silny ból głowy", PriorityPoints = 3 },
                new Symptom { Name = "Duszność", PriorityPoints = 5 },
                new Symptom { Name = "Ból w klatce piersiowej", PriorityPoints = 6 },
                new Symptom { Name = "Utrata przytomności", PriorityPoints = 8 });
        }

        if (!context.Patients.Any())
        {
            context.Patients.AddRange(
                new Patient
                {
                    FirstName = "Jan", LastName = "Kowalski", Pesel = "80010112345",
                    DateOfBirth = new DateTime(1980, 1, 1), PhoneNumber = "500600700",
                    Email = "jan.kowalski@example.com", Address = "Warszawa, ul. Przykładowa 1",
                    CreatedAt = DateTime.UtcNow
                },
                new Patient
                {
                    FirstName = "Anna", LastName = "Nowak", Pesel = "52030598765",
                    DateOfBirth = new DateTime(1952, 3, 5), PhoneNumber = "600700800",
                    Email = "anna.nowak@example.com", Address = "Kraków, ul. Zielona 20",
                    CreatedAt = DateTime.UtcNow
                },
                new Patient
                {
                    FirstName = "Piotr", LastName = "Wiśniewski", Pesel = "90051234567",
                    DateOfBirth = new DateTime(1990, 5, 12), PhoneNumber = "501111222",
                    Email = "piotr.wisniewski@example.com", Address = "Gdańsk, ul. Morska 5",
                    CreatedAt = DateTime.UtcNow
                },
                new Patient
                {
                    FirstName = "Maria", LastName = "Lewandowska", Pesel = "85072098765",
                    DateOfBirth = new DateTime(1985, 7, 20), PhoneNumber = "502333444",
                    Email = "maria.lewandowska@example.com", Address = "Poznań, ul. Kwiatowa 8",
                    CreatedAt = DateTime.UtcNow
                },
                new Patient
                {
                    FirstName = "Tomasz", LastName = "Zieliński", Pesel = "65120111223",
                    DateOfBirth = new DateTime(1965, 12, 1), PhoneNumber = "503555666",
                    Email = "tomasz.zielinski@example.com", Address = "Wrocław, ul. Słoneczna 12",
                    CreatedAt = DateTime.UtcNow
                },
                new Patient
                {
                    FirstName = "Ewa", LastName = "Kamińska", Pesel = "10031577889",
                    DateOfBirth = new DateTime(2010, 3, 15), PhoneNumber = "504777888",
                    Email = "ewa.kaminska@example.com", Address = "Łódź, ul. Parkowa 3",
                    CreatedAt = DateTime.UtcNow
                });
        }

        context.SaveChanges();

        if (!context.Doctors.Any())
        {
            var specs = context.Specializations.OrderBy(s => s.Id).ToList();
            var offices = context.Offices.OrderBy(o => o.Id).ToList();

            if (specs.Count > 0 && offices.Count > 0)
            {
                context.Doctors.AddRange(
                    new Doctor { FirstName = "Marek", LastName = "Wiśniewski", PhoneNumber = "700800900", Email = "marek.wisniewski@clinic.local", SpecializationId = specs[0].Id, OfficeId = offices[0].Id },
                    new Doctor { FirstName = "Katarzyna", LastName = "Maj", PhoneNumber = "501502503", Email = "katarzyna.maj@clinic.local", SpecializationId = IdAt(specs, 1), OfficeId = IdAt(offices, 2) },
                    new Doctor { FirstName = "Adam", LastName = "Kowalczyk", PhoneNumber = "502111333", Email = "adam.kowalczyk@clinic.local", SpecializationId = IdAt(specs, 2), OfficeId = IdAt(offices, 3) },
                    new Doctor { FirstName = "Joanna", LastName = "Sikora", PhoneNumber = "503222444", Email = "joanna.sikora@clinic.local", SpecializationId = IdAt(specs, 3), OfficeId = IdAt(offices, 1) },
                    new Doctor { FirstName = "Paweł", LastName = "Nowicki", PhoneNumber = "504333555", Email = "pawel.nowicki@clinic.local", SpecializationId = IdAt(specs, 4), OfficeId = IdAt(offices, 4) },
                    new Doctor { FirstName = "Magdalena", LastName = "Wójcik", PhoneNumber = "505444666", Email = "magdalena.wojcik@clinic.local", SpecializationId = IdAt(specs, 5), OfficeId = IdAt(offices, 5) });
                context.SaveChanges();
            }
        }

        if (!context.Appointments.Any())
        {
            var patients = context.Patients.OrderBy(p => p.Id).ToList();
            var doctors = context.Doctors.OrderBy(d => d.Id).ToList();
            var symptoms = context.Symptoms.OrderBy(s => s.Id).ToList();

            if (patients.Count == 0 || doctors.Count == 0)
                return;

            var calculator = new PriorityCalculatorService();
            var durationService = new VisitDurationService();

            var appointmentDefs = new (int PatientIndex, int DoctorIndex, int DaysOffset, int Hour, AppointmentStatus Status, string Reason, int[] SymptomIndexes)[]
            {
                (0, 0, 1, 9, AppointmentStatus.Zaplanowana, "Kontrola ogólna", new[] { 0, 1 }),
                (1, 1, 0, 10, AppointmentStatus.Zaplanowana, "Ból w klatce piersiowej", new[] { 5, 2 }),
                (2, 2, 2, 11, AppointmentStatus.Zaplanowana, "Duszność i kaszel", new[] { 4, 1 }),
                (4, 0, -1, 14, AppointmentStatus.Zakonczona, "Gorączka", new[] { 2 }),
                (3, 3, 3, 8, AppointmentStatus.Anulowana, "Wizyta odwołana", Array.Empty<int>()),
                (1, 1, 0, 15, AppointmentStatus.Zaplanowana, "Nagły przypadek", new[] { 6 })
            };

            foreach (var def in appointmentDefs)
            {
                var patient = patients[Math.Min(def.PatientIndex, patients.Count - 1)];
                var doctor = doctors[Math.Min(def.DoctorIndex, doctors.Count - 1)];

                var selectedSymptoms = def.SymptomIndexes
                    .Where(i => i >= 0 && i < symptoms.Count)
                    .Select(i => symptoms[i])
                    .ToList();

                var priority = calculator.CalculatePriority(patient, selectedSymptoms);
                var visitType = InferVisitType(def.Reason);
                var appt = new Appointment
                {
                    PatientId = patient.Id,
                    DoctorId = doctor.Id,
                    AppointmentDate = DateTime.Today.AddDays(def.DaysOffset).AddHours(def.Hour),
                    Status = def.Status,
                    Reason = def.Reason,
                    VisitType = visitType,
                    Priority = priority,
                    DurationMinutes = durationService.GetSuggestedDuration(visitType),
                    CreatedAt = DateTime.UtcNow
                };

                context.Appointments.Add(appt);
                context.SaveChanges();

                foreach (var symptom in selectedSymptoms)
                {
                    context.AppointmentSymptoms.Add(new AppointmentSymptom
                    {
                        AppointmentId = appt.Id,
                        SymptomId = symptom.Id
                    });
                }
            }

            context.SaveChanges();
        }
    }

    private static string InferVisitType(string reason)
    {
        var r = reason.ToLowerInvariant();
        if (r.Contains("kontrol"))
            return "Kontrola";
        if (r.Contains("recept"))
            return "Wypisanie recepty";
        if (r.Contains("nagł") || r.Contains("piln"))
            return "Pilna konsultacja";
        if (r.Contains("gorącz") || r.Contains("ból") || r.Contains("duszno"))
            return "Konsultacja ogólna";
        return "Konsultacja ogólna";
    }

    private static int IdAt(List<Specialization> list, int index) =>
        list[Math.Min(index, list.Count - 1)].Id;

    private static int IdAt(List<Office> list, int index) =>
        list[Math.Min(index, list.Count - 1)].Id;
}
