export function dashboardPage() {
  return `
<section class="route-shell">
  <nav class="route-nav">
    <a href="#/">Home</a>
    <a href="#/dashboard">Dashboard</a>
    <a href="#/login">Login</a>
    <a href="#/registration">Register</a>
  </nav>
  <section class="route-content">
    <h1>Dashboard</h1>
    <p>Welcome to your dashboard. Use the navigation links to move between pages.</p>
    <ul>
      <li><strong>Profile:</strong> View your account details.</li>
      <li><strong>Activity:</strong> See your latest actions.</li>
      <li><strong>Settings:</strong> Configure your experience.</li>
    </ul>
  </section>
</section>
`
}
