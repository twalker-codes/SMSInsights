// If REACT_APP_API_URL is not set in .env, default to port 5000
const API_BASE_URL = process.env.REACT_APP_API_URL || "http://localhost:5000";

// Log the API URL during development to help with debugging
if (process.env.NODE_ENV === 'development') {
    console.log('API Base URL:', API_BASE_URL);
    console.log('Environment variables:', process.env);
}

const API_ENDPOINTS = {
  globalUsage: `${API_BASE_URL}/api/metrics/global`,
  senderUsage: `${API_BASE_URL}/api/metrics/sender`,
  sendMessage: `${API_BASE_URL}/api/message/send`,
  health: `${API_BASE_URL}/api/health`
};

// Log all endpoints in development
if (process.env.NODE_ENV === 'development') {
    console.log('API Endpoints:', API_ENDPOINTS);
}

export default API_ENDPOINTS;