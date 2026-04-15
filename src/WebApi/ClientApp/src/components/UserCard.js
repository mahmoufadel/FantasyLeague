import React, { useState, useEffect, useCallback, useMemo } from "react";

function UserCard({ name, age }) {
  const [likes, setLikes] = useState(0);
 const [showDetails, setShowDetails] = useState(true);
  const [user, setUser] = useState({ name: "Mahmoud" });

  // 🔹 Mount effect
  useEffect(() => {
    console.log("UserCard mounted");

    return () => {
      console.log("UserCard unmounted");
    };
  }, []);

  // 🔹 Effect when likes change
  useEffect(() => {
    console.log("Likes updated:", likes);
  }, [likes]);

  // 🔹 Like handler (functional update)
  const handleLike = useCallback(() => {
    setLikes(prev => prev + 1);
  }, []);

  // 🔹 Reset likes
  const handleReset = useCallback(() => {
    setLikes(0);
  }, []);

  // 🔹 Toggle details
  const toggleDetails = () => {
    setShowDetails(prev => !prev);
  };

  // 🔹 Update name
  const changeName = () => {
    setUser(prev => ({ ...prev, name: "Ali" }));
  };

    const popularityLevel = useMemo(() => {
    if (likes > 10) return "🔥 Superstar";
    if (likes > 5) return "⭐ Popular";
    return "🙂 Normal";
  }, [likes]);

  return (
    <div style={{ border: "1px solid gray", padding: "10px", margin: "10px" }}>
      <h2>{user.name}</h2>

      {showDetails && (
        <>
          <p>Age: {age}</p>
          <p>Likes: {likes}</p>
          <p>Status: {popularityLevel}</p>
        </>
      )}

      <button onClick={handleLike}>Like</button>
      <button onClick={handleReset}>Reset</button>
      <button onClick={toggleDetails}>
        {showDetails ? "Hide" : "Show"} Details
      </button>
      <button onClick={changeName}>Change Name</button>
    </div>
  );
}

export default UserCard;