export function dashboardPage(sessions = {}) {
  return `
<section class="route-shell">
  <nav class="route-nav">
    <a href="#/">Home</a>
    <a href="#/dashboard">Dashboard</a>
    <a href="#/login">Login</a>
    <a href="#/registration">Register</a>
  </nav>

  <section class="route-content">
    <h1>Smart Seat Allocation Dashboard</h1>

    <!-- SESSION CREATION -->
    <div class="create-session">
      <h2>Create Session</h2>

      <input id="sessionName" placeholder="Session name" />
      <input id="sessionTime" placeholder="Time slot (e.g. 09:00 - 10:30)" />

      <select id="sessionStage">
        <option value="upcoming">Upcoming</option>
        <option value="in-progress">In Progress</option>
        <option value="completed">Completed</option>
      </select>

      <textarea id="sessionDesc" placeholder="Short description (optional)"></textarea>

      <button onclick="createSession()">Create Session</button>
    </div>

    <hr/>

    <!-- SESSIONS DISPLAY -->
    <div class="sessions">

      ${Object.entries(sessions).map(([key, session]) => {
        const totalAssigned = session.assigned.length;
        const remaining = session.capacity - totalAssigned;

        return `
        <div class="session-card">
          <h2>${session.name || key.toUpperCase()}</h2>

          ${session.description ? `<p><strong>Description:</strong> ${session.description}</p>` : ''}

          <p><strong>Time:</strong> ${session.time}</p>

          <p>
            <strong>Status:</strong> 
            <span class="status-${session.stage}">
              ${session.stage.replace("-", " ").toUpperCase()}
            </span>
          </p>

          <p><strong>Members Inside:</strong> ${totalAssigned}</p>
          <p><strong>Seats Left:</strong> ${remaining} / ${session.capacity}</p>

          <div class="departments">
            <p><strong>Division A:</strong> ${session.departments.A} / 8</p>
            <p><strong>Division B:</strong> ${session.departments.B} / 8</p>
            <p><strong>Division C:</strong> ${session.departments.C} / 6</p>
          </div>

          <p class="${remaining === 0 ? 'status-full' : 'status-available'}">
            ${remaining === 0 ? 'Session Full' : 'Seats Available'}
          </p>
        </div>
        `;
      }).join('')}

    </div>

    <div id="feedback" class="feedback">
      <p>No actions yet.</p>
    </div>

  </section>
</section>
`
}