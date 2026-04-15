---
description: React frontend conventions for the ClientApp
globs: ["src/WebApi/ClientApp/**/*.{js,jsx,ts,tsx,css}"]
alwaysApply: false
---

# Frontend Rules

## Component Conventions

- Functional components with hooks only — no class components
- One component per file; filename matches the exported component name (PascalCase)
- Place components in `ClientApp/src/components/`

```jsx
// ✅ Correct
export default function Players() {
    const [players, setPlayers] = useState([]);

    useEffect(() => {
        playerApi.getAll().then(setPlayers);
    }, []);

    return <ul>{players.map(p => <li key={p.id}>{p.name}</li>)}</ul>;
}
```

## API Calls

All API calls go through service files in `ClientApp/src/services/`, not inline `axios` calls in components.

```js
// src/services/api.js
import axios from 'axios';

const BASE = '/api';

export const playerApi = {
    getAll:           ()       => axios.get(`${BASE}/players`).then(r => r.data),
    getById:          (id)     => axios.get(`${BASE}/players/${id}`).then(r => r.data),
    getByPosition:    (pos)    => axios.get(`${BASE}/players/position/${pos}`).then(r => r.data),
    create:           (dto)    => axios.post(`${BASE}/players`, dto).then(r => r.data),
    updateStats:      (id, dto) => axios.put(`${BASE}/players/${id}/stats`, dto).then(r => r.data),
    delete:           (id)     => axios.delete(`${BASE}/players/${id}`),
};
```

## State Management

- Local state via `useState`, side effects via `useEffect`
- No global state library unless complexity clearly justifies it
- Loading and error states must be handled in every component that fetches data

## Routing

Use React Router for navigation. Route definitions live in `App.js`.

## Naming

- Components: PascalCase (`Players.js`, `UserCard.js`)
- Service files: camelCase (`api.js`)
- CSS: same name as component (`Players.css` alongside `Players.js`) when component-specific styles are needed
