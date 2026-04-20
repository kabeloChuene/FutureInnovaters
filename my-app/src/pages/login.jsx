export function loginPage() {
  return `
<section class="route-shell auth-shell">
  <nav class="auth-topbar">
    <div class="auth-brand">
      <svg width="28" height="28" viewBox="0 0 28 28" fill="none">
        <rect width="28" height="28" rx="6" fill="#1a56db"/>
        <rect x="6" y="6" width="7" height="7" rx="1.5" fill="white"/>
        <rect x="15" y="6" width="7" height="7" rx="1.5" fill="white" opacity="0.6"/>
        <rect x="6" y="15" width="7" height="7" rx="1.5" fill="white" opacity="0.6"/>
        <rect x="15" y="15" width="7" height="7" rx="1.5" fill="white"/>
      </svg>
      <span class="auth-brand-name">SeatSync</span>
    </div>
    <a href="#/registration" class="auth-switch-link">New here? <strong>Create an account</strong></a>
  </nav>

  <div class="auth-layout">
    <div class="auth-panel-left">
      <div class="auth-panel-content">
        <h1 class="auth-panel-heading">Smart Seat<br/>Allocation<br/>Platform</h1>
        <p class="auth-panel-sub">Automated, fair, and real-time training session management — no spreadsheets needed.</p>
        <ul class="auth-features">
          <li><span class="feature-dot"></span>No overbooking, ever</li>
          <li><span class="feature-dot"></span>Department limits enforced automatically</li>
          <li><span class="feature-dot"></span>Live seat availability per session</li>
        </ul>
      </div>
      <div class="auth-panel-decoration">
        <div class="deco-grid">
          <div class="deco-card">Morning<span>09:00–10:30</span><em>20 seats</em></div>
          <div class="deco-card">Midday<span>11:00–12:30</span><em>20 seats</em></div>
          <div class="deco-card">Afternoon<span>13:00–14:30</span><em>20 seats</em></div>
        </div>
      </div>
    </div>

    <div class="auth-card">
      <div class="auth-card-header">
        <h2 class="auth-title">Welcome back</h2>
        <p class="auth-subtitle">Sign in to manage your department's training allocations</p>
      </div>

      <form class="auth-form" id="login-form" onsubmit="handleLogin(event)">
        <div class="form-group">
          <label class="form-label" for="login-email">Work Email</label>
          <div class="input-wrapper">
            <svg class="input-icon" width="16" height="16" viewBox="0 0 16 16" fill="none">
              <path d="M2 4a1 1 0 011-1h10a1 1 0 011 1v8a1 1 0 01-1 1H3a1 1 0 01-1-1V4z" stroke="#93c5fd" stroke-width="1.4"/>
              <path d="M2 4l6 5 6-5" stroke="#93c5fd" stroke-width="1.4" stroke-linecap="round"/>
            </svg>
            <input class="form-input" type="email" id="login-email" name="email" placeholder="you@company.com" required />
          </div>
        </div>

        <div class="form-group">
          <label class="form-label" for="login-password">Password</label>
          <div class="input-wrapper">
            <svg class="input-icon" width="16" height="16" viewBox="0 0 16 16" fill="none">
              <rect x="3" y="7" width="10" height="8" rx="1.5" stroke="#93c5fd" stroke-width="1.4"/>
              <path d="M5 7V5a3 3 0 016 0v2" stroke="#93c5fd" stroke-width="1.4" stroke-linecap="round"/>
            </svg>
            <input class="form-input" type="password" id="login-password" name="password" placeholder="••••••••" required />
          </div>
        </div>

        <div id="login-error" class="form-error" style="display:none"></div>

        <button type="submit" class="auth-btn">
          <span class="auth-btn-text">Sign In to Dashboard</span>
          <svg width="16" height="16" viewBox="0 0 16 16" fill="none">
            <path d="M3 8h10M9 4l4 4-4 4" stroke="white" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round"/>
          </svg>
        </button>
      </form>

      <p class="auth-footer-note">Don't have an account? <a href="#/registration" class="auth-link">Register here</a></p>
    </div>
  </div>
</section>
`
}