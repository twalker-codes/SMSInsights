const API_BASE_URL = "http://localhost:5000";

const API_ENDPOINTS = {
  globalUsage: `${API_BASE_URL}/api/metrics/global`,
  senderUsage: `${API_BASE_URL}/api/metrics/sender`,
  sendMessage: `${API_BASE_URL}/api/message/send`,
  health: `${API_BASE_URL}/api/health`
};

export default API_ENDPOINTS;