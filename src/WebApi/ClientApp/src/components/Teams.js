import React, { useState, useEffect } from 'react';
import { teamService, playerService } from '../services/api';

function Teams() {
  const [teams, setTeams] = useState([]);
  const [players, setPlayers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [success, setSuccess] = useState(null);
  const [showCreateForm, setShowCreateForm] = useState(false);
  const [newTeam, setNewTeam] = useState({ name: '', managerName: '' });
  const [selectedTeam, setSelectedTeam] = useState(null);

  useEffect(() => {
    loadTeams();
    loadPlayers();
  }, []);

  const loadTeams = async () => {
    try {
      setLoading(true);
      const response = await teamService.getAll();
      setTeams(response.data);
      setError(null);
    } catch (err) {
      setError('Failed to load teams. Please try again.');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const loadPlayers = async () => {
    try {
      const response = await playerService.getAll();
      setPlayers(response.data);
    } catch (err) {
      console.error(err);
    }
  };

  const handleCreateTeam = async (e) => {
    e.preventDefault();
    try {
      await teamService.create(newTeam);
      setSuccess('Team created successfully!');
      setNewTeam({ name: '', managerName: '' });
      setShowCreateForm(false);
      loadTeams();
      setTimeout(() => setSuccess(null), 3000);
    } catch (err) {
      setError('Failed to create team. Please try again.');
      console.error(err);
    }
  };

  const handleAddPlayer = async (teamId, playerId) => {
    try {
      await teamService.addPlayer({ teamId, playerId });
      setSuccess('Player added to team!');
      loadTeams();
      setTimeout(() => setSuccess(null), 3000);
    } catch (err) {
      setError(err.response?.data || 'Failed to add player to team.');
      setTimeout(() => setError(null), 5000);
    }
  };

  const handleRemovePlayer = async (teamId, playerId) => {
    try {
      await teamService.removePlayer(teamId, playerId);
      setSuccess('Player removed from team!');
      loadTeams();
      setTimeout(() => setSuccess(null), 3000);
    } catch (err) {
      setError('Failed to remove player from team.');
      setTimeout(() => setError(null), 3000);
    }
  };

  if (loading) return <div className="loading">Loading teams...</div>;

  return (
    <div>
      {error && <div className="error">{error}</div>}
      {success && <div className="success">{success}</div>}

      <div className="card">
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <h2>My Teams</h2>
          <button 
            className="btn" 
            onClick={() => setShowCreateForm(!showCreateForm)}
          >
            {showCreateForm ? 'Cancel' : 'Create New Team'}
          </button>
        </div>

        {showCreateForm && (
          <form onSubmit={handleCreateTeam} style={{ marginTop: '2rem' }}>
            <div className="form-group">
              <label>Team Name</label>
              <input
                type="text"
                value={newTeam.name}
                onChange={(e) => setNewTeam({ ...newTeam, name: e.target.value })}
                required
              />
            </div>
            <div className="form-group">
              <label>Manager Name</label>
              <input
                type="text"
                value={newTeam.managerName}
                onChange={(e) => setNewTeam({ ...newTeam, managerName: e.target.value })}
                required
              />
            </div>
            <button type="submit" className="btn">Create Team</button>
          </form>
        )}
      </div>

      {teams.length === 0 ? (
        <div className="empty-state">
          <p>No teams created yet.</p>
          <p>Create your first team to get started!</p>
        </div>
      ) : (
        teams.map(team => (
          <div key={team.id} className="card team-section">
            <h3>{team.name}</h3>
            <p style={{ color: '#666', marginBottom: '1rem' }}>Manager: {team.managerName}</p>

            <div className="team-stats">
              <div className="stat-card">
                <h4>Budget Remaining</h4>
                <div className="value">£{team.budget.toFixed(1)}m</div>
              </div>
              <div className="stat-card">
                <h4>Total Points</h4>
                <div className="value">{team.totalPoints}</div>
              </div>
              <div className="stat-card">
                <h4>Players</h4>
                <div className="value">{team.players.length}/15</div>
              </div>
            </div>

            <div style={{ marginTop: '1.5rem' }}>
              <h4 style={{ marginBottom: '1rem' }}>Team Players</h4>
              {team.players.length === 0 ? (
                <p style={{ color: '#666' }}>No players in team yet.</p>
              ) : (
                <div className="players-grid">
                  {team.players.map(player => (
                    <div key={player.id} className="player-card">
                      <h3>{player.name}</h3>
                      <div className="player-info">
                        <p><strong>Position:</strong> <span>{player.position}</span></p>
                        <p><strong>Club:</strong> <span>{player.club}</span></p>
                        <p><strong>Price:</strong> <span>£{player.price}m</span></p>
                        <p><strong>Points:</strong> <span>{player.points}</span></p>
                      </div>
                      <button 
                        className="btn btn-danger" 
                        style={{ width: '100%' }}
                        onClick={() => handleRemovePlayer(team.id, player.id)}
                      >
                        Remove
                      </button>
                    </div>
                  ))}
                </div>
              )}
            </div>

            {team.players.length < 15 && (
              <div style={{ marginTop: '1.5rem' }}>
                <button 
                  className="btn" 
                  onClick={() => setSelectedTeam(selectedTeam === team.id ? null : team.id)}
                >
                  {selectedTeam === team.id ? 'Hide Available Players' : 'Add Players'}
                </button>

                {selectedTeam === team.id && (
                  <div className="players-grid" style={{ marginTop: '1rem' }}>
                    {players
                      .filter(p => !team.players.some(tp => tp.id === p.id))
                      .map(player => (
                        <div key={player.id} className="player-card">
                          <h3>{player.name}</h3>
                          <div className="player-info">
                            <p><strong>Position:</strong> <span>{player.position}</span></p>
                            <p><strong>Club:</strong> <span>{player.club}</span></p>
                            <p><strong>Price:</strong> <span>£{player.price}m</span></p>
                            <p><strong>Points:</strong> <span>{player.points}</span></p>
                          </div>
                          <button 
                            className="btn" 
                            style={{ width: '100%' }}
                            onClick={() => handleAddPlayer(team.id, player.id)}
                            disabled={team.budget < player.price}
                          >
                            {team.budget < player.price ? 'Insufficient Budget' : 'Add to Team'}
                          </button>
                        </div>
                      ))}
                  </div>
                )}
              </div>
            )}
          </div>
        ))
      )}
    </div>
  );
}

export default Teams;
