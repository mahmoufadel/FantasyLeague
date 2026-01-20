import React, { useState, useEffect } from 'react';
import { playerService } from '../services/api';

function Players({ onAddToTeam }) {
  const [players, setPlayers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [filter, setFilter] = useState('All');

  useEffect(() => {
    loadPlayers();
  }, []);

  const loadPlayers = async () => {
    try {
      setLoading(true);
      const response = await playerService.getAll();
      setPlayers(response.data);
      setError(null);
    } catch (err) {
      setError('Failed to load players. Please try again.');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const filteredPlayers = filter === 'All' 
    ? players 
    : players.filter(p => p.position === filter);

  if (loading) return <div className="loading">Loading players...</div>;
  if (error) return <div className="error">{error}</div>;

  return (
    <div>
      <div className="card">
        <h2>Available Players</h2>
        <div style={{ marginTop: '1rem' }}>
          <label style={{ marginRight: '1rem' }}>Filter by Position:</label>
          {['All', 'Goalkeeper', 'Defender', 'Midfielder', 'Forward'].map(pos => (
            <button
              key={pos}
              onClick={() => setFilter(pos)}
              className={`btn btn-secondary ${filter === pos ? 'active' : ''}`}
              style={{ marginRight: '0.5rem', marginBottom: '0.5rem' }}
            >
              {pos}
            </button>
          ))}
        </div>
      </div>

      <div className="players-grid">
        {filteredPlayers.map(player => (
          <div key={player.id} className="player-card">
            <h3>{player.name}</h3>
            <div className="player-info">
              <p><strong>Position:</strong> <span>{player.position}</span></p>
              <p><strong>Club:</strong> <span>{player.club}</span></p>
              <p><strong>Price:</strong> <span>£{player.price}m</span></p>
              <p><strong>Points:</strong> <span>{player.points}</span></p>
              <p><strong>Goals:</strong> <span>{player.goalsScored}</span></p>
              <p><strong>Assists:</strong> <span>{player.assists}</span></p>
            </div>
            {onAddToTeam && (
              <button 
                className="btn" 
                style={{ width: '100%' }}
                onClick={() => onAddToTeam(player)}
              >
                Add to Team
              </button>
            )}
          </div>
        ))}
      </div>

      {filteredPlayers.length === 0 && (
        <div className="empty-state">
          <p>No players found for this position.</p>
        </div>
      )}
    </div>
  );
}

export default Players;
