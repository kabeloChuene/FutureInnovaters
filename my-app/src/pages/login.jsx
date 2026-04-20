export function loginPage() {
  return `
<section class="route-shell">
  <nav class="route-nav">
    <a href="#/">Home</a>
    <a href="#/dashboard">Dashboard</a>
    <a href="#/login">Login</a>
    <a href="#/registration">Register</a>
  </nav>
  <section class="route-content">
    <h1>Login</h1>
    <form class="auth-form">
      <label>
        Email
        <input type="email" name="email" placeholder="you@example.com" />
      </label>
      <label>
        Password
        <input type="password" name="password" placeholder="••••••••" />
      </label>
      <button type="submit">Sign In</button>
    </form>
  </section>
</section>
`
}
