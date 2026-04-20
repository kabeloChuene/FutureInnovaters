import './style.css'
import { setupCounter } from './counter.jsx'
import { createRouter } from './router.jsx'
import { dashboardPage } from './pages/dashboard.jsx'
import { loginPage } from './pages/login.jsx'
import { registrationPage } from './pages/registration.jsx'

const routes = {
  '/': registrationPage,
  '/dashboard': dashboardPage,
  '/login': loginPage,
}

const appRoot = document.querySelector('#app')

createRouter(appRoot, routes, {
  defaultRoute: '/',
  onRoute: () => {
    const counterButton = document.querySelector('#counter')
    if (counterButton) {
      setupCounter(counterButton)
    }
  }
})

window.handleRegister = function(e) {
  e.preventDefault();
  const form = e.target;
  const email = form.email.value;
  const password = form.password.value;
  const department = form.department.value;
  const errorEl = document.getElementById('register-error');
  const successEl = document.getElementById('register-success');

  // Basic validation
  if (password.length < 8) {
    errorEl.textContent = 'Password must be at least 8 characters.';
    errorEl.style.display = 'block';
    successEl.style.display = 'none';
    return;
  }

  // Save to localStorage (replace with real API call when backend is ready)
  const users = JSON.parse(localStorage.getItem('seatSync_users') || '[]');
  if (users.find(u => u.email === email)) {
    errorEl.textContent = 'An account with this email already exists.';
    errorEl.style.display = 'block';
    successEl.style.display = 'none';
    return;
  }

  users.push({ email, password, department });
  localStorage.setItem('seatSync_users', JSON.stringify(users));

  errorEl.style.display = 'none';
  successEl.textContent = 'Account created! Redirecting to login…';
  successEl.style.display = 'block';

  setTimeout(() => { window.location.hash = '#/login'; }, 1500);
};

window.handleLogin = function(e) {
  e.preventDefault();
  const form = e.target;
  const email = form.email.value;
  const password = form.password.value;
  const errorEl = document.getElementById('login-error');

  const users = JSON.parse(localStorage.getItem('seatSync_users') || '[]');
  const user = users.find(u => u.email === email && u.password === password);

  if (!user) {
    errorEl.textContent = 'Invalid email or password. Please try again.';
    errorEl.style.display = 'block';
    return;
  }

  // Store session
  sessionStorage.setItem('seatSync_currentUser', JSON.stringify(user));
  window.location.hash = '#/dashboard';
};
