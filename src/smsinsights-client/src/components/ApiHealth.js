import React, { useState, useEffect } from 'react';
import { Alert, Collapse, Box, Typography } from '@mui/material';
import API_ENDPOINTS from '../config';

function ApiHealth() {
  const [isHealthy, setIsHealthy] = useState(true);
  const [lastChecked, setLastChecked] = useState(null);
  const [error, setError] = useState(null);
  const [retryCount, setRetryCount] = useState(0);

  useEffect(() => {
    async function checkHealth() {
      try {
        console.log('Attempting health check to:', API_ENDPOINTS.health); // Debug log

        const response = await fetch(API_ENDPOINTS.health, {
          method: 'GET',
          headers: {
            'Accept': 'application/json',
            'Cache-Control': 'no-cache',
          },
          mode: 'cors', // Explicitly enable CORS
          signal: AbortSignal.timeout(5000)
        });
        
        const data = await response.json();
        
        if (!response.ok) {
          throw new Error(data.message || `API returned status ${response.status}`);
        }
        
        setIsHealthy(data.status === 'Healthy');
        setLastChecked(data.timestamp);
        setError(null);
        setRetryCount(0); // Reset retry count on success
      } catch (err) {
        console.error('Health check failed:', err);
        setIsHealthy(false);
        
        let errorMessage = '';
        if (err.name === 'AbortError') {
          errorMessage = 'API request timed out after 5 seconds';
        } else if (err.name === 'TypeError' && err.message === 'Failed to fetch') {
          errorMessage = `Cannot connect to API server at ${API_ENDPOINTS.health} - please check if the server is running`;
        } else {
          errorMessage = err.message;
        }
        
        setError(errorMessage);
        setRetryCount(prev => prev + 1);
      }
    }

    checkHealth();
    
    // Adjust polling interval based on connection status
    const interval = setInterval(
      checkHealth, 
      isHealthy ? 10000 : Math.min(5000 * (retryCount + 1), 30000) // Exponential backoff up to 30s
    );
    
    return () => clearInterval(interval);
  }, [isHealthy, retryCount]);

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
          <Typography variant="body2" color="textSecondary">
            Retry attempt: {retryCount}
          </Typography>
        </Alert>
      </Collapse>
      <Collapse in={isHealthy}>
        <Alert 
          severity="success" 
          sx={{ mb: 2 }}
        >
          <Typography variant="body1">
            API service is healthy
          </Typography>
          {lastChecked && (
            <Typography variant="body2">
              Last checked: {new Date(lastChecked).toLocaleString()}
            </Typography>
          )}
        </Alert>
      </Collapse>
    </Box>
  );
}

export default ApiHealth; 