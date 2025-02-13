import React from 'react';
import { Container, Box, Tabs, Tab } from '@mui/material';
import GlobalDashboard from './components/GlobalDashboard';
import SenderDashboard from './components/SenderDashboard';

function App() {
  const [tabIndex, setTabIndex] = React.useState(0);

  const handleTabChange = (event, newValue) => {
    setTabIndex(newValue);
  };

  return (
    <Container maxWidth="md">
      <Box sx={{ my: 4 }}>
        <Tabs value={tabIndex} onChange={handleTabChange} centered>
          <Tab label="Global Overview" />
          <Tab label="Sender Overview" />
        </Tabs>
        {tabIndex === 0 && <GlobalDashboard />}
        {tabIndex === 1 && <SenderDashboard />}
      </Box>
    </Container>
  );
}

export default App;