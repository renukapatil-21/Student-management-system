using MongoDB.Driver;
using StudentAPI.Models;
using StudentAPI.Services;
using Microsoft.Extensions.Options;

namespace StudentAPI.Services;

public class DatabaseInitializer
{
    private readonly IMongoDatabase _database;
    private readonly StudentService _studentService;
    private readonly FeeService _feeService;
    private readonly InquiryService _inquiryService;
    private readonly ILogger<DatabaseInitializer> _logger;

    public DatabaseInitializer(
        IOptions<MongoDbSettings> mongoDbSettings,
        StudentService studentService,
        FeeService feeService,
        InquiryService inquiryService,
        ILogger<DatabaseInitializer> logger)
    {
        var mongoClient = new MongoClient(mongoDbSettings.Value.ConnectionString);
        _database = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);
        _studentService = studentService;
        _feeService = feeService;
        _inquiryService = inquiryService;
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        try
        {
            _logger.LogInformation("Initializing database...");

            // Create indexes for better performance
            await CreateIndexesAsync();

            // Check if data already exists
            var studentCount = await _studentService.GetTotalCountAsync();
            if (studentCount > 0)
            {
                _logger.LogInformation("Database already contains data. Skipping seeding.");
                return;
            }

            // Seed initial data
            await SeedStudentsAsync();
            await SeedFeesAsync();
            await SeedInquiriesAsync();

            _logger.LogInformation("Database initialization completed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing database");
            throw;
        }
    }

    private async Task CreateIndexesAsync()
    {
        // Create indexes for Students collection
        var studentsCollection = _database.GetCollection<Student>("Students");
        var studentIndexKeys = Builders<Student>.IndexKeys
            .Ascending(s => s.Email)
            .Ascending(s => s.FirstName)
            .Ascending(s => s.LastName);
        await studentsCollection.Indexes.CreateOneAsync(new CreateIndexModel<Student>(studentIndexKeys));

        // Create indexes for Fees collection
        var feesCollection = _database.GetCollection<Fee>("Fees");
        var feeIndexKeys = Builders<Fee>.IndexKeys
            .Ascending(f => f.StudentId)
            .Ascending(f => f.Status)
            .Ascending(f => f.DueDate);
        await feesCollection.Indexes.CreateOneAsync(new CreateIndexModel<Fee>(feeIndexKeys));

        // Create indexes for Inquiries collection
        var inquiriesCollection = _database.GetCollection<Inquiry>("Inquiries");
        var inquiryIndexKeys = Builders<Inquiry>.IndexKeys
            .Ascending(i => i.Status)
            .Ascending(i => i.InquiryType)
            .Descending(i => i.CreatedAt);
        await inquiriesCollection.Indexes.CreateOneAsync(new CreateIndexModel<Inquiry>(inquiryIndexKeys));

        _logger.LogInformation("Database indexes created successfully.");
    }

    private async Task SeedStudentsAsync()
    {
        var students = new List<Student>
        {
            new Student
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@email.com",
                Phone = "+1-555-0123",
                Course = "Computer Science",
                DateOfBirth = new DateTime(2000, 5, 15),
                Address = new Address
                {
                    Street = "123 Main St",
                    City = "New York",
                    State = "NY",
                    ZipCode = "10001",
                    Country = "USA"
                },
                GuardianInfo = new GuardianInfo
                {
                    Name = "Robert Doe",
                    Relationship = "Father",
                    Phone = "+1-555-0124",
                    Email = "robert.doe@email.com"
                }
            },
            new Student
            {
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane.smith@email.com",
                Phone = "+1-555-0125",
                Course = "Business Administration",
                DateOfBirth = new DateTime(1999, 8, 22),
                Address = new Address
                {
                    Street = "456 Oak Ave",
                    City = "Los Angeles",
                    State = "CA",
                    ZipCode = "90001",
                    Country = "USA"
                },
                GuardianInfo = new GuardianInfo
                {
                    Name = "Mary Smith",
                    Relationship = "Mother",
                    Phone = "+1-555-0126",
                    Email = "mary.smith@email.com"
                }
            },
            new Student
            {
                FirstName = "Bob",
                LastName = "Johnson",
                Email = "bob.johnson@email.com",
                Phone = "+1-555-0127",
                Course = "Engineering",
                DateOfBirth = new DateTime(2001, 2, 10),
                Address = new Address
                {
                    Street = "789 Pine St",
                    City = "Chicago",
                    State = "IL",
                    ZipCode = "60601",
                    Country = "USA"
                },
                GuardianInfo = new GuardianInfo
                {
                    Name = "David Johnson",
                    Relationship = "Father",
                    Phone = "+1-555-0128",
                    Email = "david.johnson@email.com"
                }
            },
            new Student
            {
                FirstName = "Alice",
                LastName = "Brown",
                Email = "alice.brown@email.com",
                Phone = "+1-555-0129",
                Course = "Psychology",
                DateOfBirth = new DateTime(2000, 11, 5),
                Address = new Address
                {
                    Street = "321 Elm St",
                    City = "Miami",
                    State = "FL",
                    ZipCode = "33101",
                    Country = "USA"
                },
                GuardianInfo = new GuardianInfo
                {
                    Name = "Linda Brown",
                    Relationship = "Mother",
                    Phone = "+1-555-0130",
                    Email = "linda.brown@email.com"
                }
            },
            new Student
            {
                FirstName = "Mike",
                LastName = "Wilson",
                Email = "mike.wilson@email.com",
                Phone = "+1-555-0131",
                Course = "Mathematics",
                DateOfBirth = new DateTime(1998, 12, 18),
                Address = new Address
                {
                    Street = "654 Cedar Ave",
                    City = "Seattle",
                    State = "WA",
                    ZipCode = "98101",
                    Country = "USA"
                },
                GuardianInfo = new GuardianInfo
                {
                    Name = "Tom Wilson",
                    Relationship = "Father",
                    Phone = "+1-555-0132",
                    Email = "tom.wilson@email.com"
                }
            }
        };

        foreach (var student in students)
        {
            await _studentService.CreateAsync(student);
        }

        _logger.LogInformation($"Seeded {students.Count} students successfully.");
    }

    private async Task SeedFeesAsync()
    {
        // Get all students to create fees for them
        var students = await _studentService.GetAsync();
        var fees = new List<Fee>();

        foreach (var student in students)
        {
            // Create tuition fee
            fees.Add(new Fee
            {
                StudentId = student.Id!,
                StudentName = $"{student.FirstName} {student.LastName}",
                FeeType = "Tuition",
                Amount = 5000,
                DueDate = DateTime.UtcNow.AddDays(30),
                Status = FeeStatus.Pending,
                AcademicYear = "2026",
                Semester = "Spring"
            });

            // Create library fee
            fees.Add(new Fee
            {
                StudentId = student.Id!,
                StudentName = $"{student.FirstName} {student.LastName}",
                FeeType = "Library",
                Amount = 200,
                DueDate = DateTime.UtcNow.AddDays(15),
                Status = FeeStatus.Pending,
                AcademicYear = "2026",
                Semester = "Spring"
            });

            // Create one paid fee for demonstration
            if (fees.Count == 2) // For first student
            {
                fees.Add(new Fee
                {
                    StudentId = student.Id!,
                    StudentName = $"{student.FirstName} {student.LastName}",
                    FeeType = "Lab",
                    Amount = 300,
                    PaidAmount = 300,
                    DueDate = DateTime.UtcNow.AddDays(-10),
                    PaymentDate = DateTime.UtcNow.AddDays(-5),
                    PaymentMethod = "Credit Card",
                    TransactionId = "TXN-001",
                    Status = FeeStatus.Paid,
                    AcademicYear = "2026",
                    Semester = "Spring"
                });
            }
        }

        foreach (var fee in fees)
        {
            await _feeService.CreateAsync(fee);
        }

        _logger.LogInformation($"Seeded {fees.Count} fees successfully.");
    }

    private async Task SeedInquiriesAsync()
    {
        var inquiries = new List<Inquiry>
        {
            new Inquiry
            {
                Name = "Sarah Johnson",
                Email = "sarah.johnson@email.com",
                Phone = "+1-555-0200",
                Subject = "Admission Requirements",
                Message = "I would like to know about the admission requirements for the Computer Science program. What are the prerequisites and application deadlines?",
                InquiryType = InquiryType.Admission,
                Status = InquiryStatus.New,
                Priority = InquiryPriority.High
            },
            new Inquiry
            {
                Name = "Mark Davis",
                Email = "mark.davis@email.com",
                Phone = "+1-555-0201",
                Subject = "Fee Payment Options",
                Message = "What are the available payment options for tuition fees? Do you offer installment plans?",
                InquiryType = InquiryType.Fees,
                Status = InquiryStatus.InProgress,
                Priority = InquiryPriority.Medium,
                AssignedTo = "Finance Team"
            },
            new Inquiry
            {
                Name = "Emily Clark",
                Email = "emily.clark@email.com",
                Phone = "+1-555-0202",
                Subject = "Course Information",
                Message = "Can you provide detailed information about the Psychology curriculum and course structure?",
                InquiryType = InquiryType.Academic,
                Status = InquiryStatus.Resolved,
                Priority = InquiryPriority.Medium,
                AssignedTo = "Academic Advisor",
                Response = "The Psychology program includes courses in cognitive psychology, behavioral analysis, research methods, and clinical psychology. Please visit our website for the complete curriculum details.",
                ResponseDate = DateTime.UtcNow.AddDays(-2)
            },
            new Inquiry
            {
                Name = "Alex Thompson",
                Email = "alex.thompson@email.com",
                Phone = "+1-555-0203",
                Subject = "Technical Issue",
                Message = "I'm having trouble accessing the student portal. Can you help me reset my password?",
                InquiryType = InquiryType.Technical,
                Status = InquiryStatus.New,
                Priority = InquiryPriority.High
            },
            new Inquiry
            {
                Name = "Lisa Martinez",
                Email = "lisa.martinez@email.com",
                Phone = "+1-555-0204",
                Subject = "General Information",
                Message = "What are your campus visiting hours? I'd like to schedule a tour.",
                InquiryType = InquiryType.General,
                Status = InquiryStatus.Closed,
                Priority = InquiryPriority.Low,
                AssignedTo = "Reception",
                Response = "Campus tours are available Monday-Friday 9 AM to 4 PM. Please call our reception to schedule your visit.",
                ResponseDate = DateTime.UtcNow.AddDays(-1)
            }
        };

        foreach (var inquiry in inquiries)
        {
            await _inquiryService.CreateAsync(inquiry);
        }

        _logger.LogInformation($"Seeded {inquiries.Count} inquiries successfully.");
    }
}