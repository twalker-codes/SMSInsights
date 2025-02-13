import React, { useState, useEffect } from 'react';
import { Alert, Box, Typography } from '@mui/material';
import API_ENDPOINTS from '../config';

function ApiHealth() {
  const [isHealthy, setIsHealthy] = useState(true);
  const [lastChecked, setLastChecked] = useState(null);
  const [error, setError] = useState(null);

  useEffect(() => {
    async function checkHealth() {
      try {
        const response = await fetch(API_ENDPOINTS.health, {
          method: 'GET',
          headers: {
            'Accept': 'application/json'
          },
          signal: AbortSignal.timeout(3000)
        });
        
        if (!response.ok) {
          throw new Error('API service unavailable');
        }
        
        const data = await response.json();
        setIsHealthy(true);
        setLastChecked(data.timestamp);
        setError(null);
      } catch (err) {
        setIsHealthy(false);
        setError('Cannot connect to API server');
        setLastChecked(new Date());
      }
    }

    checkHealth();
    const interval = setInterval(checkHealth, 10000);
    return () => clearInterval(interval);
  }, []);

  return (
    <Box sx={{ mb: 2 }}>
      <Alert severity={isHealthy ? "success" : "error"}>
        <Typography variant="body1">
          {isHealthy ? 'API service is healthy' : 'API service is currently unavailable'}
          {!isHealthy && error && `: ${error}`}
        </Typography>
        {lastChecked && (
          <Typography variant="body2">
            Last checked: {new Date(lastChecked).toLocaleString()}
          </Typography>
        )}
      </Alert>
    </Box>
  );
}

export default ApiHealth; 