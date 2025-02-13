const API_BASE_URL = "http://localhost:5000";

const API_ENDPOINTS = {
  globalUsage: `${API_BASE_URL}/api/globalUsage`,
  senderUsage: `${API_BASE_URL}/api/senderUsage`,
  sendMessage: `${API_BASE_URL}/api/message/send`
};

export default API_ENDPOINTS;