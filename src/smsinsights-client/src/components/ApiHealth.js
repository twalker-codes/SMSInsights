import React, { useState, useEffect } from 'react';
import { Alert, Collapse } from '@mui/material';
import API_ENDPOINTS from '../config';

function ApiHealth() {
  const [isHealthy, setIsHealthy] = useState(true);
  const [lastChecked, setLastChecked] = useState(null);

  useEffect(() => {
    async function checkHealth() {
      try {
        const response = await fetch(API_ENDPOINTS.health);
        if (!response.ok) throw new Error('API health check failed');
        const data = await response.json();
        setIsHealthy(data.status === 'Healthy');
        setLastChecked(data.timestamp);
      } catch (err) {
        console.error('Health check failed:', err);
        setIsHealthy(false);
      }
    }

    checkHealth();
    const interval = setInterval(checkHealth, 30000); // Check every 30 seconds
    return () => clearInterval(interval);
  }, []);

  return (
    <Collapse in={!isHealthy}>
      <Alert severity="error" sx={{ mb: 2 }}>
        API service is currently unavailable
      </Alert>
    </Collapse>
  );
}

export default ApiHealth; 