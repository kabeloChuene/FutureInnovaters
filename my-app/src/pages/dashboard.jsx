export function dashboardPage(sessions = {}) {
  return `
<section class="route-shell">

  <section class="route-content">
    <h1>Smart Seat Allocation</h1>

    <div class="session-grid">

      ${["morning", "midday", "afternoon"].map((key) => {
        const session = sessions[key] || {
          name: "",
          assigned: [],
          capacity: 8
        };

        const remaining = session.capacity - session.assigned.length;

        const titles = {
          morning: "Morning Session 09:00 - 10:30",
          midday: "Midday Session 11:00 - 12:30",
          afternoon: "Afternoon Session 13:00 - 14:30"
        };

        return `
        <div class="session-card">

          <h2>${titles[key]}</h2>

          <p><strong>Members:</strong> ${session.assigned.length} / ${session.capacity}</p>
          <p><strong>Seats Left:</strong> ${remaining}</p>

          <div class="worker-list">
            ${
              session.assigned.length
                ? session.assigned.map(w => `<span class="worker">${w}</span>`).join("")
                : "<p>No workers assigned</p>"
            }
          </div>

          <button onclick="allocateWorkers('${key}')">
            Allocate Workers
          </button>

        </div>
        `;
      }).join("")}

    </div>

    <div id="feedback" class="feedback">
      <p>No actions yet.</p>
    </div>

  </section>
</section>
`;
}