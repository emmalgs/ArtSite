window.authHelper = {
  setToken: function(token, expiry) {
    localStorage.setItem('authToken', token),
    localStorage.setItem('tokenExpiry', expiry)
  },
  getToken: function() {
    return localStorage.getItem('authToken')
  },
  getExpiry: function() {
    return localStorage.getItem('tokenExpiry')
  },
  removeToken: function() {
    localStorage.removeItem('authToken'),
    localStorage.removeItem('tokenExpiry')
  },
  isTokenExpired: function() {
    const expiry = localStorage.getItem('tokenExpiry')
    if (!expiry) return true;
    return new Date(expiry) <= new Date();
  }
}