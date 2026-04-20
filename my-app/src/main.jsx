import './style.css'
import { setupCounter } from './counter.jsx'
import { createRouter } from './router.jsx'
import { homePage } from './pages/home.jsx'
import { dashboardPage } from './pages/dashboard.jsx'
import { loginPage } from './pages/login.jsx'
import { registrationPage } from './pages/registration.jsx'

const routes = {
  '/': homePage,
  '/dashboard': dashboardPage,
  '/login': loginPage,
  '/registration': registrationPage,
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
