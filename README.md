# FutureInnovaters
Smart automation for scalable operations

### 1.User Classes

| User Class | Description | Access Level |
|------------|-------------|--------------|
| Department Head | Authenticated user | Full platform access |

### 2. Operating Environment

- **Client:** Modern web browser (Chrome, Firefox, Edge, Safari)
- **Network:** Standard internet connection
- **Backend:** C#
- **Hosting:** Local

### 2.1 Design Constraints

- GitHub repository public for assessment duration
- 
- 

---

## 3. Functional Requirements

### 3.1 Account Management

**FR-001 — User Registration**

The system shall allow a Department Head to create an account using an email address and password.The user selects which department they belong to.

*Acceptance criteria:*
- Email must be validated 
- Password must be a minimum of 8 characters
- User is redirected to the their respective  Department feed after successful registration

**FR-002 — User Authentication State**

The system shall persist authentication state across browser sessions.

*Acceptance criteria:*
- Logged-in user remains authenticated after reopening browser
- Protected routes redirect unauthenticated users to login page
- Sign-out clears authentication state immediately

### 3.2 Session Management

**FR-003 — Create Allocation**

A registered Department Head shall be able to create a new allocation entry with the following fields:

| Field | Type | Required |
|-------|------|----------|
| Session name | String (max 100 chars) | Yes |
| Description | String (max 500 chars) | Yes |
| Total Attendes| String (max 2 chars) | Yes |
| Seats left| String (max 2 chars) | Yes |



*Acceptance criteria:*
- Required fields enforced with validation
- CreatedAt and updatedAt timestamps set server-side
- Session appears in live feed immediately

**FR-004 — Edit and Delete Session**

A Department Head shall be able to edit all fields of their own project and permanently delete it.

*Acceptance criteria:*
- Only session owner (matching ownerId) can edit or delete
- Deletion removes project document and all associated subcollections
- Edit triggers updatedAt timestamp refresh

**FR-005 — Project Stage Tracking**

Sessions shall be assigned one of the following stages:
- `upcoming` — Initial invitation phase
- `in-progress` — Session is underway
- `completed` — Session has ended

*Acceptance criteria:*
- Stage displayed as color-coded badge on session card
- Stage change updates updatedAt



### 3.4 Live Feed

**FR-009 — Real-Time Session Feed**

-All Department heads shall see a live feed of Sessions ordered by most recently updated.
-All Department heads shall see how many seats are occupied and how many still available.

*Acceptance criteria:*
- New and updated sessions appear within 2 seconds
- Feed displays: Session name, description, stage,timestamp

### 3.5 Functional Constraints
-No same user can be assigned session twice
-No exceeding Department limit(capacity)
-


---

## 4. Non-Functional Requirements

**NFR-001 — Performance**
Initial page load shall complete within X seconds. Live feed updates within Y seconds.

**NFR-002 — Security**


**NFR-003 — Usability**


**NFR-004 — Reliability**


**NFR-005 — Maintainability**
React components follow consistent naming conventions (camelCase for functions, PascalCase for components).

**NFR-006 — Scalability**
queries use indexed fields for consistent performance.






