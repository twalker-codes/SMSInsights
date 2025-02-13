import React, { useState, useEffect } from 'react';
import { Paper, Typography, Box } from '@mui/material';
import API_ENDPOINTS from '../config';

function GlobalDashboard() {
  const [globalUsage, setGlobalUsage] = useState(null);
  const [apiResponse, setApiResponse] = useState(null);

  // Poll the backend every second for live global usage data.
  useEffect(() => {
    async function fetchGlobalUsage() {
      try {
        const response = await fetch(API_ENDPOINTS.globalUsage);
        if (!response.ok) throw new Error('Network error');
        const data = await response.json();
        setGlobalUsage(data);
        setApiResponse({
          status: response.status,
          timestamp: new Date().toLocaleString()
        });
      } catch (err) {
        console.error(err);
        setApiResponse({
          status: 'Error',
          message: err.message,
          timestamp: new Date().toLocaleString()
        });
      }
    }

    fetchGlobalUsage();
    const interval = setInterval(fetchGlobalUsage, 1000);
    return () => clearInterval(interval);
  }, []);

  return (
    <Paper sx={{ p: 3, mt: 3 }}>
      <Typography variant="h6" gutterBottom>
        Global Rate Limit Usage
      </Typography>
      {globalUsage ? (
        <Box>
          <Typography variant="body1">
            Usage: {globalUsage.usagePercentage}%
          </Typography>
          <Typography variant="body2" color="textSecondary">
            Last Updated: {new Date(globalUsage.timestamp).toLocaleString()}
          </Typography>
        </Box>
      ) : (
        <Typography variant="body1">Loading...</Typography>
      )}
      {apiResponse && (
        <Box sx={{ mt: 2, pt: 2, borderTop: 1, borderColor: 'divider' }}>
          <Typography variant="body2" color="textSecondary">
            Last API Call: {apiResponse.status === 'Error' ? 
              `Error - ${apiResponse.message}` : 
              `Success (${apiResponse.status})`} at {apiResponse.timestamp}
          </Typography>
        </Box>
      )}
    </Paper>
  );
}

export default GlobalDashboard;