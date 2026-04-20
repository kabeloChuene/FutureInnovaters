export function createRouter(outlet, routes, options = {}) {
  const defaultRoute = options.defaultRoute || '/'

  const renderRoute = () => {
    const hash = window.location.hash.slice(1) || defaultRoute
    const routePath = routes[hash] ? hash : defaultRoute
    outlet.innerHTML = routes[routePath]()

    if (typeof options.onRoute === 'function') {
      options.onRoute(routePath)
    }
  }

  window.addEventListener('hashchange', renderRoute)
  renderRoute()

  return {
    navigate: (path) => {
      window.location.hash = path
    }
  }
}
