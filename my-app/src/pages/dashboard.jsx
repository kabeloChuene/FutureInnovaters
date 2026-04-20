import { db } from '../firebase/config.js'
import { doc, getDoc, setDoc, collection, getDocs } from 'firebase/firestore'

export function dashboardPage() {
  return `
<section class="route-shell">
  <section class="route-content">
    <div class="dashboard-header">
      <h1>Smart Seat Allocation</h1>
      <div class="user-info">
        <span id="user-department" class="department-badge">Loading...</span>
        <button onclick="handleLogout()" class="logout-btn">Sign Out</button>
      </div>
    </div>
    <div id="dashboard-loading" style="text-align: center; padding: 40px;">
      <p>Loading sessions...</p>
    </div>
    <div class="session-grid" id="session-grid" style="display: none;">
      <!-- Sessions will be loaded here -->
    </div>
    <div id="feedback" class="feedback">
      <p>No actions yet.</p>
    </div>
  </section>
</section>
`;
}

// Load dashboard data
export async function loadDashboardData() {
  const currentUserStr = sessionStorage.getItem('seatSync_currentUser');
  if (!currentUserStr) {
    window.location.hash = '#/login';
    return;
  }

  const currentUser = JSON.parse(currentUserStr);
  const loadingEl = document.getElementById('dashboard-loading');
  const gridEl = document.getElementById('session-grid');

  try {
    // Fetch all sessions from Firestore based on user's department
    const sessionKeys = ['morning', 'midday', 'afternoon'];
    const sessionsData = {};
    const department = currentUser.department;

    for (const key of sessionKeys) {
      const sessionDoc = await getDoc(doc(db, 'sessions', `${department}_${key}`));
      if (sessionDoc.exists()) {
        sessionsData[key] = sessionDoc.data();
      } else {
        // Create missing department session doc on the fly
        const capacity = department === 'A' ? 8 : 6;
        const sessionRef = doc(db, 'sessions', `${department}_${key}`);
        const sessionData = {
          capacity: capacity,
          allocated: [],
          department: department,
          createdAt: new Date().toISOString()
        };
        await setDoc(sessionRef, sessionData);
        sessionsData[key] = sessionData;
      }
    }

    // Generate HTML
    const html = sessionKeys.map((key) => {
      const session = sessionsData[key] || { allocated: [], capacity: 20 };
      const remaining = session.capacity - (session.allocated || []).length;
      const isAllocated = (session.allocated || []).includes(currentUser.email);

      const titles = {
        morning: "Morning Session 09:00 - 10:30",
        midday: "Midday Session 11:00 - 12:30",
        afternoon: "Afternoon Session 13:00 - 14:30"
      };

      return `
      <div class="session-card">
        <h2>${titles[key]}</h2>
        <div class="session-capacity">
          <span class="capacity-badge">Max: ${session.capacity} seats</span>
        </div>
        <p><strong>Members:</strong> ${(session.allocated || []).length} / ${session.capacity}</p>
        <p><strong>Seats Left:</strong> ${remaining}</p>
        <div class="worker-list">
          ${
            (session.allocated && session.allocated.length > 0)
              ? session.allocated.map(w => `<span class="worker">${w}</span>`).join("")
              : "<p>No workers assigned</p>"
          }
        </div>
        <button onclick="allocateWorkers('${key}')" ${isAllocated ? 'disabled' : ''} style="${isAllocated ? 'opacity: 0.5; cursor: not-allowed;' : ''}">
          ${isAllocated ? 'Already Allocated' : 'Allocate to This Session'}
        </button>
      </div>
      `;
    }).join("");

    // Update DOM
    if (loadingEl) loadingEl.style.display = 'none';
    if (gridEl) {
      gridEl.innerHTML = html;
      gridEl.style.display = 'grid';
    }

    // Update department badge
    const deptEl = document.getElementById('user-department');
    if (deptEl) {
      const deptNames = {
        'A': 'Division A (8 seats/session)',
        'B': 'Division B (6 seats/session)',
        'C': 'Division C (6 seats/session)'
      };
      deptEl.textContent = deptNames[department] || department;
      deptEl.className = `department-badge dept-${department.toLowerCase()}`;
    }
  } catch (error) {
    console.error('Error loading dashboard:', error);
    if (loadingEl) {
      loadingEl.innerHTML = '<p style="color: red;">Error loading sessions. Please refresh the page.</p>';
    }
  }
}

// Handle logout
window.handleLogout = async function() {
  const { signOut } = await import('firebase/auth');
  const { auth } = await import('../firebase/config.js');

  try {
    await signOut(auth);
    sessionStorage.removeItem('seatSync_currentUser');
    window.location.hash = '#/login';
  } catch (error) {
    console.error('Error signing out:', error);
  }
};