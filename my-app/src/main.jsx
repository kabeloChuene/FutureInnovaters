import './style.css'
import { setupCounter } from './counter.jsx'
import { createRouter } from './router.jsx'
import { dashboardPage, loadDashboardData } from './pages/dashboard.jsx'
import { loginPage } from './pages/login.jsx'
import { registrationPage } from './pages/registration.jsx'
import { auth, db } from './firebase/config.js'
import { createUserWithEmailAndPassword, signInWithEmailAndPassword, onAuthStateChanged } from 'firebase/auth'
import { doc, setDoc, getDoc, collection, getDocs, query, where, updateDoc } from 'firebase/firestore'

const routes = {
  '/': registrationPage,
  '/dashboard': dashboardPage,
  '/login': loginPage,
}

const appRoot = document.querySelector('#app')

let currentUser = null;

// Monitor authentication state
onAuthStateChanged(auth, async (user) => {
  if (user) {
    currentUser = user;
    // Fetch user profile from Firestore
    const userDoc = await getDoc(doc(db, 'users', user.uid));
    if (userDoc.exists()) {
      sessionStorage.setItem('seatSync_currentUser', JSON.stringify({ 
        uid: user.uid,
        email: user.email,
        ...userDoc.data() 
      }));
    }
  } else {
    currentUser = null;
    sessionStorage.removeItem('seatSync_currentUser');
  }
});

createRouter(appRoot, routes, {
  defaultRoute: '/',
  onRoute: (route) => {
    const counterButton = document.querySelector('#counter')
    if (counterButton) {
      setupCounter(counterButton)
    }
    
    // Load dashboard data when navigating to dashboard
    if (route === '/dashboard') {
      loadDashboardData();
    }
  }
})

// Registration handler with Firebase
window.handleRegister = async function(e) {
  e.preventDefault();
  const form = e.target;
  const email = form.email.value;
  const password = form.password.value;
  const department = form.department.value;
  const errorEl = document.getElementById('register-error');
  const successEl = document.getElementById('register-success');
  const btn = form.querySelector('button[type="submit"]');

  try {
    // Disable button and show loading state
    btn.disabled = true;
    btn.innerHTML = '<span class="auth-btn-text">Creating account...</span>';
    errorEl.style.display = 'none';
    successEl.style.display = 'none';

    // Create user with Firebase
    const userCredential = await createUserWithEmailAndPassword(auth, email, password);
    const user = userCredential.user;

    // Store user data in Firestore
    await setDoc(doc(db, 'users', user.uid), {
      email: email,
      department: department,
      assignedSession: null,
      createdAt: new Date().toISOString(),
      allocations: {
        morning: false,
        midday: false,
        afternoon: false
      }
    });

    // Initialize shared session documents in Firestore
    const sessions = ['morning', 'midday', 'afternoon'];
    const initialSession = {
      capacity: 20,
      allocated: [],
      departmentTotals: { A: 0, B: 0, C: 0 },
      departmentCapacities: { A: 8, B: 6, C: 6 },
      createdAt: new Date().toISOString()
    };

    for (const session of sessions) {
      const sessionRef = doc(db, 'sessions', session);
      const sessionDoc = await getDoc(sessionRef);
      if (!sessionDoc.exists()) {
        await setDoc(sessionRef, initialSession);
      }
    }

    errorEl.style.display = 'none';
    successEl.textContent = 'Account created! Redirecting to login…';
    successEl.style.display = 'block';

    setTimeout(() => { window.location.hash = '#/login'; }, 1500);
  } catch (error) {
    btn.disabled = false;
    btn.innerHTML = '<span class="auth-btn-text">Create Account</span><svg width="16" height="16" viewBox="0 0 16 16" fill="none"><path d="M3 8h10M9 4l4 4-4 4" stroke="white" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round"/></svg>';
    
    let errorMessage = error.message;
    if (error.code === 'auth/email-already-in-use') {
      errorMessage = 'An account with this email already exists.';
    } else if (error.code === 'auth/weak-password') {
      errorMessage = 'Password should be at least 6 characters.';
    } else if (error.code === 'auth/invalid-email') {
      errorMessage = 'Please enter a valid email address.';
    }
    
    errorEl.textContent = errorMessage;
    errorEl.style.display = 'block';
    successEl.style.display = 'none';
  }
};

// Login handler with Firebase
window.handleLogin = async function(e) {
  e.preventDefault();
  const form = e.target;
  const email = form.email.value;
  const password = form.password.value;
  const errorEl = document.getElementById('login-error');
  const btn = form.querySelector('button[type="submit"]');

  try {
    // Disable button and show loading state
    btn.disabled = true;
    btn.innerHTML = '<span class="auth-btn-text">Signing in...</span>';
    errorEl.style.display = 'none';

    // Sign in with Firebase
    await signInWithEmailAndPassword(auth, email, password);
    
    // Redirect to dashboard
    setTimeout(() => { window.location.hash = '#/dashboard'; }, 500);
  } catch (error) {
    btn.disabled = false;
    btn.innerHTML = '<span class="auth-btn-text">Sign In to Dashboard</span><svg width="16" height="16" viewBox="0 0 16 16" fill="none"><path d="M3 8h10M9 4l4 4-4 4" stroke="white" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round"/></svg>';
    
    let errorMessage = error.message;
    if (error.code === 'auth/user-not-found' || error.code === 'auth/wrong-password') {
      errorMessage = 'Invalid email or password. Please try again.';
    } else if (error.code === 'auth/invalid-email') {
      errorMessage = 'Please enter a valid email address.';
    }
    
    errorEl.textContent = errorMessage;
    errorEl.style.display = 'block';
  }
};

// Allocation handler
window.allocateWorkers = async function(sessionKey) {
  const currentUserStr = sessionStorage.getItem('seatSync_currentUser');
  if (!currentUserStr) {
    alert('Please log in first');
    return;
  }

  const currentUser = JSON.parse(currentUserStr);
  
  try {
    const userRef = doc(db, 'users', currentUser.uid);
    const userDoc = await getDoc(userRef);
    const userData = userDoc.exists() ? userDoc.data() : {};

    if (userData.assignedSession) {
      const msg = userData.assignedSession === sessionKey
        ? 'You are already assigned to this session.'
        : 'A participant can only be assigned to one session.';
      alert(msg);
      return;
    }

    const sessionRef = doc(db, 'sessions', sessionKey);
    const sessionDoc = await getDoc(sessionRef);
    
    if (!sessionDoc.exists()) {
      alert('Session not found');
      return;
    }

    const sessionData = sessionDoc.data();
    const capacity = sessionData.capacity || 20;
    const allocated = sessionData.allocated || [];
    const departmentTotals = sessionData.departmentTotals || { A: 0, B: 0, C: 0 };
    const departmentCapacities = sessionData.departmentCapacities || { A: 8, B: 6, C: 6 };
    const dept = currentUser.department;

    // Check if user is already allocated to this session
    if (allocated.includes(currentUser.email)) {
      alert('You are already allocated to this session');
      return;
    }

    // Check overall session capacity
    if (allocated.length >= capacity) {
      alert('This session is full. Maximum 20 participants allowed.');
      return;
    }

    // Check department cap for this session
    const deptTotal = departmentTotals[dept] || 0;
    const deptCap = departmentCapacities[dept] || (dept === 'A' ? 8 : 6);
    if (deptTotal >= deptCap) {
      alert(`Department ${dept} has reached its per-session limit of ${deptCap} participants.`);
      return;
    }

    // Add user to session
    const updatedAllocated = [...allocated, currentUser.email];
    const updatedTotals = { ...departmentTotals, [dept]: deptTotal + 1 };

    await updateDoc(sessionRef, {
      allocated: updatedAllocated,
      departmentTotals: updatedTotals
    });

    // Update user's allocation record
    await updateDoc(userRef, {
      assignedSession: sessionKey,
      [`allocations.${sessionKey}`]: true
    });

    alert('Successfully allocated to ' + sessionKey + ' session!');
    // Refresh dashboard
    window.location.hash = '#/dashboard';
  } catch (error) {
    console.error('Error allocating worker:', error);
    alert('Failed to allocate. Please try again.');
  }
};
