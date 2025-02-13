import React, { useState, useEffect } from 'react';
import { Paper, Typography, Box, TextField, Button } from '@mui/material';
import API_ENDPOINTS from '../config';

function SenderDashboard() {
  const [phoneNumber, setPhoneNumber] = useState('');
  const [fromDate, setFromDate] = useState('');
  const [toDate, setToDate] = useState('');
  const [senderUsage, setSenderUsage] = useState(null);
  const [apiResponse, setApiResponse] = useState(null);

  // Fetch sender usage based on the entered phone number and date range.
  async function fetchSenderUsage() {
    if (phoneNumber.trim() === '') return;
    try {
      const url = `${API_ENDPOINTS.senderUsage}/${encodeURIComponent(phoneNumber)}`;
      const response = await fetch(url);
      if (!response.ok) throw new Error('Network error');
      const data = await response.json();
      setSenderUsage(data);
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

  // Poll the backend for sender usage once a phone number is specified.
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
          InputLabelProps={{
            shrink: true,
          }}
          value={fromDate}
          onChange={(e) => setFromDate(e.target.value)}
        />
        <TextField
          label="To Date/Time"
          type="datetime-local"
          InputLabelProps={{
            shrink: true,
          }}
          value={toDate}
          onChange={(e) => setToDate(e.target.value)}
        />
        <Button variant="contained" onClick={fetchSenderUsage}>
          Filter
        </Button>
      </Box>
      {senderUsage ? (
        <Box>
          <Typography variant="body1">
            Messages per second: {senderUsage.currentCount} / {senderUsage.maxCount}
          </Typography>
          <Typography variant="body1">
            Usage: {senderUsage.usagePercentage}%
          </Typography>
        </Box>
      ) : (
        <Typography variant="body1">
          Enter a sender phone number to view usage.
        </Typography>
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

export default SenderDashboard;