import React, { useState, useEffect } from 'react';
import { Paper, Typography, Box, TextField } from '@mui/material';
import API_ENDPOINTS from '../config';

function GlobalDashboard() {
  const [globalUsage, setGlobalUsage] = useState(null);
  const [fromDate, setFromDate] = useState(
    new Date(Date.now() - 3600000).toISOString().slice(0, 16) // 1 hour ago
  );
  const [toDate, setToDate] = useState(
    new Date().toISOString().slice(0, 16)
  );

  useEffect(() => {
    async function fetchGlobalUsage() {
      try {
        const url = `${API_ENDPOINTS.globalUsage}?from=${fromDate}&to=${toDate}`;
        const response = await fetch(url);
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
  }, [fromDate, toDate]);

  return (
    <Paper sx={{ p: 3, mt: 3 }}>
      <Typography variant="h6" gutterBottom>
        Global Rate Limit Usage
      </Typography>
      <Box sx={{ mb: 2, display: 'flex', gap: 2 }}>
        <TextField
          label="From Date/Time"
          type="datetime-local"
          value={fromDate}
          onChange={(e) => setFromDate(e.target.value)}
          InputLabelProps={{ shrink: true }}
        />
        <TextField
          label="To Date/Time"
          type="datetime-local"
          value={toDate}
          onChange={(e) => setToDate(e.target.value)}
          InputLabelProps={{ shrink: true }}
        />
      </Box>
      {globalUsage ? (
        <Box>
          <Typography variant="body1">
            Average Usage: {globalUsage.averageUsagePercentage.toFixed(1)}%
          </Typography>
          <Typography variant="body1">
            Total Messages: {globalUsage.totalMessageCount}
          </Typography>
          <Typography variant="body2" color="textSecondary">
            Period: {new Date(globalUsage.fromTime).toLocaleString()} - {new Date(globalUsage.toTime).toLocaleString()}
          </Typography>
        </Box>
      ) : (
        <Typography variant="body1">Loading...</Typography>
      )}
    </Paper>
  );
}

export default GlobalDashboard;