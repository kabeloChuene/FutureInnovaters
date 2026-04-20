# Smart Seat Allocation Platform

A comprehensive C# backend system for automatically allocating participants to training sessions while respecting capacity limits and department constraints.

## Overview

The Smart Seat Allocation Platform replaces manual seat assignment with an automated, rule-driven system that ensures:

- No Overbooking: Sessions cannot exceed their 20-person capacity
- No Duplicate Assignments: Each participant is assigned to exactly one session
- Department Compliance: All department seat limits are respected
- Validation Feedback: Clear messages explain why an allocation is rejected

## Architecture

### Project Structure

```
SmartSeatAllocation/
├── SmartSeatAllocation.Core/
│   ├── Models/
│   │   ├── Department.cs          # Department definitions & limits
│   │   ├── Session.cs             # Training session details
│   │   ├── Participant.cs         # Participant information
│   │   ├── AllocationResult.cs    # API response wrapper
│   │   └── AllocationRequest.cs   # API request model
│   └── Services/
│       ├── AllocationService.cs   # Core allocation logic & validation
│       ├── SessionService.cs      # Session statistics & queries
│       └── ParticipantService.cs  # Participant management
├── SmartSeatAllocation.API/
│   ├── Controllers/
│   │   ├── AllocationController.cs    # POST/GET allocation endpoints
│   │   ├── SessionController.cs       # Session info & statistics endpoints
│   │   └── ParticipantController.cs   # Participant management endpoints
│   ├── Program.cs                     # Startup & dependency injection
│   └── appsettings.json              # Configuration
└── SmartSeatAllocation.Tests/
    ├── AllocationServiceTests.cs      # Allocation logic tests
    ├── SessionServiceTests.cs         # Session functionality tests
    └── ParticipantServiceTests.cs     # Participant management tests
```

## Training Programme Details

### Sessions

| Session | Time Slot | Capacity | Notes |
|---------|-----------|----------|-------|
| Morning | 09:00 - 10:30 | 20 | 1.5 hour slot |
| Midday | 11:00 - 12:30 | 20 | 1.5 hour slot |
| Afternoon | 13:00 - 14:30 | 20 | 1.5 hour slot |

**Total Capacity**: 60 seats (3 sessions × 20 per session)

### Departments

| Department | Total Participants | Total Seats | Max per Session |
|------------|-------------------|-------------|-----------------|
| Division A | 24 | 24 | 8 |
| Division B | 18 | 18 | 8 |
| Division C | 18 | 18 | 6 |
| **Total** | **60** | **60** | **20** |

## Business Rules

### Allocation Validation

The system enforces four key validation rules, checked in order:

1. No Duplicate Assignment: A participant cannot be assigned more than once
2. Session Capacity: A session cannot exceed 20 participants
3. Department Session Limit: No department can exceed its per-session limit:
   - Division A: max 8 per session
   - Division B: max 8 per session
   - Division C: max 6 per session
4. Department Total Allocation: Departments cannot exceed their total seat allocation

### Allocation Flow

```
Allocation Request
    ↓
Validate Participant Exists → Return Error if Not Found
    ↓
Validate Session Exists → Return Error if Not Found
    ↓
Check: Already Assigned? → Return Error
    ↓
Check: Session Full? → Return Error
    ↓
Check: Department Limit Exceeded? → Return Error
    ↓
Check: Department Total Exceeded? → Return Error
    ↓
Allocation Success
```

## Getting Started

### Prerequisites

- .NET 8.0 SDK or later
- Visual Studio, VS Code, or any C# IDE

### Building the Solution

```bash
# Navigate to the solution directory
cd SmartSeatAllocation

# Restore dependencies
dotnet restore

# Build the solution
dotnet build

# Run tests
dotnet test

# Run the API server
cd SmartSeatAllocation.API
dotnet run
```

The API will start on `https://localhost:5001` (HTTPS) and `http://localhost:5000` (HTTP).

### Interactive API Documentation

Once the API is running, visit:
- **Swagger UI**: http://localhost:5000/swagger

This provides an interactive interface to test all API endpoints.

## API Endpoints

### Allocation Endpoints

#### 1. Allocate Participant
```http
POST /api/allocation/allocate
Content-Type: application/json

{
  "participantId": 1,
  "sessionId": 1
}
```

**Success Response** (200 OK):
```json
{
  "isSuccess": true,
  "message": "Successfully allocated John Doe to Morning session",
  "participant": {
    "id": 1,
    "name": "John Doe",
    "email": "john@example.com",
    "department": "DivisionA",
    "assignedSessionId": 1,
    "assignedSessionType": "Morning"
  },
  "session": {
    "id": 1,
    "name": "Morning",
    "timeSlot": "09:00 - 10:30",
    "capacity": 20,
    "currentOccupancy": 1
  }
}
```

**Failure Response** (400 Bad Request):
```json
{
  "isSuccess": false,
  "message": "John Doe is already assigned to Morning session"
}
```

#### 2. Deallocate Participant
```http
POST /api/allocation/deallocate/{participantId}
```

#### 3. Check Allocation Status
```http
GET /api/allocation/participant/{participantId}/status
```

#### 4. Get Session Allocations
```http
GET /api/allocation/session/{sessionId}/allocations
```

#### 5. Validate Allocation (No Changes)
```http
GET /api/allocation/validate?participantId=1&sessionId=1
```

Response:
```json
{
  "isValid": true,
  "message": "Can allocate participant to session",
  "participantId": 1,
  "sessionId": 1
}
```

### Session Endpoints

#### 1. Get Session Details
```http
GET /api/session/{sessionId}
```

#### 2. Get All Sessions
```http
GET /api/session
```

#### 3. Get Session Statistics
```http
GET /api/session/{sessionId}/statistics
```

Response:
```json
{
  "session": {
    "id": 1,
    "name": "Morning",
    "timeSlot": "09:00 - 10:30",
    "capacity": 20,
    "currentOccupancy": 5
  },
  "totalAllocated": 5,
  "availableSeats": 15,
  "occupancyPercentage": 25.0,
  "departmentBreakdown": {
    "DivisionA": 2,
    "DivisionB": 2,
    "DivisionC": 1
  }
}
```

#### 4. Get All Session Statistics
```http
GET /api/session/statistics/all
```

### Participant Endpoints

#### 1. Get Participant Details
```http
GET /api/participant/{participantId}
```

#### 2. Get All Participants
```http
GET /api/participant
```

#### 3. Get Unallocated Participants
```http
GET /api/participant/unallocated
```

#### 4. Get Participants by Department
```http
GET /api/participant/department/{department}
```

Departments: `DivisionA`, `DivisionB`, `DivisionC`

#### 5. Add New Participant
```http
POST /api/participant
Content-Type: application/json

{
  "name": "Alice Brown",
  "email": "alice@example.com",
  "department": "DivisionA"
}
```

## Testing

The solution includes comprehensive unit tests using xUnit:

```bash
# Run all tests
dotnet test

# Run tests with verbose output
dotnet test --verbosity detailed

# Run specific test class
dotnet test --filter "AllocationServiceTests"
```

### Test Coverage

- **AllocationServiceTests**: 12 tests covering all business rules and edge cases
- **SessionServiceTests**: 6 tests for session management and statistics
- **ParticipantServiceTests**: 8 tests for participant operations

Key scenarios tested:
- Valid allocation
- Duplicate assignment prevention
- Session capacity enforcement
- Department limit enforcement
- Deallocation
- Validation messages
- Statistics calculations
- Participant filtering

## Data Persistence

Currently, the system uses **in-memory storage** (singleton pattern). This is suitable for:
- Development and testing
- Co-Lab demonstrations
- Proof of concept

For production use, you can integrate:
- **Entity Framework Core** with SQL Server/PostgreSQL
- **Azure SQL Database**
- **Cosmos DB**
- **Any REST-based data store**

## Example Workflow

### Scenario: Allocate 5 participants

```csharp
// 1. Add participants
POST /api/participant
{
  "name": "John Doe",
  "email": "john@example.com",
  "department": "DivisionA"
}

// 2. Allocate to Morning session
POST /api/allocation/allocate
{
  "participantId": 1,
  "sessionId": 1
}

// 3. Check session statistics
GET /api/session/1/statistics
→ Returns: 1 allocated, 19 available, 5% occupancy

// 4. Get all allocations for the session
GET /api/allocation/session/1/allocations
→ Returns: List of participants in Morning session
```

## Key Features Implemented

### 1. Allocation Management
- Allocate participants with automatic validation
- Deallocate participants (free up seats)
- Check allocation status

### 2. Validation & Feedback
- Real-time allocation validation
- Detailed error messages explaining why allocation failed
- No changes if validation fails (atomic operations)

### 3. Session Management
- View session capacity and occupancy
- Monitor available seats
- Track department distribution within sessions

### 4. Participant Management
- Add participants with department assignment
- View all participants
- Filter by allocation status or department
- Get participant details

### 5. Statistics & Reporting
- Session occupancy percentage
- Department breakdown per session
- Unallocated participant counts
- Available seat tracking

## Design Principles

1. Separation of Concerns: Domain logic, services, and API layers are separated
2. Testability: All business logic is unit tested and mockable
3. Clear Errors: Validation messages guide users on why allocations fail
4. Atomicity: Allocations succeed completely or fail completely
5. Scalability: Services designed for easy expansion (e.g., database integration)

## Documentation

- API Endpoints: Full Swagger/OpenAPI documentation at `/swagger`
- Code Comments: Inline documentation for complex logic
- Test Examples: Tests serve as usage examples
- README: This comprehensive guide

## Future Enhancements

1. Bulk Allocations: Allocate multiple participants simultaneously
2. Automatic Allocation: Smart algorithm to auto-fill sessions optimally
3. Waitlist Management: Queue for oversubscribed sessions
4. Data Persistence: Database integration with Entity Framework
5. Advanced Reporting: Charts, analytics, export functionality
6. Audit Trail: Track all allocation changes with timestamps
7. Notifications: Email/SMS notifications for allocations
8. Web Frontend: React/Angular UI for end users

## License

This project is provided as-is for educational and operational purposes.

## Support

For questions or issues, please refer to the inline code documentation or test cases for usage examples.

---

**Last Updated**: April 2026
**Version**: 1.0.0
