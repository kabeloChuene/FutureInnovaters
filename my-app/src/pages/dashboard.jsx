import { db } from '../firebase/config.js'
import { doc, getDoc, setDoc, updateDoc, collection, query, where, getDocs } from 'firebase/firestore'

export function dashboardPage() {
  return `
<section class="route-shell">
  <section class="route-content">
    <div class="dashboard-header">
      <div class="header-content">
        <h1>Smart Seat Allocation Platform</h1>
        <p class="dashboard-subtitle">Training Programme Management System</p>
      </div>
      <div class="user-info">
        <span id="user-department" class="department-badge">Loading...</span>        <span id="division-remaining" class="division-remaining">Loading remaining members...</span>        <div class="admin-note">
          <small>� Manage your division's members and allocate them to sessions</small>
        </div>
        <button onclick="fillSessionDemo()" class="demo-btn">Demo: Fill Session</button>
        <button onclick="clearSessionDemo()" class="clear-btn">Clear Demo</button>
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
    // Fetch all sessions from Firestore
    const sessionKeys = ['morning', 'midday', 'afternoon'];
    const sessionsData = {};
    const department = currentUser.department;

    for (const key of sessionKeys) {
      const sessionRef = doc(db, 'sessions', key);
      const sessionDoc = await getDoc(sessionRef);
      if (sessionDoc.exists()) {
        sessionsData[key] = sessionDoc.data();
      } else {
        // Create missing shared session doc on the fly
        const sessionData = {
          capacity: 20,
          allocated: [],
          departmentTotals: { A: 0, B: 0, C: 0 },
          departmentCapacities: { A: 8, B: 6, C: 6 },
          createdAt: new Date().toISOString()
        };
        await setDoc(sessionRef, sessionData);
        sessionsData[key] = sessionData;
      }
    }

    // Generate HTML
    const html = await Promise.all(sessionKeys.map(async (key) => {
      const session = sessionsData[key] || { allocated: [], capacity: 20, departmentTotals: { A: 0, B: 0, C: 0 }, departmentCapacities: { A: 8, B: 6, C: 6 } };
      const remaining = session.capacity - (session.allocated || []).length;
      const isAllocated = (session.allocated || []).includes(currentUser.email);
      const isFull = (session.allocated || []).length >= session.capacity;
      const deptTotal = (session.departmentTotals || {})[department] || 0;
      const deptCap = (session.departmentCapacities || {})[department] || (department === 'A' ? 8 : 6);
      const deptRemaining = Math.max(0, deptCap - deptTotal);
      const canAddMembers = !isFull && deptRemaining > 0;

      const titles = {
        morning: "Morning Session 09:00 - 10:30",
        midday: "Midday Session 11:00 - 12:30",
        afternoon: "Afternoon Session 13:00 - 14:30"
      };

      // Filter participants by current user's division
      let filteredParticipants = [];
      if (session.allocated && session.allocated.length > 0) {
        // Get user data for each allocated participant to filter by division
        const participantPromises = session.allocated.map(async (item) => {
          if (typeof item === 'string' && item.startsWith('placeholder:')) {
            const [, deptId, memberNum] = item.split(':');
            return {
              email: item,
              department: deptId,
              displayName: `Member ${deptId}${memberNum}`
            };
          }

          try {
            const usersRef = collection(db, 'users');
            const q = query(usersRef, where('email', '==', item));
            const querySnapshot = await getDocs(q);
            if (!querySnapshot.empty) {
              const userData = querySnapshot.docs[0].data();
              return { email: item, department: userData.department, displayName: userData.email };
            }
            return null;
          } catch (error) {
            console.error('Error fetching user data for', item, error);
            return null;
          }
        });

        const participantData = await Promise.all(participantPromises);
        filteredParticipants = participantData
          .filter(p => p && p.department === department)
          .map(p => p.displayName || p.email);
      }

      return `
      <div class="session-card ${isFull ? 'session-full' : ''}">
        <h2>${titles[key]}</h2>
        <div class="session-capacity">
          <span class="capacity-badge ${isFull ? 'capacity-full' : ''}">Max: ${session.capacity} seats</span>
        </div>
        <p><strong>Members:</strong> ${(session.allocated || []).length} / ${session.capacity}</p>
        <p><strong>Seats Left:</strong> ${remaining}</p>
        <p><strong>Your division slots left:</strong> ${deptRemaining} / ${deptCap}</p>
        <div class="worker-list">
          ${
            filteredParticipants.length > 0
              ? filteredParticipants.map(w => `<span class="worker">${w}</span>`).join("")
              : "<p>No participants from your division assigned</p>"
          }
        </div>
        <button onclick="allocateWorkers('${key}')" ${isAllocated || isFull ? 'disabled' : ''} style="${isAllocated || isFull ? 'opacity: 0.5; cursor: not-allowed;' : ''}">
          ${isAllocated ? 'Already Allocated' : isFull ? 'Session Full' : 'Allocate to This Session'}
        </button>
        <button onclick="addDivisionMembers('${key}')" ${!canAddMembers ? 'disabled' : ''} class="add-members-btn" style="${!canAddMembers ? 'opacity: 0.5; cursor: not-allowed;' : ''}">
          Add Division Members
        </button>
      </div>
      `;
    }));

    // Update DOM
    if (loadingEl) loadingEl.style.display = 'none';
    if (gridEl) {
      gridEl.innerHTML = html.join("");
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

      const assignedOverall = sessionKeys.reduce((sum, key) => {
        const totals = sessionsData[key]?.departmentTotals || { A: 0, B: 0, C: 0 };
        return sum + (totals[department] || 0);
      }, 0);
      const totalDepartmentSize = department === 'A' ? 24 : 18;
      const remainingOverall = Math.max(0, totalDepartmentSize - assignedOverall);
      const remainingEl = document.getElementById('division-remaining');
      if (remainingEl) {
        remainingEl.textContent = `${remainingOverall} of ${totalDepartmentSize} ${department === 'A' ? 'Division A' : department === 'B' ? 'Division B' : 'Division C'} members left to assign`;
      }
    }
  } catch (error) {
    console.error('Error loading dashboard:', error);
    if (loadingEl) {
      loadingEl.innerHTML = '<p style="color: red;">Error loading sessions. Please refresh the page.</p>';
    }
  }
}

function setDashboardFeedback(message, variant = 'info') {
  const feedbackEl = document.getElementById('feedback');
  if (!feedbackEl) return;
  feedbackEl.innerHTML = `<p class="feedback-text ${variant}">${message}</p>`;
  feedbackEl.style.display = 'block';
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

// Add members from the current division to a session
window.addDivisionMembers = async function(sessionKey) {
  const currentUserStr = sessionStorage.getItem('seatSync_currentUser');
  if (!currentUserStr) {
    alert('Please log in first');
    return;
  }

  const currentUser = JSON.parse(currentUserStr);
  const department = currentUser.department;
  const sessionKeys = ['morning', 'midday', 'afternoon'];
  const deptCapBySession = { A: 8, B: 6, C: 6 };
  const deptTotalByDivision = { A: 24, B: 18, C: 18 };

  try {
    // Calculate current department allocation across all sessions
    let totalDeptAssigned = 0;
    for (const key of sessionKeys) {
      const sessionRef = doc(db, 'sessions', key);
      const sessionDoc = await getDoc(sessionRef);
      if (sessionDoc.exists()) {
        const sessionData = sessionDoc.data();
        const totals = sessionData.departmentTotals || { A: 0, B: 0, C: 0 };
        totalDeptAssigned += totals[department] || 0;
      }
    }

    const deptRemainingGlobal = deptTotalByDivision[department] - totalDeptAssigned;
    if (deptRemainingGlobal <= 0) {
      alert(`Your division has already assigned all ${deptTotalByDivision[department]} members.`);
      return;
    }

    const sessionRef = doc(db, 'sessions', sessionKey);
    const sessionDoc = await getDoc(sessionRef);
    if (!sessionDoc.exists()) {
      alert('Session not found');
      return;
    }

    const sessionData = sessionDoc.data();
    const allocated = sessionData.allocated || [];
    const capacity = sessionData.capacity || 20;
    const deptTotal = (sessionData.departmentTotals || {})[department] || 0;
    const deptCap = deptCapBySession[department];
    const sessionRemaining = capacity - allocated.length;
    const deptRemainingSession = deptCap - deptTotal;
    const maxAdd = Math.min(deptRemainingGlobal, sessionRemaining, deptRemainingSession);

    if (maxAdd <= 0) {
      alert('No additional members can be added to this session from your division. Session or department limits reached.');
      return;
    }

    const countStr = prompt(`How many members would you like to add to the ${sessionKey} session?\nYou can add up to ${maxAdd} members from your division.`);
    if (!countStr) return;

    const count = Number(countStr);
    if (!Number.isInteger(count) || count <= 0 || count > maxAdd) {
      alert(`Please enter a valid number between 1 and ${maxAdd}.`);
      return;
    }

    const deptMemberCounts = { A: 8, B: 6, C: 6 };
    const currentDeptCount = (sessionData.departmentTotals || {})[department] || 0;
    const newMembers = Array.from({ length: count }, (_, idx) => {
      const memberNum = currentDeptCount + idx + 1;
      return `placeholder:${department}:${memberNum}`;
    });
    const updatedAllocated = [...allocated, ...newMembers];
    const updatedTotals = { ...sessionData.departmentTotals, [department]: deptTotal + count };

    await updateDoc(sessionRef, {
      allocated: updatedAllocated,
      departmentTotals: updatedTotals
    });

    alert(`${count} member(s) added to the ${sessionKey} session from Division ${department}.`);
    setDashboardFeedback(`${count} member(s) added to the ${sessionKey} session from Division ${department}.`, 'success');
    loadDashboardData();
  } catch (error) {
    console.error('Error adding division members:', error);
    alert('Failed to add members. Please try again.');
    setDashboardFeedback('Failed to add members. Please try again.', 'error');
  }
};

// Demo function to fill a session with 20 participants
window.fillSessionDemo = async function() {
  if (!confirm('This will fill the morning session with 20 demo participants. Continue?')) {
    return;
  }

  try {
    const sessionRef = doc(db, 'sessions', 'morning');
    const sessionDoc = await getDoc(sessionRef);

    if (!sessionDoc.exists()) {
      alert('Session not found');
      return;
    }

    // Create 20 demo participants with balanced department distribution
    const demoParticipants = [];
    const departmentTotals = { A: 0, B: 0, C: 0 };

    // Division A: 8 participants
    for (let i = 1; i <= 8; i++) {
      demoParticipants.push(`demo_a${i}@company.com`);
      departmentTotals.A++;
    }

    // Division B: 6 participants
    for (let i = 1; i <= 6; i++) {
      demoParticipants.push(`demo_b${i}@company.com`);
      departmentTotals.B++;
    }

    // Division C: 6 participants
    for (let i = 1; i <= 6; i++) {
      demoParticipants.push(`demo_c${i}@company.com`);
      departmentTotals.C++;
    }

    // Update the session with demo data
    await updateDoc(sessionRef, {
      allocated: demoParticipants,
      departmentTotals: departmentTotals
    });

    alert('Demo session filled with 20 participants!\n\nDivision A: 8 participants\nDivision B: 6 participants\nDivision C: 6 participants\n\nThe morning session is now at maximum capacity (20/20).\n\nThis demonstrates that the system can handle the full 20 participant limit per session.');
    setDashboardFeedback('Demo session filled successfully. Morning session is now full at 20/20.', 'success');

    // Refresh dashboard
    loadDashboardData();

  } catch (error) {
    console.error('Error filling demo session:', error);
    alert('Failed to fill demo session. Please try again.');
    setDashboardFeedback('Failed to fill demo session. Please try again.', 'error');
  }
};

// Demo function to clear the session
window.clearSessionDemo = async function() {
  if (!confirm('This will clear all participants from the morning session. Continue?')) {
    return;
  }

  try {
    const sessionRef = doc(db, 'sessions', 'morning');

    // Reset the session to empty state
    await updateDoc(sessionRef, {
      allocated: [],
      departmentTotals: { A: 0, B: 0, C: 0 }
    });

    alert('Demo session cleared! All participants removed from morning session.');
    setDashboardFeedback('Demo session cleared successfully. Morning session is now reset.', 'success');

    // Refresh dashboard
    loadDashboardData();

  } catch (error) {
    console.error('Error clearing demo session:', error);
    alert('Failed to clear demo session. Please try again.');
  }
};