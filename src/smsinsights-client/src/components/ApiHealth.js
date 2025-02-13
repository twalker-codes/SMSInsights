import React, { useState, useEffect } from 'react';
import { Alert, Collapse, Box, Typography } from '@mui/material';
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
            'Accept': 'application/json',
          },
        });
        
        const data = await response.json();
        
        if (!response.ok) {
          throw new Error(data.message || 'API returned an error status');
        }
        
        setIsHealthy(data.status === 'Healthy');
        setLastChecked(data.timestamp);
        setError(null);
      } catch (err) {
        console.error('Health check failed:', err);
        setIsHealthy(false);
        setError(err.message);
      }
    }

    checkHealth();
    const interval = setInterval(checkHealth, 10000); // Check every 10 seconds
    return () => clearInterval(interval);
  }, []);

  return (
    <Box>
      <Collapse in={!isHealthy}>
        <Alert 
          severity="error" 
          sx={{ mb: 2 }}
        >
          <Typography variant="body1">
            API service is currently unavailable
            {error && `: ${error}`}
          </Typography>
          {lastChecked && (
            <Typography variant="body2">
              Last checked: {new Date(lastChecked).toLocaleString()}
            </Typography>
          )}
        </Alert>
      </Collapse>
      <Collapse in={isHealthy}>
        <Alert 
          severity="success" 
          sx={{ mb: 2 }}
        >
          API service is healthy
        </Alert>
      </Collapse>
    </Box>
  );
}

export default ApiHealth; 