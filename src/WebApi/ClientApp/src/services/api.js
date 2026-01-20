import axios from 'axios';

const API_BASE_URL = 'https://localhost:5001/api';

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

export const playerService = {
  getAll: () => api.get('/players'),
  getById: (id) => api.get(`/players/${id}`),
  getByPosition: (position) => api.get(`/players/position/${position}`),
  create: (data) => api.post('/players', data),
  updateStats: (id, data) => api.put(`/players/${id}/stats`, data),
  delete: (id) => api.delete(`/players/${id}`),
};

export const teamService = {
  getAll: () => api.get('/teams'),
  getById: (id) => api.get(`/teams/${id}`),
  create: (data) => api.post('/teams', data),
  addPlayer: (data) => api.post('/teams/add-player', data),
  removePlayer: (teamId, playerId) => api.delete(`/teams/${teamId}/players/${playerId}`),
  delete: (id) => api.delete(`/teams/${id}`),
};

export default api;
