export function registrationPage() {
  return `
<section class="route-shell">
  <nav class="route-nav">
    <a href="#/">Home</a>
    <a href="#/dashboard">Dashboard</a>
    <a href="#/login">Login</a>
    <a href="#/registration">Register</a>
  </nav>
  <section class="route-content">
    <h1>Registration</h1>
    <form class="auth-form">
      <label>
        Full Name
        <input type="text" name="name" placeholder="Jane Doe" />
      </label>
      <label>
        Email
        <input type="email" name="email" placeholder="you@example.com" />
      </label>
      <label>
        Password
        <input type="password" name="password" placeholder="••••••••" />
      </label>
      <button type="submit">Create Account</button>
    </form>
  </section>
</section>
`
}
