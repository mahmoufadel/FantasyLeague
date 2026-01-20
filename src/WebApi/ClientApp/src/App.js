import React, { useState } from 'react';
import './App.css';
import Players from './components/Players';
import Teams from './components/Teams';

function App() {
  const [activeTab, setActiveTab] = useState('players');

  return (
    <div className="app">
      <header className="header">
        <h1>⚽ Fantasy Premier League</h1>
        <p>Build your dream team and compete!</p>
        <nav className="nav">
          <button 
            className={activeTab === 'players' ? 'active' : ''}
            onClick={() => setActiveTab('players')}
          >
            Players
          </button>
          <button 
            className={activeTab === 'teams' ? 'active' : ''}
            onClick={() => setActiveTab('teams')}
          >
            My Teams
          </button>
        </nav>
      </header>

      <main className="container">
        {activeTab === 'players' && <Players />}
        {activeTab === 'teams' && <Teams />}
      </main>

      <footer style={{ 
        textAlign: 'center', 
        padding: '2rem', 
        color: '#666',
        borderTop: '1px solid #e0e0e0',
        marginTop: '4rem'
      }}>
        <p>Fantasy Premier League © 2026 | Built with .NET 9 & React</p>
      </footer>
    </div>
  );
}

export default App;
