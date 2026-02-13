# Student Management System

A full-stack web application for managing student information, built with Angular frontend and .NET Core backend.

![License](https://img.shields.io/badge/license-MIT-blue.svg)
![Angular](https://img.shields.io/badge/Angular-17+-red.svg)
![.NET](https://img.shields.io/badge/.NET-8.0-blue.svg)

## 🚀 Features

- **Student Management**: Add, view, edit, and delete student records
- **Responsive Design**: Modern UI that works on desktop and mobile devices
- **RESTful API**: Clean .NET Core Web API backend
- **Real-time Updates**: Angular frontend with reactive forms
- **Professional UI**: Clean and intuitive user interface

## 🛠️ Tech Stack

### Frontend
- **Angular 17+** - Modern web framework
- **TypeScript** - Type-safe JavaScript
- **Angular Material** - UI component library
- **RxJS** - Reactive programming

### Backend
- **.NET Core 8** - Cross-platform web API
- **ASP.NET Core Web API** - RESTful API framework
- **Entity Framework Core** - ORM for database operations
- **MongoDB/SQL Server** - Database options

## 📋 Prerequisites

Before running this application, make sure you have the following installed:

- **Node.js** (v18 or higher)
- **npm** (v9 or higher)
- **.NET SDK** (8.0 or higher)
- **Angular CLI** (`npm install -g @angular/cli`)

## 🔧 Installation & Setup

### 1. Clone the repository
```bash
git clone https://github.com/renukapatil-21/Student-management-system.git
cd Student-management-system
```

### 2. Backend Setup
```bash
# Navigate to the simple API directory
cd StudentAPI

# Restore dependencies
dotnet restore

# Run the backend API
dotnet run --urls "http://localhost:5286"
```

The backend API will be available at: `http://localhost:5286`

### 3. Frontend Setup
```bash
# Navigate to the frontend directory
cd StudentManagementSystem/Frontend

# Install dependencies
npm install

# Start the Angular development server
npm start
```

The frontend application will be available at: `http://localhost:4200`

## 🏃‍♂️ Running the Application

1. **Start the Backend**: 
   ```bash
   cd StudentAPI && dotnet run --urls "http://localhost:5286"
   ```

2. **Start the Frontend**: 
   ```bash
   cd StudentManagementSystem/Frontend && npm start
   ```

3. **Open your browser** and navigate to `http://localhost:4200`

## 📁 Project Structure

```
Student-management-system/
├── StudentAPI/                          # Simple .NET Core Web API
│   ├── Controllers/
│   │   └── StudentsController.cs        # API endpoints
│   ├── Program.cs                       # API configuration
│   └── StudentAPI.csproj               # Project file
│
├── StudentManagementSystem/             # Full-stack application
│   ├── Backend/                         # Complex Clean Architecture backend
│   │   ├── src/
│   │   │   ├── Core/
│   │   │   │   ├── Domain/             # Domain entities
│   │   │   │   └── Application/        # Business logic
│   │   │   ├── Infrastructure/         # Data access
│   │   │   └── Presentation/           # Web API
│   │   └── README.md
│   │
│   └── Frontend/                        # Angular application
│       ├── src/
│       │   ├── app/
│       │   │   ├── core/               # Core services
│       │   │   ├── features/           # Feature modules
│       │   │   │   ├── auth/          # Authentication
│       │   │   │   ├── dashboard/     # Dashboard
│       │   │   │   └── students/      # Student management
│       │   │   └── shared/            # Shared components
│       │   └── environments/           # Environment configs
│       ├── package.json
│       └── angular.json
│
└── README.md                            # This file
```

## 🔌 API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/students` | Get all students |
| GET | `/api/students/{id}` | Get student by ID |
| POST | `/api/students` | Create new student |
| PUT | `/api/students/{id}` | Update student |
| DELETE | `/api/students/{id}` | Delete student |

### Sample API Response
```json
{
  "success": true,
  "data": [
    {
      "id": "1",
      "firstName": "John",
      "lastName": "Doe",
      "email": "john@example.com",
      "studentId": "STU001",
      "status": "Active"
    }
  ],
  "message": "Students retrieved successfully"
}
```

## 🎨 Screenshots

### Students List
![Students List](./screenshots/students-list.png)

### Dashboard
![Dashboard](./screenshots/dashboard.png)

*Note: Add screenshots after deployment*

## 🚀 Deployment

### Frontend (Netlify/Vercel)
```bash
cd StudentManagementSystem/Frontend
ng build --prod
# Deploy the dist/ folder
```

### Backend (Azure/AWS)
```bash
cd StudentAPI
dotnet publish -c Release
# Deploy to your cloud provider
```

## 🤝 Contributing

1. Fork the project
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 👨‍💻 Author

**Renuka Patil**
- GitHub: [@renukapatil-21](https://github.com/renukapatil-21)
- LinkedIn: [Your LinkedIn Profile]

## 🙏 Acknowledgments

- Angular team for the amazing framework
- Microsoft for .NET Core
- Community contributors

---

⭐ Don't forget to star this repository if you found it helpful!