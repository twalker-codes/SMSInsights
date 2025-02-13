import React, { useState, useEffect } from 'react';
import { Paper, Typography, Box } from '@mui/material';
import API_ENDPOINTS from '../config';

function GlobalDashboard() {
  const [globalUsage, setGlobalUsage] = useState(null);

  // Poll the backend every second for live global usage data.
  useEffect(() => {
    async function fetchGlobalUsage() {
      try {
        const response = await fetch(API_ENDPOINTS.globalUsage);
        if (!response.ok) throw new Error('Network error');
        const data = await response.json();
        setGlobalUsage(data);
      } catch (err) {
        console.error(err);
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
    </Paper>
  );
}

export default GlobalDashboard;