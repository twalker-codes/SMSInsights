import React, { useState, useEffect } from 'react';
import { Paper, Typography, Box, TextField, Button } from '@mui/material';
import API_ENDPOINTS from '../config';

function SenderDashboard() {
  const [phoneNumber, setPhoneNumber] = useState('');
  const [fromDate, setFromDate] = useState(
    new Date(Date.now() - 3600000).toISOString().slice(0, 16) // 1 hour ago
  );
  const [toDate, setToDate] = useState(
    new Date().toISOString().slice(0, 16)
  );
  const [senderUsage, setSenderUsage] = useState(null);

  async function fetchSenderUsage() {
    if (phoneNumber.trim() === '') return;
    try {
      const url = `${API_ENDPOINTS.senderUsage}/${encodeURIComponent(phoneNumber)}?from=${fromDate}&to=${toDate}`;
      const response = await fetch(url);
      if (!response.ok) throw new Error('Network error');
      const data = await response.json();
      setSenderUsage(data);
    } catch (err) {
      console.error(err);
    }
  }

  useEffect(() => {
    if (phoneNumber.trim() === '') return;
    fetchSenderUsage();
    const interval = setInterval(fetchSenderUsage, 1000);
    return () => clearInterval(interval);
  }, [phoneNumber, fromDate, toDate]);

  return (
    <Paper sx={{ p: 3, mt: 3 }}>
      <Typography variant="h6" gutterBottom>
        Sender Rate Limit Usage
      </Typography>
      <Box component="form" sx={{ display: 'flex', flexDirection: 'column', gap: 2, mb: 2 }}>
        <TextField
          label="Sender Phone Number"
          value={phoneNumber}
          onChange={(e) => setPhoneNumber(e.target.value)}
        />
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
        <Button variant="contained" onClick={fetchSenderUsage}>
          Filter
        </Button>
      </Box>
      {senderUsage ? (
        <Box>
          <Typography variant="body1">
            Average Usage: {senderUsage.averageUsagePercentage.toFixed(1)}%
          </Typography>
          <Typography variant="body1">
            Total Messages: {senderUsage.totalMessageCount}
          </Typography>
          <Typography variant="body2" color="textSecondary">
            Period: {new Date(senderUsage.fromTime).toLocaleString()} - {new Date(senderUsage.toTime).toLocaleString()}
          </Typography>
        </Box>
      ) : (
        <Typography variant="body1">
          Enter a sender phone number to view usage.
        </Typography>
      )}
    </Paper>
  );
}

export default SenderDashboard;